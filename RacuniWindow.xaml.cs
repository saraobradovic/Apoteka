using System.Windows;
using ApotekaApp.Data;

namespace ApotekaApp
{
    public partial class RacuniWindow : Window
    {
        private DatabaseHelper db;

        public RacuniWindow()
        {
            LoadSelectedTheme();
            InitializeComponent();
            db = new DatabaseHelper();
            dataGridRacuni.ItemsSource = db.GetRacuni().DefaultView;
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
    }
}
