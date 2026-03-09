using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace MozizzAdminKliens.Windows
{
    public partial class UsersWindow : Window
    {
        private readonly HttpClient _client;
        private List<dynamic> _allUsers = new();

        public UsersWindow(HttpClient client)
        {
            _client = client;
            InitializeComponent();
        }

        //Teszt
        private async void Window_Loaded(object sender, RoutedEventArgs e) => await LoadUsers();

        private async Task LoadUsers()
        {
            tbStatus.Text = "Betöltés...";
            try
            {
                var response = await _client.GetAsync("User/GetAllUsers");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    // MySQL 0000-00-00 jellegű érvénytelen dátumok cseréje null-ra
                    json = System.Text.RegularExpressions.Regex.Replace(
                        json,
                        @"""(0000-00-00[^""]*|00010101[^""]*)""",
                        "null");

                    var data = JsonSerializer.Deserialize<JsonElement>(json);
                    _allUsers = data.EnumerateArray().Select(u => (dynamic)new
                    {
                        userId = u.TryGetProperty("userId", out var id) ? id.GetInt32() :
                                 u.TryGetProperty("UserId", out var Id) ? Id.GetInt32() : 0,
                        name = u.TryGetProperty("name", out var n) ? n.GetString() :
                                 u.TryGetProperty("Name", out var N) ? N.GetString() : "",
                        email = u.TryGetProperty("email", out var em) ? em.GetString() :
                                 u.TryGetProperty("Email", out var Em) ? Em.GetString() : "",
                        phone = u.TryGetProperty("phone", out var ph) ? ph.GetString() :
                                 u.TryGetProperty("Phone", out var Ph) ? Ph.GetString() : "",
                        role = u.TryGetProperty("roleId", out var r) ? (r.GetInt32() == 1 ? "Admin" : "Customer") :
                                 u.TryGetProperty("RoleId", out var R) ? (R.GetInt32() == 1 ? "Admin" : "Customer") : "",
                    }).ToList();
                    ApplyFilter();
                    tbStatus.Text = $"{_allUsers.Count} felhasználó betöltve.";
                }
                else
                {
                    string err = await response.Content.ReadAsStringAsync();
                    tbStatus.Text = "Hiba a betöltés során.";
                    MessageBox.Show($"Státusz: {response.StatusCode}\n{err}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            string f = tbxSearch.Text.Trim().ToLower();
            var filtered = string.IsNullOrEmpty(f)
                ? _allUsers
                : _allUsers.Where(u =>
                    ((string)u.name).ToLower().Contains(f) ||
                    ((string)u.email).ToLower().Contains(f)).ToList();
            dgUsers.ItemsSource = filtered;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e) => ApplyFilter();
        private async void btnRefresh_Click(object sender, RoutedEventArgs e) => await LoadUsers();

        private async void btnSetAdmin_Click(object sender, RoutedEventArgs e)
        {
            await ChangeRole(1, "Admin");
        }

        private async void btnSetCustomer_Click(object sender, RoutedEventArgs e)
        {
            await ChangeRole(2, "Customer");
        }

        private async Task ChangeRole(int roleId, string roleName)
        {
            if (dgUsers.SelectedItem == null)
            {
                MessageBox.Show("Válassz ki egy felhasználót!");
                return;
            }

            dynamic selected = dgUsers.SelectedItem;
            int id = selected.userId;
            string name = selected.name;

            var confirm = MessageBox.Show($"Biztosan {roleName} szerepkörre állítod \"{name}\" felhasználót?",
                "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            var getResponse = await _client.GetAsync($"User/UserById/{id}");
            if (!getResponse.IsSuccessStatusCode)
            {
                MessageBox.Show("Nem sikerült lekérni a felhasználó adatait.");
                return;
            }

            string getJson = await getResponse.Content.ReadAsStringAsync();

            // MySQL érvénytelen dátumok kezelése
            getJson = System.Text.RegularExpressions.Regex.Replace(
                getJson,
                @"""(0000-00-00[^""]*|00010101[^""]*)""",
                "null");

            var userData = JsonSerializer.Deserialize<JsonElement>(getJson);

            // createdAt stringként kezelve, hogy elkerüljük a MySQL timestamp konverziós hibát
            string? createdAt = null;
            if (userData.TryGetProperty("createdAt", out var ca))
                createdAt = ca.ValueKind == JsonValueKind.String ? ca.GetString() : ca.GetRawText();
            else if (userData.TryGetProperty("created_at", out var ca2))
                createdAt = ca2.ValueKind == JsonValueKind.String ? ca2.GetString() : ca2.GetRawText();
            else if (userData.TryGetProperty("CreatedAt", out var ca3))
                createdAt = ca3.ValueKind == JsonValueKind.String ? ca3.GetString() : ca3.GetRawText();

            var bodyObj = new
            {
                UserId = id,
                Name = userData.TryGetProperty("name", out var n) ? n.GetString() :
                       userData.TryGetProperty("Name", out var N2) ? N2.GetString() : "",
                Email = userData.TryGetProperty("email", out var em) ? em.GetString() :
                        userData.TryGetProperty("Email", out var Em2) ? Em2.GetString() : "",
                Phone = userData.TryGetProperty("phone", out var ph) ? ph.GetString() :
                        userData.TryGetProperty("Phone", out var Ph2) ? Ph2.GetString() : "",
                   PasswordHash = userData.TryGetProperty("passwordHash", out var pw) ? pw.GetString() :
                               userData.TryGetProperty("PasswordHash", out var Pw2) ? Pw2.GetString() : "",
                RoleId = roleId,
                Role = new { RoleId = roleId, RoleName = roleName },
                CreatedAt = createdAt
            };

            var body = JsonSerializer.Serialize(bodyObj);

            var response = await _client.PutAsync("User/ModifyUser",
                new StringContent(body, System.Text.Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show($"Szerepkör sikeresen módosítva: {roleName}!");
                await LoadUsers();
            }
            else
            {
                string err = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Hiba: {err}");
            }
        }
    }
}