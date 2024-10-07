using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToPanorama.Class
{
    public class Urun : IUrun
    {
        public bool Secim {  get; set; }
        public string UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public string UrunKisaAdi { get; set; }
        public string UrunGrupKodu { get; set; }
        public string UrunEkGrupKodu { get; set; }
        public string SeviyeliGrup1 { get; set; }
        public string UreticiKodu { get; set; }
        public string Birim1 { get; set; }
        public string Barkod1 { get; set; }
        public string Birim2 { get; set; }
        public string Barkod2 { get; set; }
        public string BirimCarpani2 { get; set; }
        public string Birim3 { get; set; }
        public string Barkod3 { get; set; }
        public string BirimCarpani3 { get; set; }
        public string SatisKDVOrani { get; set; }
        public string UrunTip { get; set; }
        public string AlisKDVOrani { get; set; }
        public string UrunAciklama { get; set; }
    }

}
