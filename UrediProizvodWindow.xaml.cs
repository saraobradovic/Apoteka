using System;
using System.Data;
using System.Windows;
using ApotekaApp.Data;

namespace ApotekaApp
{
    public partial class UrediProizvodWindow : Window
    {
        private DatabaseHelper dbHelper;
        private int proizvodId;

        public UrediProizvodWindow(int proizvodId)
        {
            LoadSelectedTheme();
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            this.proizvodId = proizvodId;

            UcitajProizvod();
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
        private void UcitajProizvod()
        {
            try
            {
                DataTable dt = dbHelper.GetProizvodi();
                DataRow row = dt.Select($"ProizvodID = {proizvodId}")[0];

                txtNaziv.Text = row["Proizvod"].ToString();
               
                txtCijena.Text = row["Cijena"].ToString();
                txtKolicina.Text = row["Kolicina"].ToString();
               
                
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju proizvoda: " + ex.Message);
                this.Close();
            }
        }

        private void BtnSpremi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validacija cijene
                if (string.IsNullOrWhiteSpace(txtCijena.Text))
                {
                    MessageBox.Show("Cijena je obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCijena.Focus();
                    return;
                }

                if (!decimal.TryParse(txtCijena.Text, out decimal novaCijena))
                {
                    MessageBox.Show("Cijena mora biti validan broj!\nPrimjer: 12.50 ili 25", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCijena.SelectAll();
                    txtCijena.Focus();
                    return;
                }

                if (novaCijena < 0)
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

                if (!int.TryParse(txtKolicina.Text, out int novaKolicina))
                {
                    MessageBox.Show("Količina mora biti validan cijeli broj!\nPrimjer: 10 ili 25", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtKolicina.SelectAll();
                    txtKolicina.Focus();
                    return;
                }

                if (novaKolicina < 0)
                {
                    MessageBox.Show("Količina ne može biti negativna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtKolicina.SelectAll();
                    txtKolicina.Focus();
                    return;
                }

                // Ažuriranje proizvoda
                bool uspjeh = dbHelper.PromijeniCijenuIKolicinuProizvoda(proizvodId, novaCijena, novaKolicina);

                if (uspjeh)
                {
                    MessageBox.Show("Proizvod uspješno ažuriran!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Nije moguće ažurirati proizvod.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri spremanju: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
