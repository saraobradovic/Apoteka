using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ApotekaApp.Data;

namespace ApotekaApp
{
    public partial class KorisnikWindow : Window
    {
        private DatabaseHelper dbHelper;
        private List<KorpaItem> korpa = new List<KorpaItem>();
        private DataTable trenutniProizvodi; // DODANO: Čuva trenutno prikazane proizvode

        private List<string> slike = new List<string>
        {
            "Images/lavanda.png",
            "Images/list.png",
            "Images/ljekovito.png"
        };

        private int trenutnaSlika = 0;
        private DispatcherTimer slideshowTimer;

        public KorisnikWindow()
        {
            LoadSelectedTheme();
            InitializeComponent();
            dbHelper = new DatabaseHelper();

            UcitajKategorije();
            PrikaziPorukuZaOdabirKategorije();
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

        private void PrikaziPorukuZaOdabirKategorije()
        {
            wrapPanelProizvodi.Children.Clear();

            var tekst = new TextBlock
            {
                Text = "Odaberite kategoriju za prikaz proizvoda",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.Gray
            };

            wrapPanelProizvodi.Children.Add(tekst);
        }

        #region Korpa klasa
        public class KorpaItem
        {
            public int ProizvodID { get; set; }
            public string Naziv { get; set; }
            public decimal Cijena { get; set; }
            public int Kolicina { get; set; }
            public decimal Ukupno => Cijena * Kolicina;

            public override string ToString() => $"{Naziv} x {Kolicina} = {Ukupno} KM";
        }
        #endregion

        #region Kategorije
        private void UcitajKategorije()
        {
            DataTable dt = dbHelper.GetKategorije();
            listBoxKategorije.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                var listItem = new ListBoxItem
                {
                    Content = row["Naziv"].ToString(),
                    Tag = row["KategorijaID"],
                    Foreground = (System.Windows.Media.Brush)FindResource("TextBrush")
                };
                listBoxKategorije.Items.Add(listItem);
            }
        }

        private void listBoxKategorije_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Očisti pretragu kada se promijeni kategorija
            txtPretraga.Text = string.Empty;
            btnOcistiPretragu.Visibility = Visibility.Collapsed;

            if (listBoxKategorije.SelectedItem is ListBoxItem selected)
            {
                int katId = Convert.ToInt32(selected.Tag);
                trenutniProizvodi = dbHelper.GetProizvodiByKategorija(katId);

                if (trenutniProizvodi.Rows.Count == 0)
                {
                    PrikaziPorukuNemaProizvoda();
                }
                else
                {
                    PrikaziProizvode(trenutniProizvodi);
                }
            }
        }

        private void PrikaziPorukuNemaProizvoda()
        {
            wrapPanelProizvodi.Children.Clear();

            var porukaBorder = new Border
            {
                Background = (System.Windows.Media.Brush)FindResource("BackgroundBrush"),
                CornerRadius = new System.Windows.CornerRadius(8),
                Padding = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10)
            };

            var porukaStack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var ikona = new TextBlock
            {
                Text = "😔",
                FontSize = 48,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var naslov = new TextBlock
            {
                Text = "Nema proizvoda",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = (System.Windows.Media.Brush)FindResource("TextBrush"),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var opis = new TextBlock
            {
                Text = "Ova kategorija trenutno nema dostupnih proizvoda.",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = (System.Windows.Media.Brush)FindResource("TextBrush"),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 300
            };

            porukaStack.Children.Add(ikona);
            porukaStack.Children.Add(naslov);
            porukaStack.Children.Add(opis);
            porukaBorder.Child = porukaStack;

            wrapPanelProizvodi.Children.Add(porukaBorder);
        }
        #endregion

        #region Pretraga Proizvoda
        private void TxtPretraga_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPretraga.Text))
            {
                btnOcistiPretragu.Visibility = Visibility.Collapsed;
                // Ako nema teksta, prikaži sve proizvode trenutne kategorije
                if (trenutniProizvodi != null)
                {
                    PrikaziProizvode(trenutniProizvodi);
                }
                else
                {
                    PrikaziPorukuZaOdabirKategorije();
                }
            }
            else
            {
                btnOcistiPretragu.Visibility = Visibility.Visible;
                PretraziProizvode(txtPretraga.Text.Trim());
            }
        }

        private void BtnOcistiPretragu_Click(object sender, RoutedEventArgs e)
        {
            txtPretraga.Text = string.Empty;
            btnOcistiPretragu.Visibility = Visibility.Collapsed;
        }

        private void PretraziProizvode(string searchText)
        {
            if (trenutniProizvodi == null || trenutniProizvodi.Rows.Count == 0)
                return;

            try
            {
                var filtriraniRedovi = trenutniProizvodi.AsEnumerable()
                    .Where(row => row.Field<string>("Naziv").ToLower().Contains(searchText.ToLower()));

                if (filtriraniRedovi.Any())
                {
                    DataTable filtriraniProizvodi = filtriraniRedovi.CopyToDataTable();
                    PrikaziProizvode(filtriraniProizvodi);
                }
                else
                {
                    PrikaziPorukuNemaRezultataPretrage(searchText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri pretrazi: {ex.Message}");
            }
        }

        private void PrikaziPorukuNemaRezultataPretrage(string searchText)
        {
            wrapPanelProizvodi.Children.Clear();

            var porukaBorder = new Border
            {
                Background = (System.Windows.Media.Brush)FindResource("BackgroundBrush"),
                CornerRadius = new System.Windows.CornerRadius(8),
                Padding = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10)
            };

            var porukaStack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var ikona = new TextBlock
            {
                Text = "🔍",
                FontSize = 48,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var naslov = new TextBlock
            {
                Text = "Nema rezultata",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = (System.Windows.Media.Brush)FindResource("TextBrush"),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var opis = new TextBlock
            {
                Text = $"Nije pronađen nijedan proizvod za pojam: '{searchText}'",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = (System.Windows.Media.Brush)FindResource("TextBrush"),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 300
            };

            var savjet = new TextBlock
            {
                Text = "Pokušajte s drugačijim pojmom ili očistite pretragu",
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.Gray,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 10, 0, 0)
            };

            porukaStack.Children.Add(ikona);
            porukaStack.Children.Add(naslov);
            porukaStack.Children.Add(opis);
            porukaStack.Children.Add(savjet);
            porukaBorder.Child = porukaStack;

            wrapPanelProizvodi.Children.Add(porukaBorder);
        }
        #endregion

        #region Prikaz Proizvoda
        private void PrikaziSveProizvode()
        {
            trenutniProizvodi = dbHelper.GetProizvodi();
            if (trenutniProizvodi.Rows.Count == 0)
            {
                PrikaziPorukuNemaProizvoda();
            }
            else
            {
                PrikaziProizvode(trenutniProizvodi);
            }
        }

        private void PrikaziProizvode(DataTable dt)
        {
            wrapPanelProizvodi.Children.Clear();

            foreach (DataRow row in dt.Rows)
            {
                var border = new Border
                {
                    Style = (Style)FindResource("ProizvodCardStyle")
                };

                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                var stack = new StackPanel();
                Grid.SetRow(stack, 0);

                var naziv = new TextBlock
                {
                    Text = row["Naziv"].ToString(),
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 5, 0, 0),
                    Foreground = (System.Windows.Media.Brush)FindResource("TextBrush")
                };

                var cijena = new TextBlock
                {
                    Text = $"{Convert.ToDecimal(row["Cijena"]):F2} KM",
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0),
                    Foreground = (System.Windows.Media.Brush)FindResource("PrimaryBrush"),
                    FontWeight = FontWeights.Bold
                };

                // Prazan prostor koji gura dugme na dno
                var spacer = new Border
                {
                    Height = 20
                };
                Grid.SetRow(spacer, 1);

                var btn = new Button
                {
                    Content = "Dodaj u korpu",
                    Style = (Style)FindResource("KorisnikButton"),
                    Background = (System.Windows.Media.Brush)FindResource("SecondaryBrush"),
                    FontSize = 11,
                    Margin = new Thickness(0, 10, 0, 0),
                    Tag = row,
                    Height = 30
                };
                Grid.SetRow(btn, 2);
                btn.Click += BtnDodajUKorpu_Click;

                stack.Children.Add(naziv);
                stack.Children.Add(cijena);

                grid.Children.Add(stack);
                grid.Children.Add(spacer);
                grid.Children.Add(btn);

                border.Child = grid;
                wrapPanelProizvodi.Children.Add(border);
            }
        }
        #endregion

        #region Korpa Funkcionalnosti
        private void BtnDodajUKorpu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DataRow row)
            {
                int proizvodId = Convert.ToInt32(row["ProizvodID"]);
                string naziv = row["Naziv"].ToString();
                decimal cijena = Convert.ToDecimal(row["Cijena"]);

                var item = korpa.FirstOrDefault(x => x.ProizvodID == proizvodId);
                if (item != null)
                    item.Kolicina++;
                else
                    korpa.Add(new KorpaItem
                    {
                        ProizvodID = proizvodId,
                        Naziv = naziv,
                        Cijena = cijena,
                        Kolicina = 1
                    });

                OsveziKorpu();
            }
        }

        private void OsveziKorpu()
        {
            listBoxKorpa.Items.Clear();

            foreach (var item in korpa)
            {
                var panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center
                };

                var txt = new TextBlock
                {
                    Text = $"{item.Naziv} x {item.Kolicina} = {item.Ukupno:F2} KM",
                    Width = 150,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = (System.Windows.Media.Brush)FindResource("TextBrush")
                };

                var btnPlus = new Button
                {
                    Content = "+",
                    Width = 25,
                    Height = 25,
                    Tag = item,
                    Margin = new Thickness(5, 0, 0, 0),
                    Style = (Style)FindResource("KorisnikButton"),
                    Background = (System.Windows.Media.Brush)FindResource("AccentBrush")
                };
                btnPlus.Click += BtnPlus_Click;

                var btnMinus = new Button
                {
                    Content = "-",
                    Width = 25,
                    Height = 25,
                    Tag = item,
                    Margin = new Thickness(5, 0, 0, 0),
                    Style = (Style)FindResource("KorisnikButton"),
                    Background = (System.Windows.Media.Brush)FindResource("PrimaryBrush")
                };
                btnMinus.Click += BtnMinus_Click;

                panel.Children.Add(txt);
                panel.Children.Add(btnPlus);
                panel.Children.Add(btnMinus);

                listBoxKorpa.Items.Add(panel);
            }

            decimal ukupno = korpa.Sum(x => x.Ukupno);
            textBlockUkupno.Text = $"Ukupno: {ukupno:F2} KM";
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is KorpaItem item)
            {
                item.Kolicina++;
                OsveziKorpu();
            }
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is KorpaItem item)
            {
                item.Kolicina--;
                if (item.Kolicina <= 0)
                    korpa.Remove(item);
                OsveziKorpu();
            }
        }
        #endregion

        #region Kupovina
        private void BtnKupi_Click(object sender, RoutedEventArgs e)
        {
            if (!korpa.Any())
            {
                MessageBox.Show("Korpa je prazna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // otvori prozor za unos kupca
            var narudzbaWin = new NarudzbaWindow(korpa, korisnikId: 1);
            narudzbaWin.ShowDialog();

            korpa.Clear();
            OsveziKorpu();
        }
        #endregion
    }
}