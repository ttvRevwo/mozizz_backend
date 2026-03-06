using MozizzAdminKliens.Windows;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;

namespace MozizzAdminKliens
{
    public partial class MainWindow : Window
    {
        public static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5083/api/")
        };
        public static string token = null;
        public static string loggedInName = null;

        //Teszttt
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuLogin_Click(object sender, RoutedEventArgs e)
        {
            if (token == null)
            {
                LoginWindow lw = new LoginWindow(client);
                lw.ShowDialog();

                if (token != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    itemLogin.Header = $"Logout ({loggedInName})";
                    itemUsers.IsEnabled = true;
                    tbWelcome.Text = $"Üdvözlünk, {loggedInName}! Válassz a menüből.";
                    tbWelcome.Foreground = System.Windows.Media.Brushes.Black;
                }
            }
            else
            {
                token = null;
                loggedInName = null;
                client.DefaultRequestHeaders.Authorization = null;
                itemLogin.Header = "Login";
                itemUsers.IsEnabled = false;
                tbWelcome.Text = "Kérjük, jelentkezz be az adminisztrációs felülethez.";
                tbWelcome.Foreground = System.Windows.Media.Brushes.Gray;
                MessageBox.Show("Kijelentkeztél!");
            }
        }

        private void itemUserList_Click(object sender, RoutedEventArgs e)
        {
            new UsersWindow(client).ShowDialog();
        }
    }
}
