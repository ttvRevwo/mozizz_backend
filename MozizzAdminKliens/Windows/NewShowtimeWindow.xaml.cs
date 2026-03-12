using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace MozizzAdminKliens.Windows
{
    public partial class NewShowtimeWindow : Window
    {
        private readonly HttpClient _client;

        public NewShowtimeWindow(HttpClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadMovies();
            await LoadHalls();
        }

        private async Task LoadMovies()
        {
            try
            {
                var response = await _client.GetAsync("Movie/GetMovies");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    var movies = data.EnumerateArray().Select(m => new
                    {
                        movieId = m.TryGetProperty("movieId", out var id) ? id.GetInt32() : 0,
                        title = m.TryGetProperty("title", out var t) ? t.GetString() : "",
                    }).ToList();
                    cmbMovies.ItemsSource = movies;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Filmek betöltési hiba: {ex.Message}");
            }
        }

        private async Task LoadHalls()
        {
            try
            {
                var response = await _client.GetAsync("Hall/GetAllHall");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    var halls = data.EnumerateArray().Select(h => new
                    {
                        hallId = h.TryGetProperty("hallId", out var id) ? id.GetInt32() :
                                 h.TryGetProperty("HallId", out var Id) ? Id.GetInt32() : 0,
                        name = h.TryGetProperty("name", out var n) ? n.GetString() :
                               h.TryGetProperty("Name", out var N) ? N.GetString() : "",
                    }).ToList();
                    cmbHalls.ItemsSource = halls;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Termek betöltési hiba: {ex.Message}");
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMovies.SelectedItem == null)
            {
                MessageBox.Show("Válassz filmet!");
                return;
            }
            if (cmbHalls.SelectedItem == null)
            {
                MessageBox.Show("Válassz termet!");
                return;
            }

            string timeInput = tbxTime.Text.Trim();
            if (timeInput.Length == 5) timeInput += ":00";

            dynamic selectedMovie = cmbMovies.SelectedItem;
            dynamic selectedHall = cmbHalls.SelectedItem;

            int movieId = (int)selectedMovie.movieId;
            string movieTitle = (string)selectedMovie.title;
            int hallId = (int)selectedHall.hallId;
            string hallName = (string)selectedHall.name;

            var body = JsonSerializer.Serialize(new
            {
                MovieId = movieId,
                ShowDate = tbxDate.Text.Trim(),
                ShowTime1 = timeInput,
                HallId = hallId,
                Movie = new { MovieId = movieId, Title = movieTitle },
                Hall = new { HallId = hallId, Name = hallName }
            });

            var response = await _client.PostAsync("Showtime/NewShowtime",
                new StringContent(body, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Vetítés sikeresen hozzáadva!");
                Close();
            }
            else
            {
                string err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Státusz: {response.StatusCode}\nHiba: {err}");
            }
        }
    }
}