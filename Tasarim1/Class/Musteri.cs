using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExcelToPanorama
{

    public class Musteri : IMusteri
    {
        
        public string Durum { get; set; }
        public string IlgiliKisi { get; set; }
        public string MusteriGrubu { get; set; }
        public string MusteriEkGrubu { get; set; }
        public string OdemeTipi { get; set; }
        public string KisaAdi { get; set; }
        public string MusteriKodu { get; set; }
        public string Unvan { get; set; }
        public string VergiTipi { get; set; }
        public string Adres { get; set; }
        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string TcNo { get; set; }
        public string Telefon { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNumarasi { get; set; }
        public string KoordinatX { get; set; }
        public string KoordinatY { get; set; }
        public string VadeGunu { get; set; }
        public string Iskonto { get; set; }
    }

}
