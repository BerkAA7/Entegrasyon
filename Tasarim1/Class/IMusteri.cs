using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToPanorama
{
    public interface IMusteri
    {
        string Durum { get; set; }
        string MusteriKodu { get; set; }
        string Unvan { get; set; }
        string IlgiliKisi { get; set; }
        string MusteriGrubu { get; set; }
        string MusteriEkGrubu { get; set; }
        string OdemeTipi { get; set; }
        string KisaAdi { get; set; }
        string VergiTipi { get; set; }
    }
}