using System.Windows;

namespace ApotekaApp
{
    public partial class DodajDobavljacaWindow : Window
    {
        public string NazivDobavljaca { get; private set; }

        public DodajDobavljacaWindow()
        {
            LoadSelectedTheme();
            InitializeComponent();
            txtNazivDobavljaca.Focus();
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
            if (string.IsNullOrWhiteSpace(txtNazivDobavljaca.Text))
            {
                MessageBox.Show("Unesite naziv dobavljača!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NazivDobavljaca = txtNazivDobavljaca.Text.Trim();
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