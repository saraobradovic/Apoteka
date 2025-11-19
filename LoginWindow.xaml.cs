using System.Windows;
using ApotekaApp.Data;

namespace ApotekaApp
{
    public partial class LoginWindow : Window
    {
        private DatabaseHelper dbHelper;

        public LoginWindow()
        {
            LoadSelectedTheme();
            InitializeComponent();
         
            dbHelper = new DatabaseHelper();
        }
        private void LoadSelectedTheme()
        {
            try
            {
                if (!string.IsNullOrEmpty(StartWindow.SelectedTheme))
                {
                    Application.Current.Resources.MergedDictionaries.Clear();

                    var themeUri = new Uri(StartWindow.SelectedTheme, UriKind.Relative);
                    var themeDict = new ResourceDictionary();
                    themeDict.Source = themeUri;

                    Application.Current.Resources.MergedDictionaries.Add(themeDict);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju teme: {ex.Message}");
            }
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (dbHelper.LoginAdministrator(username, password))
            {
                // Ako su podaci tačni, otvara se AdminWindow

                AdminWindow admin = new AdminWindow();
                admin.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Neispravno korisničko ime ili lozinka!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
