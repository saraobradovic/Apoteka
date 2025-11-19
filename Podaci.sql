use apoteka;

INSERT INTO administratori (KorisnickoIme, Lozinka, Ime, Prezime) VALUES
('admin1', 'lozinka123', 'Sara', 'Obradovic'),
('admin2', 'pass456', 'Marko', 'Markovic');

-- dobavljaci
INSERT INTO dobavljaci (Naziv, Kontakt) VALUES
('Farmaco d.o.o.', 'info@farmaco.ba'),
('Medika d.o.o.', 'kontakt@medika.ba'),
('ApotekaPlus', 'prodaja@apotekaplus.ba');

-- kategorije
INSERT INTO kategorije (Naziv) VALUES
('Lijekovi protiv bolova'),
('Vitamini i suplementi'),
('Antibiotici'),
('Injekcije i inzulin'),
('Higijena');

-- kupci
INSERT INTO kupci (Ime, Prezime, Email, Telefon) VALUES
('Petar', 'Petrovic', 'petar@gmail.com', '061123456'),
('Ana', 'Anic', 'ana@yahoo.com', '062987654'),
('Jovan', 'Jovic', 'jovan@hotmail.com', '063555777');

-- narudzbe
INSERT INTO narudzbe (Datum, KupacID) VALUES
('2025-11-19 10:00:00', 1),
('2025-11-19 11:30:00', 2),
('2025-11-19 12:15:00', 3);

-- proizvodi
INSERT INTO proizvodi (Sifra, Naziv, Cijena, Kolicina, KategorijaID, DobavljacID) VALUES
('LJ001', 'Aspirin 500mg', 5.50, 100, 1, 1),
('VITC01', 'Vitamin C 1000mg', 7.20, 50, 2, 2),
('PARA01', 'Paracetamol 500mg', 4.80, 200, 1, 1),
('IBU01', 'Ibuprofen 200mg', 6.00, 150, 1, 1),
('AMO01', 'Amoksicilin 500mg', 12.50, 30, 3, 2),
('INS01', 'Insulin 10ml', 45.00, 20, 4, 3);

-- promjenecijena
INSERT INTO promjenecijena (ProizvodID, StaraCijena, NovaCijena, DatumPromjene, AdministratorID) VALUES
(1, 5.00, 5.50, '2025-11-19 09:00:00', 1),
(2, 7.00, 7.20, '2025-11-19 09:30:00', 2),
(6, 40.00, 45.00, '2025-11-19 10:00:00', 1);

-- racuni
INSERT INTO racuni (NarudzbaID, Datum, UkupanIznos) VALUES
(1, '2025-11-19 10:05:00', 10.30),
(2, '2025-11-19 11:35:00', 19.70),
(3, '2025-11-19 12:20:00', 45.00);

-- stavkenarudzbe
INSERT INTO stavkenarudzbe (NarudzbaID, ProizvodID, Kolicina) VALUES
(1, 1, 1),
(1, 2, 1),
(2, 3, 2),
(2, 4, 1),
(3, 6, 1);