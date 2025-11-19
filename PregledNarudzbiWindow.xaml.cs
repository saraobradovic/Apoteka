using System.Data;
using System.Windows;
using ApotekaApp.Data;

namespace ApotekaApp
{
    public partial class PregledNarudzbiWindow : Window
    {
        private DatabaseHelper db;

        public PregledNarudzbiWindow()
        {
            InitializeComponent();
            db = new DatabaseHelper();
            UcitajNarudzbe();
        }

        private void UcitajNarudzbe()
        {
            dataGridNarudzbe.ItemsSource = db.GetNarudzbe().DefaultView;
        }


        private void BtnOtkaziNarudzbu_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridNarudzbe.SelectedItem is DataRowView row)
            {
                int narudzbaId = Convert.ToInt32(row["NarudzbaID"]);
                var result = MessageBox.Show(
                    "Da li ste sigurni da želite otkazati odabranu narudžbu?",
                    "Potvrda",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.OtkaziNarudzbu(narudzbaId);
                        MessageBox.Show("Narudžba je otkazana i računi su obrisani.");
                        UcitajNarudzbe();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Greška pri otkazivanju narudžbe: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Odaberite narudžbu za otkazivanje!");
            }
        }

    }
}
