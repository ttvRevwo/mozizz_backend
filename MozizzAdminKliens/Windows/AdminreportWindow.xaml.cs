using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace MozizzAdminKliens.Windows
{
    public partial class AdminReportWindow : Window
    {
        private readonly HttpClient _client;
        private JsonElement _aktivVetitesek;
        private JsonElement _archivVetitesek;

        public AdminReportWindow(HttpClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDailyReport();
            await LoadTopMovies();
            await LoadOccupancy();
        }

        private async Task LoadDailyReport()
        {
            try
            {
                var response = await _client.GetAsync("Admin/DailyReport");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);

                    tbDatum.Text = data.TryGetProperty("Datum", out var d) ? "Dátum: " + d.GetString() :
                                   data.TryGetProperty("datum", out var d2) ? "Dátum: " + d2.GetString() : "";

                    tbBevetel.Text = data.TryGetProperty("MaiBevetel", out var b) ? "Bevétel: " + b.GetString() :
                                     data.TryGetProperty("maiBevetel", out var b2) ? "Bevétel: " + b2.GetString() : "";

                    tbJegyek.Text = data.TryGetProperty("EladottJegyek", out var j) ? "Jegyek: " + j.GetString() :
                                    data.TryGetProperty("eladottJegyek", out var j2) ? "Jegyek: " + j2.GetString() : "";

                    tbFoglalasok.Text = data.TryGetProperty("FoglalasokSzama", out var f) ? "Foglalások: " + f.GetInt32().ToString() :
                                        data.TryGetProperty("foglalasokSzama", out var f2) ? "Foglalások: " + f2.GetInt32().ToString() : "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Napi riport hiba: {ex.Message}");
            }
        }

        private async Task LoadTopMovies()
        {
            try
            {
                var response = await _client.GetAsync("Admin/TopMovies");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    var list = data.EnumerateArray().Select(m => new
                    {
                        filmCim = m.TryGetProperty("FilmCim", out var fc) ? fc.GetString() :
                                  m.TryGetProperty("filmCim", out var fc2) ? fc2.GetString() : "",
                        jegyekSzama = m.TryGetProperty("JegyekSzama", out var js) ? js.GetInt32().ToString() :
                                      m.TryGetProperty("jegyekSzama", out var js2) ? js2.GetInt32().ToString() : "",
                        bevetel = m.TryGetProperty("Bevetel", out var b) ? b.GetInt32() + " Ft" :
                                  m.TryGetProperty("bevetel", out var b2) ? b2.GetInt32() + " Ft" : "",
                    }).ToList();
                    dgTopMovies.ItemsSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Top filmek hiba: {ex.Message}");
            }
        }

        private async Task LoadOccupancy()
        {
            try
            {
                var response = await _client.GetAsync("Admin/ShowtimeOccupancy");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);

                    if (data.TryGetProperty("AktivVetitesek", out var aktiv))
                        _aktivVetitesek = aktiv;
                    else if (data.TryGetProperty("aktivVetitesek", out var aktiv2))
                        _aktivVetitesek = aktiv2;

                    if (data.TryGetProperty("ArchivVetitesek", out var archiv))
                        _archivVetitesek = archiv;
                    else if (data.TryGetProperty("archivVetitesek", out var archiv2))
                        _archivVetitesek = archiv2;

                    ShowOccupancy(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Telítettség hiba: {ex.Message}");
            }
        }

        private void ShowOccupancy(bool aktiv)
        {
            var source = aktiv ? _aktivVetitesek : _archivVetitesek;
            try
            {
                var list = source.EnumerateArray().Select(s => new
                {
                    film = s.TryGetProperty("Film", out var f) ? f.GetString() :
                           s.TryGetProperty("film", out var f2) ? f2.GetString() : "",
                    idopont = s.TryGetProperty("Idopont", out var i) ? i.GetString() :
                              s.TryGetProperty("idopont", out var i2) ? i2.GetString() : "",
                    eladottJegyek = s.TryGetProperty("EladottJegyek", out var e) ? e.GetInt32().ToString() :
                                    s.TryGetProperty("eladottJegyek", out var e2) ? e2.GetInt32().ToString() : "",
                    telitettseg = s.TryGetProperty("Telitettseg", out var t) ? t.GetString() :
                                  s.TryGetProperty("telitettseg", out var t2) ? t2.GetString() : "",
                }).ToList();
                dgOccupancy.ItemsSource = list;
            }
            catch { }
        }

        private void rbAktiv_Checked(object sender, RoutedEventArgs e)
        {
            if (dgOccupancy != null) ShowOccupancy(true);
        }

        private void rbArchiv_Checked(object sender, RoutedEventArgs e)
        {
            if (dgOccupancy != null) ShowOccupancy(false);
        }
    }
}