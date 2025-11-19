using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;
using ApotekaApp.Data;
using System.Windows.Media;


namespace ApotekaApp
{
    public partial class DodajProizvodWindow : Window
    {
        private DatabaseHelper dbHelper;
        private DataTable dtKategorije;
        private DataTable dtDobavljaci;

        public DodajProizvodWindow()
        {
            LoadSelectedTheme();
            InitializeComponent();
            dbHelper = new DatabaseHelper();

            UcitajKategorije();
            UcitajDobavljace();
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

        private void UcitajKategorije()
        {
            dtKategorije = dbHelper.GetKategorije(); // vraća DataTable
            comboKategorija.ItemsSource = dtKategorije.DefaultView;
            comboKategorija.DisplayMemberPath = "Naziv";
            comboKategorija.SelectedValuePath = "KategorijaID";
        }

        private void UcitajDobavljace()
        {
            dtDobavljaci = dbHelper.GetDobavljace();
            comboDobavljac.ItemsSource = dtDobavljaci.DefaultView;
            comboDobavljac.DisplayMemberPath = "Naziv";
            comboDobavljac.SelectedValuePath = "DobavljacID";
        }

        private void BtnDodajKategoriju_Click(object sender, RoutedEventArgs e)
        {
            var window = new DodajKategorijuWindow();
            window.Owner = this;
            window.Background = Brushes.Honeydew; // Svijetlo zelena boja

            if (window.ShowDialog() == true && !string.IsNullOrWhiteSpace(window.NazivKategorije))
            {
                string naziv = window.NazivKategorije;
                int id = dbHelper.DodajKategoriju(naziv);
                DataRow red = dtKategorije.NewRow();
                red["KategorijaID"] = id;
                red["Naziv"] = naziv;
                dtKategorije.Rows.Add(red);
                comboKategorija.SelectedValue = id;
            }
        }

        private void BtnDodajDobavljaca_Click(object sender, RoutedEventArgs e)
        {
            var window = new DodajDobavljacaWindow();
            window.Owner = this; // Postavi ovaj prozor kao vlasnika
            window.Background = new SolidColorBrush(Color.FromRgb(240, 248, 255)); // Svijetlo plava boja

            if (window.ShowDialog() == true && !string.IsNullOrWhiteSpace(window.NazivDobavljaca))
            {
                string naziv = window.NazivDobavljaca;
                int id = dbHelper.DodajDobavljaca(naziv);
                DataRow red = dtDobavljaci.NewRow();
                red["DobavljacID"] = id;
                red["Naziv"] = naziv;
                dtDobavljaci.Rows.Add(red);
                comboDobavljac.SelectedValue = id;
            }
        }

        private void BtnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validacija šifre
                if (string.IsNullOrWhiteSpace(txtSifra.Text))
                {
                    MessageBox.Show("Šifra je obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtSifra.Focus();
                    return;
                }

                // Validacija naziva
                if (string.IsNullOrWhiteSpace(txtNaziv.Text))
                {
                    MessageBox.Show("Naziv je obavezan!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtNaziv.Focus();
                    return;
                }

                // Validacija cijene
                if (string.IsNullOrWhiteSpace(txtCijena.Text))
                {
                    MessageBox.Show("Cijena je obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCijena.Focus();
                    return;
                }

                if (!decimal.TryParse(txtCijena.Text, out decimal cijena))
                {
                    MessageBox.Show("Cijena mora biti validan broj!\nPrimjer: 12.50 ili 25", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCijena.SelectAll();
                    txtCijena.Focus();
                    return;
                }

                if (cijena < 0)
                {
                    MessageBox.Show("Cijena ne može biti negativna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCijena.SelectAll();
                    txtCijena.Focus();
                    return;
                }

                // Validacija količine
                if (string.IsNullOrWhiteSpace(txtKolicina.Text))
                {
                    MessageBox.Show("Količina je obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtKolicina.Focus();
                    return;
                }

                if (!int.TryParse(txtKolicina.Text, out int kolicina))
                {
                    MessageBox.Show("Količina mora biti validan cijeli broj!\nPrimjer: 10 ili 25", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtKolicina.SelectAll();
                    txtKolicina.Focus();
                    return;
                }

                if (kolicina < 0)
                {
                    MessageBox.Show("Količina ne može biti negativna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtKolicina.SelectAll();
                    txtKolicina.Focus();
                    return;
                }

                // Validacija kategorije
                if (comboKategorija.SelectedValue == null)
                {
                    MessageBox.Show("Odaberite kategoriju!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    comboKategorija.Focus();
                    return;
                }

                if (!int.TryParse(comboKategorija.SelectedValue.ToString(), out int kategorijaID))
                {
                    MessageBox.Show("Nevalidna kategorija!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    comboKategorija.Focus();
                    return;
                }

                // Validacija dobavljača
                if (comboDobavljac.SelectedValue == null)
                {
                    MessageBox.Show("Odaberite dobavljača!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    comboDobavljac.Focus();
                    return;
                }

                if (!int.TryParse(comboDobavljac.SelectedValue.ToString(), out int dobavljacID))
                {
                    MessageBox.Show("Nevalidan dobavljač!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    comboDobavljac.Focus();
                    return;
                }

                // Dodavanje proizvoda
                dbHelper.DodajProizvod(txtSifra.Text.Trim(), txtNaziv.Text.Trim(), cijena, kolicina, kategorijaID, dobavljacID);

                MessageBox.Show("Proizvod je uspješno dodat!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri dodavanju proizvoda: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
