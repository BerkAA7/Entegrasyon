using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelToPanorama;
using ExcelToPanorama.Class;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using ExcelToPanorama.Interface;

namespace ExcelToPanorama.Helpers
{
    internal class ExcelHelper 
    {
        string filePath { get; set; }
        public void BtnExcelYukle<T>(Func<string, List<T>> readExcelFileFunc, Action<List<T>> alFunc)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Excel Dosyaları|*.xls;*.xlsx;*.xlsm",
                Title = "Excel Dosyası Seçin"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                this.filePath = filePath;
                List<T> item = readExcelFileFunc(filePath); // Excel dosyasını oku
                if (item != null && item.Any())
                {
                    alFunc(item); // Listeyi al
                }
                else
                {
                    MessageBox.Show("Veri yüklenemedi.");
                }
            }
        }


        #region ReadExcelGroup
        public List<T> ReadExcelFile<T>(string filePath) where T : ISelectable
        {
            try
            {
                List<T> list = new List<T>();
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // İlk sayfayı seç
                    var rows = worksheet.RowsUsed().Skip(1); // İlk satırı başlık olarak say
                    var headers = worksheet.Row(1).Cells().Select(c => c.GetString()).ToList();
                    var columnIndices = headers.Select((header, index) => new { header, index }).ToDictionary(x => x.header, x => x.index + 1);

                    foreach (var row in rows)
                    {
                        T obj = default(T); // Başlangıçta varsayılan değeri ayarlayın

                        if (typeof(T) == typeof(IMusteri)) // Musteri işlemleri
                        {
                            obj = (T)(ISelectable)Activator.CreateInstance(typeof(Musteri)); // Somut Musteri sınıfını oluştur
                            var musteri = (IMusteri)obj;

                            // Hücre değerlerini atayın
                            musteri.Durum = GetCellValue(row, columnIndices, "Durum", 1);
                            musteri.MusteriKodu = GetCellValue(row, columnIndices, "MusteriKodu", 2);
                            musteri.Unvan = GetCellValue(row, columnIndices, "Unvan", 3);
                            musteri.IlgiliKisi = GetCellValue(row, columnIndices, "IlgiliKisi", 4);
                            musteri.Adres = GetCellValue(row, columnIndices, "Adres", 5);
                            musteri.Sehir = GetCellValue(row, columnIndices, "Şehir", 6);
                            musteri.Ilce = GetCellValue(row, columnIndices, "İlçe", 7);
                            musteri.TcNo = GetCellValue(row, columnIndices, "Tc No", 8);
                            musteri.Telefon = GetCellValue(row, columnIndices, "Telefon", 9);
                            musteri.VergiDairesi = GetCellValue(row, columnIndices, "Vergi Dairesi", 10);
                            musteri.VergiNumarasi = GetCellValue(row, columnIndices, "Vergi Numarası", 11);
                            musteri.MusteriGrubu = GetCellValue(row, columnIndices, "MusteriGrubu", 12);
                            musteri.MusteriEkGrubu = GetCellValue(row, columnIndices, "MusteriEkGrubu", 13);
                            musteri.OdemeTipi = GetCellValue(row, columnIndices, "OdemeTipi", 14);
                            musteri.KisaAdi = GetCellValue(row, columnIndices, "KisaAdi", 15);
                            musteri.VergiTipi = GetCellValue(row, columnIndices, "VergiTipi", 16);
                            musteri.KoordinatX = GetCellValue(row, columnIndices, "Koordinat X", 17);
                            musteri.KoordinatY = GetCellValue(row, columnIndices, "Koordinat Y", 18);
                            musteri.VadeGunu = GetCellValue(row, columnIndices, "VADE GÜNÜ", 19);
                            musteri.Iskonto = GetCellValue(row, columnIndices, "İSKONTO", 20);
                        }
                        else if (typeof(T) == typeof(IUrun)) // Urun işlemleri
                        {
                            obj = (T)(ISelectable)Activator.CreateInstance(typeof(Urun)); // Somut Urun sınıfını oluştur
                            var urun = (IUrun)obj;

                            // Hücre değerlerini atayın
                            urun.UrunKodu = GetCellValue(row, columnIndices, "Ürün Kodu", 1);
                            urun.UrunAdi = GetCellValue(row, columnIndices, "Ürün Adı", 2);
                            urun.UrunKisaAdi = GetCellValue(row, columnIndices, "Ürün Kısa Adı", 3);
                            urun.UrunGrupKodu = GetCellValue(row, columnIndices, "Ürün Grup Kodu", 4);
                            urun.UrunEkGrupKodu = GetCellValue(row, columnIndices, "Ürün Ek Grup Kodu", 5);
                            urun.SeviyeliGrup1 = GetCellValue(row, columnIndices, "Seviyeli Grup 1", 6);
                            urun.UreticiKodu = GetCellValue(row, columnIndices, "Üretici Kodu", 7);
                            urun.Birim1 = GetCellValue(row, columnIndices, "Birim 1", 8);
                            urun.Barkod1 = GetCellValue(row, columnIndices, "Barkod 1", 9);
                            urun.Birim2 = GetCellValue(row, columnIndices, "Birim 2", 10);
                            urun.Barkod2 = GetCellValue(row, columnIndices, "Barkod 2", 11);
                            urun.BirimCarpani2 = GetCellValue(row, columnIndices, "Birim Çarpanı 2", 12);
                            urun.Birim3 = GetCellValue(row, columnIndices, "Birim 3", 13);
                            urun.Barkod3 = GetCellValue(row, columnIndices, "Barkod 3", 14);
                            urun.BirimCarpani3 = GetCellValue(row, columnIndices, "Birim Çarpanı 3", 15);
                            urun.SatisKDVOrani = GetCellValue(row, columnIndices, "Satış KDV Oranı", 16);
                            urun.UrunTip = GetCellValue(row, columnIndices, "URUN TIP", 17);
                            urun.AlisKDVOrani = GetCellValue(row, columnIndices, "ALIS KDV ORANI", 18);
                            urun.UrunAciklama = GetCellValue(row, columnIndices, "URUN ACIKLAMA", 19);
                        }

                        list.Add(obj);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
                return null;
            }
        }

        private string GetCellValue(IXLRow row, Dictionary<string, int> columnIndices, string columnName, int defaultIndex)
        {

            // Belirtilen kolonu bulamazsa varsayılan index'i kullan
            var cell = row.Cell(columnIndices.ContainsKey(columnName) ? columnIndices[columnName] : defaultIndex);

            // Hücre tipi ve boşlukları kontrol et
            string cellValue = cell?.GetValue<string>()?.Trim(); // Hücre boşsa null döner
            return string.IsNullOrWhiteSpace(cellValue) ? null : cellValue; // Boşsa null döndür, aksi halde hücre değerini döndür
        }
        #endregion

        public static T CreateInstance<T>() where T : ISelectable
        {
            if (typeof(T) == typeof(IMusteri))
            {
                return (T)(ISelectable)new Musteri();
            }
            else if (typeof(T) == typeof(IUrun))
            {
                return (T)(ISelectable)new Urun();
            }

            throw new InvalidOperationException("Geçerli bir tür değil.");
        }
        public static List<T> GetCheckedRowsFromList<T>(List<T> itemList) where T : ISelectable
        {
            if (itemList == null || !itemList.Any())
                return new List<T>(); // Eğer liste boşsa, boş liste döndür

            var seçiliSatırlar = new List<T>(); // Seçili listeyi oluştur

            foreach (var item in itemList)
            {
                if (item.Secim) // secim özelliği true ise
                {
                    seçiliSatırlar.Add(item); // Seçili listeye ekle
                }
            }

            return seçiliSatırlar; // Seçili öğeleri döndür
        }
    }
}
