using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace MozizzAdminKliens.Windows
{
    public partial class MoviesWindow : Window
    {
        private readonly HttpClient _client;

        public MoviesWindow(HttpClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) => await LoadMovies();

        private async Task LoadMovies()
        {
            tbStatus.Text = "Betöltés...";
            try
            {
                var response = await _client.GetAsync("Movie/GetMovies");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    var list = data.EnumerateArray().Select(m => new
                    {
                        movieId = m.TryGetProperty("movieId", out var id) ? id.GetInt32() : 0,
                        title = m.TryGetProperty("title", out var t) ? t.GetString() : "",
                        genre = m.TryGetProperty("genre", out var g) ? g.GetString() : "",
                        duration = m.TryGetProperty("duration", out var d) ? d.GetInt32() : 0,
                        description = m.TryGetProperty("description", out var desc) ? desc.GetString() : "",
                    }).ToList();
                    dgMovies.ItemsSource = list;
                    tbStatus.Text = $"{list.Count} film betöltve.";
                }
                else
                {
                    tbStatus.Text = "Hiba a betöltés során.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}");
            }
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e) => await LoadMovies();

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgMovies.SelectedItem == null)
            {
                MessageBox.Show("Válassz ki egy filmet!");
                return;
            }

            dynamic selected = dgMovies.SelectedItem;
            int id = selected.movieId;
            string title = selected.title;

            var confirm = MessageBox.Show($"Biztosan törlöd a(z) \"{title}\" filmet?", "Megerősítés",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            var response = await _client.DeleteAsync($"Movie/DeleteMovie/{id}");
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Film törölve!");
                await LoadMovies();
            }
            else
            {
                string err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Hiba: {err}");
            }
        }
    }
}
