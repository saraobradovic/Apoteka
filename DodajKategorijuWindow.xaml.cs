using System.Windows;
using System.Windows.Media;

namespace ApotekaApp
{
    public partial class DodajKategorijuWindow : Window
    {
        public string NazivKategorije { get; private set; }

        public DodajKategorijuWindow()
        {
            LoadSelectedTheme();
            InitializeComponent();
            txtNazivKategorije.Focus();
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

        private void BtnSačuvaj_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNazivKategorije.Text))
            {
                MessageBox.Show("Unesite naziv kategorije!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NazivKategorije = txtNazivKategorije.Text.Trim();
            this.DialogResult = true;
            this.Close();
        }

        private void BtnOtkaži_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}