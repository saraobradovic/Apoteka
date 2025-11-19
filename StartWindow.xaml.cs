using System;
using System.Windows;

namespace ApotekaApp
{
    public partial class StartWindow : Window
    {
        public static string SelectedTheme { get; private set; } = "Themes/Tema1.xaml";

        public StartWindow()
        {
            // Učitaj default temu
            LoadTheme(SelectedTheme);
            InitializeComponent();
        }

        private void LoadTheme(string themePath)
        {
            try
            {
                // Obriši postojeće resurse
                Application.Current.Resources.MergedDictionaries.Clear();

                // Učitaj novu temu
                var themeUri = new Uri(themePath, UriKind.Relative);
                var themeDict = new ResourceDictionary();
                themeDict.Source = themeUri;

                Application.Current.Resources.MergedDictionaries.Add(themeDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju teme: {ex.Message}");
            }
        }

        private void BtnTema1_Click(object sender, RoutedEventArgs e)
        {
            SelectedTheme = "Themes/Tema1.xaml";
            LoadTheme(SelectedTheme);
        }

        private void BtnTema2_Click(object sender, RoutedEventArgs e)
        {
            SelectedTheme = "Themes/Tema2.xaml";
            LoadTheme(SelectedTheme);
        }

        private void BtnTema3_Click(object sender, RoutedEventArgs e)
        {
            SelectedTheme = "Themes/Tema3.xaml";
            LoadTheme(SelectedTheme);
        }

        private void BtnKorisnik_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new KorisnikWindow();
            mainWindow.Show();
            this.Close();
        }

        private void BtnPrijava_Click(object sender, RoutedEventArgs e)
        {
            // Otvori prozor za prijavu administratora
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}