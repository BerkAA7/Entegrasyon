﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToPanorama.Class
{
    public interface IUrun :ISelectable
    {
        bool Secim {  get; set; }
        string UrunKodu { get; set; }
        string UrunAdi { get; set; }
        string UrunKisaAdi { get; set; }
        string UrunGrupKodu { get; set; }
        string UrunEkGrupKodu { get; set; }
        string SeviyeliGrup1 { get; set; }
        string UreticiKodu { get; set; }
        string Birim1 { get; set; }
        string Barkod1 { get; set; }
        string Birim2 { get; set; }
        string Barkod2 { get; set; }
        string BirimCarpani2 { get; set; }
        string Birim3 { get; set; }
        string Barkod3 { get; set; }
        string BirimCarpani3 { get; set; }
        string SatisKDVOrani { get; set; }
        string UrunTip { get; set; }
        string AlisKDVOrani { get; set; }
        string UrunAciklama { get; set; }
    }

}
