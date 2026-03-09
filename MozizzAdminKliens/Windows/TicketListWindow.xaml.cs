using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace MozizzAdminKliens.Windows
{
    public partial class TicketListWindow : Window
    {
        private readonly HttpClient _client;

        public TicketListWindow(HttpClient client)
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
            await LoadForUser(tbxUserId.Text.Trim());
        }

        private async Task LoadForUser(string userId)
        {
            tbStatus.Text = "Betöltés...";
            try
            {
                var response = await _client.GetAsync($"Ticket/MyTickets/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    BindTickets(data);
                }
                else
                {
                    tbStatus.Text = "Nem sikerült betölteni.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}");
            }
        }

        private void BindTickets(JsonElement data)
        {
            var list = data.EnumerateArray().Select(t => new
            {
                ticketCode = t.TryGetProperty("ticketCode", out var tc) ? tc.GetString() : "",
                issuedDate = t.TryGetProperty("issuedDate", out var id) ? id.GetString() : "",
                status = t.TryGetProperty("status", out var st) ? st.GetString() : "",
            }).ToList();

            dgTickets.ItemsSource = list;
            tbStatus.Text = $"{list.Count} jegy betöltve.";
        }

        private void btnCopyCode_Click(object sender, RoutedEventArgs e)
        {
            if (dgTickets.SelectedItem == null)
            {
                MessageBox.Show("Válassz ki egy jegyet!");
                return;
            }
            dynamic selected = dgTickets.SelectedItem;
            string code = selected.ticketCode;
            Clipboard.SetText(code);
            tbStatus.Text = $"Kód másolva: {code}";
        }
    }
}