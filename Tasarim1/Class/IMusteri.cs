using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ExcelToPanorama
{
    public interface IMusteri
    {
        bool? secim { get; set; }
        string Durum { get; set; }
        string MusteriKodu { get; set; }
        string Unvan { get; set; }
        string IlgiliKisi { get; set; }
        string Adres { get; set; }
        string Sehir { get; set; }
        string Ilce { get; set; }
        string TcNo { get; set; }
        string Telefon { get; set; }
        string VergiDairesi { get; set; }
        string VergiNumarasi { get; set; }
        string MusteriGrubu { get; set; }
        string MusteriEkGrubu { get; set; }
        string OdemeTipi { get; set; }
        string KisaAdi { get; set; }
        string VergiTipi { get; set; }
        string KoordinatX { get; set; }
        string KoordinatY { get; set; }
        string VadeGunu { get; set; }
        string Iskonto { get; set; }

    }
}