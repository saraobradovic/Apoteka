-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema apoteka
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema apoteka
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `apoteka` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci ;
USE `apoteka` ;

-- -----------------------------------------------------
-- Table `apoteka`.`administratori`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `apoteka`.`administratori` (
  `AdministratorID` INT NOT NULL AUTO_INCREMENT,
  `KorisnickoIme` VARCHAR(50) NOT NULL,
  `Lozinka` VARCHAR(100) NOT NULL,
  `Ime` VARCHAR(50) NULL DEFAULT NULL,
  `Prezime` VARCHAR(50) NULL DEFAULT NULL,
  PRIMARY KEY (`AdministratorID`),
  UNIQUE INDEX `KorisnickoIme` (`KorisnickoIme` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `apoteka`.`dobavljaci`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `apoteka`.`dobavljaci` (
  `DobavljacID` INT NOT NULL AUTO_INCREMENT,
  `Naziv` VARCHAR(100) NOT NULL,
  `Kontakt` VARCHAR(100) NULL DEFAULT NULL,
  PRIMARY KEY (`DobavljacID`))
ENGINE = InnoDB
AUTO_INCREMENT = 9
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `apoteka`.`kategorije`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `apoteka`.`kategorije` (
  `KategorijaID` INT NOT NULL AUTO_INCREMENT,
  `Naziv` VARCHAR(100) NOT NULL,
  PRIMARY KEY (`KategorijaID`))
ENGINE = InnoDB
AUTO_INCREMENT = 10
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `apoteka`.`kupci`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `apoteka`.`kupci` (
  `KupacID` INT NOT NULL AUTO_INCREMENT,
  `Ime` VARCHAR(50) NOT NULL,
  `Prezime` VARCHAR(50) NOT NULL,
  `Email` VARCHAR(100) NULL DEFAULT NULL,
  `Telefon` VARCHAR(20) NULL DEFAULT NULL,
  PRIMARY KEY (`KupacID`))
ENGINE = InnoDB
AUTO_INCREMENT = 11
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `apoteka`.`narudzbe`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `apoteka`.`narudzbe` (
  `NarudzbaID` INT NOT NULL AUTO_INCREMENT,
  `Datum` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `KupacID` INT NULL DEFAULT NULL,
  PRIMARY KEY (`NarudzbaID`),
  INDEX `KupacID` (`KupacID` ASC) VISIBLE,
  CONSTRAINT `narudzbe_ibfk_1`
    FOREIGN KEY (`KupacID`)
    REFERENCES `apoteka`.`kupci` (`KupacID`))
ENGINE = InnoDB
AUTO_INCREMENT = 11
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `apoteka`.`proizvodi`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `apoteka`.`proizvodi` (
  `ProizvodID` INT NOT NULL AUTO_INCREMENT,
  `Sifra` VARCHAR(50) NOT NULL,
  `Naziv` VARCHAR(100) NOT NULL,
  `Cijena` DECIMAL(10,2) NOT NULL,
  `Kolicina` INT NULL DEFAULT '0',
  `KategorijaID` INT NULL DEFAULT NULL,
  `DobavljacID` INT NULL DEFAULT NULL,
  PRIMARY KEY (`ProizvodID`),
  UNIQUE INDEX `Sifra` (`Sifra` ASC) VISIBLE,
  INDEX `KategorijaID` (`KategorijaID` ASC) VISIBLE,
  INDEX `DobavljacID` (`DobavljacID` ASC) VISIBLE,
  CONSTRAINT `proizvodi_ibfk_1`
    FOREIGN KEY (`KategorijaID`)
    REFERENCES `apoteka`.`kategorije` (`KategorijaID`),
  CONSTRAINT `proizvodi_ibfk_2`
    FOREIGN KEY (`DobavljacID`)
    REFERENCES `apoteka`.`dobavljaci` (`DobavljacID`))
ENGINE = InnoDB
AUTO_INCREMENT = 36
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `apoteka`.`promjenecijena`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `apoteka`.`promjenecijena` (
  `PromjenaID` INT NOT NULL AUTO_INCREMENT,
  `ProizvodID` INT NULL DEFAULT NULL,
  `StaraCijena` DECIMAL(10,2) NULL DEFAULT NULL,
  `NovaCijena` DECIMAL(10,2) NULL DEFAULT NULL,
  `DatumPromjene` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `AdministratorID` INT NULL DEFAULT NULL,
  PRIMARY KEY (`PromjenaID`),
  INDEX `ProizvodID` (`ProizvodID` ASC) VISIBLE,
  INDEX `AdministratorID` (`AdministratorID` ASC) VISIBLE,
  CONSTRAINT `promjenecijena_ibfk_1`
    FOREIGN KEY (`ProizvodID`)
    REFERENCES `apoteka`.`proizvodi` (`ProizvodID`),
  CONSTRAINT `promjenecijena_ibfk_2`
    FOREIGN KEY (`AdministratorID`)
    REFERENCES `apoteka`.`administratori` (`AdministratorID`))
ENGINE = InnoDB
AUTO_INCREMENT = 5
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `apoteka`.`racuni`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `apoteka`.`racuni` (
  `RacunID` INT NOT NULL AUTO_INCREMENT,
  `NarudzbaID` INT NULL DEFAULT NULL,
  `Datum` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `UkupanIznos` DECIMAL(10,2) NULL DEFAULT NULL,
  PRIMARY KEY (`RacunID`),
  INDEX `NarudzbaID` (`NarudzbaID` ASC) VISIBLE,
  CONSTRAINT `racuni_ibfk_1`
    FOREIGN KEY (`NarudzbaID`)
    REFERENCES `apoteka`.`narudzbe` (`NarudzbaID`))
ENGINE = InnoDB
AUTO_INCREMENT = 9
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


-- -----------------------------------------------------
-- Table `apoteka`.`stavkenarudzbe`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `apoteka`.`stavkenarudzbe` (
  `StavkaID` INT NOT NULL AUTO_INCREMENT,
  `NarudzbaID` INT NULL DEFAULT NULL,
  `ProizvodID` INT NULL DEFAULT NULL,
  `Kolicina` INT NOT NULL,
  PRIMARY KEY (`StavkaID`),
  INDEX `NarudzbaID` (`NarudzbaID` ASC) VISIBLE,
  INDEX `ProizvodID` (`ProizvodID` ASC) VISIBLE,
  CONSTRAINT `stavkenarudzbe_ibfk_1`
    FOREIGN KEY (`NarudzbaID`)
    REFERENCES `apoteka`.`narudzbe` (`NarudzbaID`),
  CONSTRAINT `stavkenarudzbe_ibfk_2`
    FOREIGN KEY (`ProizvodID`)
    REFERENCES `apoteka`.`proizvodi` (`ProizvodID`))
ENGINE = InnoDB
AUTO_INCREMENT = 14
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
