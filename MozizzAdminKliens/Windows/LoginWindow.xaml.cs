using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace MozizzAdminKliens.Windows
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _client;

        public LoginWindow(HttpClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            try
            {
                var body = JsonSerializer.Serialize(new
                {
                    email = tbxEmail.Text,
                    password = pwxPassword.Password
                });

                var response = await _client.PostAsync("Auth/Login",
                    new StringContent(body, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<JsonElement>(json);

                    MainWindow.token = data.GetProperty("token").GetString();
                    MainWindow.loggedInName = data.GetProperty("name").GetString();
                    string role = data.GetProperty("role").GetString();

                    if (role != "Admin")
                    {
                        MainWindow.token = null;
                        MainWindow.loggedInName = null;
                        MessageBox.Show("Hozzáférés megtagadva! Csak adminok léphetnek be.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                        Close();
                        return;
                    }

                    MessageBox.Show("Sikeres bejelentkezés!");
                }
                else
                {
                    string err = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Sikertelen bejelentkezés!\n{err}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kapcsolódási hiba: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
            Close();
        }
    }
}
