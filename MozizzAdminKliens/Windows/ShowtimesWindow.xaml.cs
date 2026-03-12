using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace MozizzAdminKliens.Windows
{
    public partial class ShowtimesWindow : Window
    {
        private readonly HttpClient _client;

        public ShowtimesWindow(HttpClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) => await LoadShowtimes();

        private async Task LoadShowtimes()
        {
            tbStatus.Text = "Betöltés...";
            try
            {
                var response = await _client.GetAsync("Showtime/GetAllShowtimes");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    var list = data.EnumerateArray().Select(s => new
                    {
                        showtimeId = s.TryGetProperty("showtimeId", out var id) ? id.GetInt32() : 0,
                        movieTitle = s.TryGetProperty("movieTitle", out var mt) ? mt.GetString() : "",
                        showDate = s.TryGetProperty("date", out var sd) ? sd.GetString() : "",
                        showTime = s.TryGetProperty("time", out var st) ? st.GetString() : "",
                        hallName = s.TryGetProperty("hallName", out var hn) ? hn.GetString() : "",
                    }).ToList();
                    dgShowtimes.ItemsSource = list;
                    tbStatus.Text = $"{list.Count} vetítés betöltve.";
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

        private async void btnRefresh_Click(object sender, RoutedEventArgs e) => await LoadShowtimes();

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgShowtimes.SelectedItem == null)
            {
                MessageBox.Show("Válassz ki egy vetítést!");
                return;
            }

            dynamic selected = dgShowtimes.SelectedItem;
            int id = selected.showtimeId;

            var confirm = MessageBox.Show($"Biztosan törlöd a(z) {id}. vetítést?", "Megerősítés",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            var response = await _client.DeleteAsync($"Showtime/DeleteShowtime/{id}");
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Vetítés törölve!");
                await LoadShowtimes();
            }
            else
            {
                string err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Hiba: {err}");
            }
        }
    }
}
