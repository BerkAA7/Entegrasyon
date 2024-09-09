using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}
