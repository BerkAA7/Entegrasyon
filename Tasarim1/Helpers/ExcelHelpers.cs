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
using System.Windows.Controls;

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
                    var headers = worksheet.Row(1).Cells().Select(c => ReplaceTurkishCharacters(c.GetString())).ToList();
                    var columnIndices = headers.Select((header, index) => new { header, index }).ToDictionary(x => x.header, x => x.index + 1);

                    foreach (var column in columnIndices)
                    {
                        Console.WriteLine($"{column.Key}: {column.Value}");
                    }

                    foreach (IXLRow row in rows)
                    {
                        var obj = (T)(ISelectable)Activator.CreateInstance(typeof(Musteri)); // Somut Musteri sınıfını oluştur

                        SetMusteriFields(obj, row, columnIndices); // Reflection ile dinamik olarak özellikleri dolduruyoruz
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
        private void SetMusteriFields<T>(T obj, IXLRow row, Dictionary<string, int> columnIndices)
        {
            var propertyMappings = new Dictionary<string, string>()
    {
        { "Durum", "DURUM" },
        { "MusteriKodu", "MUSTERI KODU" },
        { "Unvan", "UNVAN" },
        { "IlgiliKisi", "ILGILI KISI" },
        { "Adres", "ADRES" },
        { "Sehir", "SEHIR" },
        { "Ilce", "ILCE" },
        { "TcNo", "TC NO" },
        { "Telefon", "TELEFON" },
        { "VergiDairesi", "VERGI DAIRESI" },
        { "VergiNumarasi", "VERGI NUMARASI" },
        { "MusteriGrubu", "MUSTERI GRUBU" },
        { "MusteriEkGrubu", "MUSTERI EK GRUBU" },
        { "OdemeTipi", "ODEME TIPI" },
        { "KisaAdi", "KISA ADI" },
        { "VergiTipi", "VERGI TIPI" },
        { "KoordinatX", "KOORDINAT X" },
        { "KoordinatY", "KOORDINAT Y" },
        { "VadeGunu", "VADE GUNU" },
        { "Iskonto", "ISKONTO" }
    };

            foreach (var mapping in propertyMappings)
            {
                var property = obj.GetType().GetProperty(mapping.Key);
                if (property != null && property.CanWrite)
                {
                    var cellValue = GetCellValue(row, columnIndices, mapping.Value);
                    property.SetValue(obj, cellValue);
                }
            }
        }
        private string ReplaceTurkishCharacters(string text)
        {
            return text
                .Trim()
                .ToUpper()
                .Replace("İ", "I")
                .Replace("Ş", "S")
                .Replace("Ç", "C")
                .Replace("Ü", "U")
                .Replace("Ö", "O")
                .Replace("Ğ", "G")
                .Replace("ı", "i")
                .Replace("ç", "c")
                .Replace("ü", "u")
                .Replace("ö", "o")
                .Replace("ğ", "g")
                .Replace("ş", "s");

        }

        private string GetCellValue(IXLRow row, Dictionary<string, int> columnIndices, string columnName)
        {
            columnName = columnName.ToUpper(); // Kolon adını büyük harfe çevir

            if (!columnIndices.ContainsKey(columnName))
            {
                // Eğer kolon adı bulunamazsa null döndürelim ve loglayalım
                Console.WriteLine($"Kolon bulunamadı: {columnName}");
                return null; // Veya bir hata fırlatabiliriz.
            }

            var cell = row.Cell(columnIndices[columnName]);

            // Hücre tipi ve boşlukları kontrol et
            string cellValue;
            if (cell.DataType == XLDataType.Text)
            {
                cellValue = cell.GetString().Trim();
            }
            else
            {
                cellValue = cell.Value.ToString().Trim();
            }

            return string.IsNullOrWhiteSpace(cellValue) ? null : cellValue;
        }
        private void dataGrid_SelectionChanged_3(object sender, SelectionChangedEventArgs e)
        {

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
