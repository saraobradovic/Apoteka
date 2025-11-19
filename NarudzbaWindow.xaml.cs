using System;
using System.Collections.Generic;
using System.Windows;
using ApotekaApp.Data;

namespace ApotekaApp
{
    public partial class NarudzbaWindow : Window
    {
        private DatabaseHelper dbHelper;
        private List<KorisnikWindow.KorpaItem> korpa;
        private int korisnikId;

        public NarudzbaWindow(List<KorisnikWindow.KorpaItem> korpaItems, int korisnikId)
        {
            // UČITAJ TEMU PRIJE InitializeComponent()
            LoadSelectedTheme();
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            korpa = korpaItems;
            this.korisnikId = korisnikId;
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

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            string ime = txtIme.Text.Trim();
            string prezime = txtPrezime.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefon = txtTelefon.Text.Trim();

            if (string.IsNullOrWhiteSpace(ime) ||
                string.IsNullOrWhiteSpace(prezime) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(telefon))
            {
                MessageBox.Show("Sva polja moraju biti popunjena.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 1️⃣ Dodaj kupca
                int kupacId = dbHelper.DodajKupca(ime, prezime, email, telefon);

                // 2️⃣ Kreiraj narudžbu
                int narudzbaId = dbHelper.KreirajNarudzbu(kupacId);

                // 3️⃣ Dodaj stavke narudžbe
                foreach (var item in korpa)
                    dbHelper.DodajStavkuNarudzbe(narudzbaId, item.ProizvodID, item.Kolicina);

                // 4️⃣ Kreiraj račun
                decimal ukupno = 0;
                foreach (var item in korpa)
                    ukupno += item.Ukupno;

                dbHelper.KreirajRacun(narudzbaId, ukupno);

                MessageBox.Show("Kupovina je uspješno završena!", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška prilikom kupovine: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}