using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WPF_LoginForm.View;



namespace Tasarim1
{

        public class CustomerIntegration
        {
            public int? Durum { get; set; }
            public string ErpKod2 { get; set; }
            public string Unvan { get; set; }
            public string IlgiliKisi { get; set; }
            public string Adres1 { get; set; }
            public string Adres2 { get; set; }
            public string MerkezIlTextKod { get; set; }
            public string Ilce { get; set; }
            public string TCKimlikNo { get; set; }
            public string CepTelNo { get; set; }
            public string VD { get; set; }
            public string VN { get; set; }
            public string MusteriGrupTextKod { get; set; }
            public string MusteriEkGrupTextKod { get; set; }
            public int? OdemeTipi { get; set; }
            public string KisaAd { get; set; }
            public int? KdvMuaf { get; set; }
            public decimal? KoordinatX { get; set; }
            public decimal? KoordinatY { get; set; }
            public int? VadeGun { get; set; }
            public decimal? IskontoOran { get; set; }
    }

    public class ProductIntegration
    {
        public string TextKod { get; set; }
        public string Ad { get; set; }
        public string KisaAd { get; set; }
        public string UrunGrupKod { get; set; }
        public string UrunEkGrupKod { get; set; }
        public string Hiyerarsi1TextKod { get; set; }
        public string UreticiKodu { get; set; }
        public string Birim1 { get; set; }
        public string Barkod1 { get; set; }
        public string Birim2 { get; set; }
        public string Barkod2 { get; set; }
        public decimal? Cevrim2 { get; set; }
        public string Birim3 { get; set; }
        public string Barkod3 { get; set; }
        public decimal? Cevrim3 { get; set; }
        public decimal? KdvOran { get; set; }
        public byte? UrunTip { get; set; }
        public decimal? Kdvoranalis { get; set; }
        public string UrunAciklama { get; set; }

    }
}
