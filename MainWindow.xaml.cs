using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ApotekaApp.Data;
namespace ApotekaApp

{
    public partial class MainWindow : Window
    {
        private DatabaseHelper dbHelper;
        private string Uloga;

        public MainWindow(string uloga)
        {
           
            InitializeComponent();
            LoadSelectedTheme();
            dbHelper = new DatabaseHelper();
            Uloga = uloga;

            UcitajProizvode();

            if (Uloga == "Korisnik")
            {
                btnDodaj.Visibility = Visibility.Collapsed;
                btnUredi.Visibility = Visibility.Collapsed;
                btnObrisi.Visibility = Visibility.Collapsed; 
            }

        }
        private void LoadSelectedTheme()
        {
            try
            {
                // Ako nema izabrane teme, koristite default
                string themeToLoad = !string.IsNullOrEmpty(StartWindow.SelectedTheme)
                    ? StartWindow.SelectedTheme
                    : "Themes/Tema1.xaml"; // ili "Tema1.xaml" ako su u root folderu

                Application.Current.Resources.MergedDictionaries.Clear();

                var themeUri = new Uri(themeToLoad, UriKind.Relative);
                var themeDict = new ResourceDictionary();
                themeDict.Source = themeUri;

                Application.Current.Resources.MergedDictionaries.Add(themeDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju teme: {ex.Message}");
                // Fallback na hardkodirane boje
                ApplyFallbackColors();
            }
        }

        private void ApplyFallbackColors()
        {
            // Fallback boje ako tema ne može da se učita
            var fallbackColors = new ResourceDictionary();
            fallbackColors.Add("PrimaryBrush", new SolidColorBrush(Color.FromRgb(74, 124, 89))); // #4A7C59
            fallbackColors.Add("SecondaryBrush", new SolidColorBrush(Color.FromRgb(44, 85, 48))); // #2C5530
            fallbackColors.Add("AccentBrush", new SolidColorBrush(Color.FromRgb(143, 185, 150))); // #8FB996
            fallbackColors.Add("BackgroundBrush", new SolidColorBrush(Color.FromRgb(248, 245, 240))); // #F8F5F0
            fallbackColors.Add("TextBrush", new SolidColorBrush(Color.FromRgb(44, 85, 48))); // #2C5530

            Application.Current.Resources.MergedDictionaries.Add(fallbackColors);
        }
        private void UcitajProizvode()
        {
            dataGridProizvodi.ItemsSource = dbHelper.GetProizvodi().DefaultView;
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            DodajProizvodWindow window = new DodajProizvodWindow();
            if (window.ShowDialog() == true)
            {
                UcitajProizvode();
            }
        }
        

        private void BtnUredi_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridProizvodi.SelectedItem is System.Data.DataRowView row)
            {
                int proizvodId = Convert.ToInt32(row["ProizvodID"]);
                UrediProizvodWindow window = new UrediProizvodWindow(proizvodId);
                if (window.ShowDialog() == true)
                {
                    UcitajProizvode();
                }
            }
            else
            {
                MessageBox.Show("Odaberite proizvod koji želite urediti!");
            }
        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridProizvodi.SelectedItem is System.Data.DataRowView row)
            {
                int proizvodId = Convert.ToInt32(row["ProizvodID"]);
                try
                {
                    dbHelper.ObrisiProizvod(proizvodId);
                    UcitajProizvode();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greška pri brisanju proizvoda: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Odaberite proizvod koji želite obrisati!");
            }
        }

        private void BtnKreirajNarudzbu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Funkcionalnost kreiranja narudžbe još nije implementirana.");
        }

    }
}