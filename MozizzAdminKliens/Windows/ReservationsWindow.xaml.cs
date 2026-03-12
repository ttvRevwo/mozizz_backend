using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace MozizzAdminKliens.Windows
{
    public partial class ReservationsWindow : Window
    {
        private readonly HttpClient _client;
        private List<dynamic> _reservations = new();
        private string _lastUserId = "";

        public ReservationsWindow(HttpClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbStatus.Text = "Add meg a felhasználó ID-ját a betöltéshez.";
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxUserId.Text))
            {
                MessageBox.Show("Add meg a felhasználó ID-ját!");
                return;
            }
            _lastUserId = tbxUserId.Text.Trim();
            await LoadReservations(_lastUserId);
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_lastUserId))
            {
                MessageBox.Show("Először tölts be egy felhasználót!");
                return;
            }
            await LoadReservations(_lastUserId);
        }

        private async Task LoadReservations(string userId)
        {
            tbStatus.Text = "Betöltés...";
            try
            {
                var response = await _client.GetAsync($"Reservation/MyReservations/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    _reservations = data.EnumerateArray().Select(r => (dynamic)new
                    {
                        reservationId = r.TryGetProperty("reservationId", out var id) ? id.GetInt32() : 0,
                        filmCim = r.TryGetProperty("filmCim", out var fc) ? fc.GetString() : "",
                        datum = r.TryGetProperty("datum", out var d) ? d.GetString() : "",
                        idopont = r.TryGetProperty("idopont", out var i) ? i.GetString() : "",
                        szekek = r.TryGetProperty("szekek", out var sz) ? sz.GetString() : "",
                        statusz = r.TryGetProperty("statusz", out var st) ? st.GetString() : "",
                        vegosszeg = r.TryGetProperty("vegosszeg", out var v) ? v.GetInt32() + " Ft" : "",
                    }).ToList();
                    dgReservations.ItemsSource = _reservations;
                    tbStatus.Text = $"{_reservations.Count} foglalás betöltve.";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    dgReservations.ItemsSource = null;
                    tbStatus.Text = "Nincsenek foglalások ehhez a felhasználóhoz.";
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

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (dgReservations.SelectedItem == null)
            {
                MessageBox.Show("Válassz ki egy foglalást!");
                return;
            }

            dynamic selected = dgReservations.SelectedItem;
            int id = selected.reservationId;

            var confirm = MessageBox.Show($"Biztosan lemondod a(z) {id}. foglalást?", "Megerősítés",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            var response = await _client.DeleteAsync($"Reservation/AdminCancel/{id}");
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Foglalás sikeresen lemondva!");
                await LoadReservations(_lastUserId);
            }
            else
            {
                string err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Hiba: {err}");
            }
        }
    }
}
