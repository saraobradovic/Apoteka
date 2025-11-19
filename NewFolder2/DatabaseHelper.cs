using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ApotekaApp.Data
{
    public class DatabaseHelper
    {
        // Podesi svoju konekciju
        private string ConnectionString = "server=localhost;database=apoteka;user=root;password=root;";
       
        #region Administratori

        public bool LoginAdministrator(string username, string password)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Administratori WHERE KorisnickoIme=@user AND Lozinka=@pass";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška prilikom prijave: " + ex.Message);
                return false;
            }
        }


        #endregion

        #region Kategorije i Proizvodi

        public DataTable GetKategorije()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Kategorije";
                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable GetProizvodi()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string query = @"
            SELECT 
                p.ProizvodID,
               
                p.Naziv AS Proizvod,
                p.Cijena,
                p.Kolicina,
                k.Naziv AS Kategorija,
                d.Naziv AS Dobavljac
            FROM Proizvodi p
            LEFT JOIN Kategorije k ON p.KategorijaID = k.KategorijaID
            LEFT JOIN Dobavljaci d ON p.DobavljacID = d.DobavljacID;
        ";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                da.Fill(dt);
            }
            return dt;
        }


        public DataTable GetProizvodiByKategorija(int kategorijaId)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Proizvodi WHERE KategorijaID=@katId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@katId", kategorijaId);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        #endregion

        #region Kupci i Narudzbe





        public int DodajKupca(string ime, string prezime, string email, string telefon)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO Kupci (Ime, Prezime, Email, Telefon) VALUES (@ime, @prezime, @email, @telefon)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ime", ime);
                cmd.Parameters.AddWithValue("@prezime", prezime);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@telefon", telefon);
                cmd.ExecuteNonQuery();
                return (int)cmd.LastInsertedId;
            }
        }

        // 🟢 Kreiraj račun
        public void KreirajRacun(int narudzbaId, decimal ukupno)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO Racuni (NarudzbaID, UkupanIznos) VALUES (@narudzbaId, @ukupno)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@narudzbaId", narudzbaId);
                cmd.Parameters.AddWithValue("@ukupno", ukupno);
                cmd.ExecuteNonQuery();
            }
        }
        public int KreirajNarudzbu(int kupacId)
        {
            int narudzbaId = 0;

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string query = @"
            INSERT INTO Narudzbe (KupacID, Datum)
            VALUES (@kupacId, @datum);
            SELECT LAST_INSERT_ID();
        ";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kupacId", kupacId);
                cmd.Parameters.AddWithValue("@datum", DateTime.Now);

                narudzbaId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return narudzbaId;
        }


        public void DodajStavkuNarudzbe(int narudzbaId, int proizvodId, int kolicina)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO StavkeNarudzbe (NarudzbaID, ProizvodID, Kolicina) VALUES (@narudzbaId, @proizvodId, @kolicina)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@narudzbaId", narudzbaId);
                cmd.Parameters.AddWithValue("@proizvodId", proizvodId);
                cmd.Parameters.AddWithValue("@kolicina", kolicina);
                cmd.ExecuteNonQuery();
            }
        }




        #endregion

        #region Promjene cijena

        public bool PromijeniCijenuIKolicinuProizvoda(int proizvodId, decimal cijena, int kolicina)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "UPDATE Proizvodi SET Cijena=@cijena, Kolicina=@kolicina WHERE ProizvodID=@proizvodId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cijena", cijena);
                cmd.Parameters.AddWithValue("@kolicina", kolicina);
                cmd.Parameters.AddWithValue("@proizvodId", proizvodId);

                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public void ObrisiProizvod(int proizvodId)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "DELETE FROM Proizvodi WHERE ProizvodID=@id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", proizvodId);
                cmd.ExecuteNonQuery();
            }
        }
        public DataTable PretraziProizvode(string naziv)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Proizvodi WHERE Naziv LIKE @naziv";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@naziv", "%" + naziv + "%");
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }


        public bool DodajProizvod(string sifra, string naziv, decimal cijena, int kolicina, int kategorijaId, int dobavljacID)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Proizvodi (Sifra, Naziv, Cijena, Kolicina, KategorijaID, DobavljacID) " +
                                   "VALUES (@sifra, @naziv, @cijena, @kolicina, @kategorija, @dobavljac)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@sifra", sifra);
                    cmd.Parameters.AddWithValue("@naziv", naziv);
                    cmd.Parameters.AddWithValue("@cijena", cijena);
                    cmd.Parameters.AddWithValue("@kolicina", kolicina);
                    cmd.Parameters.AddWithValue("@kategorija", kategorijaId);
                    cmd.Parameters.AddWithValue("@dobavljac", dobavljacID);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataTable GetDobavljace()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT DobavljacID, Naziv FROM Dobavljaci";
                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                da.Fill(dt);
            }
            return dt;
        }

        // 2️⃣ Vraća ID dobavljača po nazivu, 0 ako ne postoji
        public int GetDobavljacIdByName(string naziv)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT DobavljacID FROM Dobavljaci WHERE Naziv=@naziv";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@naziv", naziv);
                object result = cmd.ExecuteScalar();
                if (result != null)
                    return Convert.ToInt32(result);
                return 0;
            }
        }

        // 3️⃣ Dodaje novog dobavljača i vraća njegov ID
        public int DodajDobavljaca(string naziv)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO Dobavljaci (Naziv) VALUES (@naziv); SELECT LAST_INSERT_ID();";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@naziv", naziv);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public int DodajKategoriju(string naziv)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO Kategorije (Naziv) VALUES (@naziv); SELECT LAST_INSERT_ID();";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@naziv", naziv);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        public DataTable GetNarudzbe()
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string query = @"
            SELECT 
                n.NarudzbaID,
                n.Datum,
                k.Ime AS Kupac
            FROM Narudzbe n
            INNER JOIN Kupci k ON n.KupacID = k.KupacID
            ORDER BY n.Datum DESC";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            return dt;
        }


        public DataTable GetRacuni()
        {
            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string query = @"
            SELECT 
                r.RacunID,
                r.NarudzbaID,
                r.Datum,
                r.UkupanIznos,
                k.Ime AS Kupac
            FROM Racuni r
            INNER JOIN Narudzbe n ON r.NarudzbaID = n.NarudzbaID
            INNER JOIN Kupci k ON n.KupacID = k.KupacID
            ORDER BY r.Datum DESC;
        ";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            return dt;
        }
        public void OtkaziNarudzbu(int narudzbaId)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (MySqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1️⃣ Učitaj stavke narudžbe u listu
                        string queryStavke = "SELECT ProizvodID, Kolicina FROM StavkeNarudzbe WHERE NarudzbaID=@narudzbaId";
                        MySqlCommand cmdStavke = new MySqlCommand(queryStavke, conn, tran);
                        cmdStavke.Parameters.AddWithValue("@narudzbaId", narudzbaId);

                        var stavke = new List<(int ProizvodID, int Kolicina)>();
                        using (var reader = cmdStavke.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                stavke.Add((reader.GetInt32("ProizvodID"), reader.GetInt32("Kolicina")));
                            }
                        } // DataReader zatvoren ovdje

                        // 2️⃣ Povrat količine proizvoda u skladište
                        foreach (var s in stavke)
                        {
                            string updateProizvod = "UPDATE Proizvodi SET Kolicina = Kolicina + @kolicina WHERE ProizvodID=@proizvodId";
                            MySqlCommand cmdUpdate = new MySqlCommand(updateProizvod, conn, tran);
                            cmdUpdate.Parameters.AddWithValue("@kolicina", s.Kolicina);
                            cmdUpdate.Parameters.AddWithValue("@proizvodId", s.ProizvodID);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        // 3️⃣ Obriši stavke narudžbe
                        string deleteStavke = "DELETE FROM StavkeNarudzbe WHERE NarudzbaID=@narudzbaId";
                        MySqlCommand cmdDeleteStavke = new MySqlCommand(deleteStavke, conn, tran);
                        cmdDeleteStavke.Parameters.AddWithValue("@narudzbaId", narudzbaId);
                        cmdDeleteStavke.ExecuteNonQuery();

                        // 4️⃣ Obriši račun
                        string deleteRacun = "DELETE FROM Racuni WHERE NarudzbaID=@narudzbaId";
                        MySqlCommand cmdDeleteRacun = new MySqlCommand(deleteRacun, conn, tran);
                        cmdDeleteRacun.Parameters.AddWithValue("@narudzbaId", narudzbaId);
                        cmdDeleteRacun.ExecuteNonQuery();

                        // 5️⃣ Obriši samu narudžbu
                        string deleteNarudzba = "DELETE FROM Narudzbe WHERE NarudzbaID=@narudzbaId";
                        MySqlCommand cmdDeleteNarudzba = new MySqlCommand(deleteNarudzba, conn, tran);
                        cmdDeleteNarudzba.Parameters.AddWithValue("@narudzbaId", narudzbaId);
                        cmdDeleteNarudzba.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }








        #endregion
    }
}
