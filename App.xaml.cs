using System.Configuration;
using System.Data;
using System.Windows;

namespace ApotekaApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string SelectedTheme { get; set; } = "Themes/Tema1.xaml";
        protected override void OnStartup(StartupEventArgs e)
         {
            LoadTheme(SelectedTheme);
            base.OnStartup(e);
        }
        
        public static void LoadTheme(string themePath)
        {
            try
            {
                Current.Resources.MergedDictionaries.Clear();
                var themeUri = new Uri(themePath, UriKind.Relative);
                var themeDict = new ResourceDictionary();
                themeDict.Source = themeUri;
                Current.Resources.MergedDictionaries.Add(themeDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju teme: {ex.Message}");
            }
        }

    }
}

