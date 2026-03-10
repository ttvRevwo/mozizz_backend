using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace MozizzAdminKliens.Windows
{
    public partial class NewMovieWindow : Window
    {
        private readonly HttpClient _client;

        public NewMovieWindow(HttpClient client)
        {
            _client = client;
            InitializeComponent();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbxTitle.Text))
            {
                MessageBox.Show("A cím megadása kötelező!");
                return;
            }
            if (!int.TryParse(tbxDuration.Text, out int duration))
            {
                MessageBox.Show("A hossznak számnak kell lennie!");
                return;
            }
            if (!string.IsNullOrWhiteSpace(tbxReleaseDate.Text) && !DateTime.TryParse(tbxReleaseDate.Text, out _))
            {
                MessageBox.Show("A dátum formátuma helytelen! Használj ÉÉÉÉ-HH-NN formátumot.");
                return;
            }

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(tbxTitle.Text.Trim()), "title");
            form.Add(new StringContent(tbxGenre.Text.Trim()), "genre");
            form.Add(new StringContent(duration.ToString()), "duration");

            if (!string.IsNullOrWhiteSpace(tbxRating.Text))
                form.Add(new StringContent(tbxRating.Text.Trim()), "rating");
            if (!string.IsNullOrWhiteSpace(tbxDescription.Text))
                form.Add(new StringContent(tbxDescription.Text.Trim()), "description");
            if (!string.IsNullOrWhiteSpace(tbxImg.Text))
                form.Add(new StringContent(tbxImg.Text.Trim()), "img");
            if (!string.IsNullOrWhiteSpace(tbxReleaseDate.Text))
                form.Add(new StringContent(tbxReleaseDate.Text.Trim()), "release_date");

            var response = await _client.PostAsync("Movie/NewMovie", form);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Film sikeresen hozzáadva!");
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