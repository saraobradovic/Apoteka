using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using ApotekaApp.Data;

namespace ApotekaApp
{
    public partial class AdminWindow : Window
    {
        private DatabaseHelper dbHelper;

        public AdminWindow()
        {
            InitializeComponent();
            LoadSelectedTheme();
            dbHelper = new DatabaseHelper();
            UcitajProizvode();
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

        private void UcitajProizvode()
        {
            try
            {
                dataGridProizvodi.ItemsSource = dbHelper.GetProizvodi().DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju proizvoda: " + ex.Message);
            }
        }

        private void BtnDodaj_Click(object sender, RoutedEventArgs e)
        {
            // Otvara prozor za dodavanje proizvoda
            DodajProizvodWindow window = new DodajProizvodWindow();
            if (window.ShowDialog() == true)
            {
                UcitajProizvode();
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
        private void BtnPregledNarudzbi_Click(object sender, RoutedEventArgs e)
        {
            PregledNarudzbiWindow w = new PregledNarudzbiWindow();
            w.ShowDialog();
        }
        private void BtnRacuni_Click(object sender, RoutedEventArgs e)
        {
            new RacuniWindow().ShowDialog();
        }
        private void txtPretraga_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = txtPretraga.Text.Trim().ToLower();

            DataTable dt = dbHelper.GetProizvodi(); // sve proizvode

            if (!string.IsNullOrEmpty(filter))
            {
                var filteredRows = dt.AsEnumerable()
                                     .Where(row => row["Proizvod"].ToString().ToLower().Contains(filter));

                if (filteredRows.Any())
                    dataGridProizvodi.ItemsSource = filteredRows.CopyToDataTable().DefaultView;
                else
                    dataGridProizvodi.ItemsSource = null; // ili napravi prazan DataTable da se vidi prazan grid
            }
            else
            {
                dataGridProizvodi.ItemsSource = dt.DefaultView;
            }
        }




    }
}
