using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace MozizzAdminKliens.Windows
{
    public partial class TicketValidateWindow : Window
    {
        private readonly HttpClient _client;

        public TicketValidateWindow(HttpClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private async void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            string code = tbxTicketCode.Text.Trim();
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Add meg a jegy kódját!");
                return;
            }

            var response = await _client.GetAsync($"Ticket/ValidateTicket/{code}");
            string json = await response.Content.ReadAsStringAsync();

            borderResult.Visibility = Visibility.Visible;

            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                if (response.IsSuccessStatusCode)
                {
                    string film = data.TryGetProperty("film", out var f) ? f.GetString() : "";
                    string idopont = data.TryGetProperty("idopont", out var i) ? i.GetString() : "";
                    string szekek = data.TryGetProperty("szekek", out var s) ? s.GetString() : "";

                    tbResultTitle.Text = "✔ Érvényes jegy!";
                    tbResultTitle.Foreground = Brushes.Green;
                    tbResultDetails.Text = $"Film: {film}\nVetítés: {idopont}\nSzékek: {szekek}";
                    borderResult.BorderBrush = Brushes.Green;
                    borderResult.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                }
                else
                {
                    string uzenet = data.TryGetProperty("uzenet", out var u) ? u.GetString() : json;
                    tbResultTitle.Text = "✘ Érvénytelen jegy!";
                    tbResultTitle.Foreground = Brushes.Red;
                    tbResultDetails.Text = uzenet;
                    borderResult.BorderBrush = Brushes.Red;
                    borderResult.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                }
            }
            catch
            {
                tbResultTitle.Text = "Hiba a válasz feldolgozása során.";
                tbResultTitle.Foreground = Brushes.OrangeRed;
                tbResultDetails.Text = json;
                borderResult.BorderBrush = Brushes.OrangeRed;
                borderResult.Background = Brushes.White;
            }
        }
    }
}
