using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Xml;
using Flurl.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Documents;
using System.Xml.XPath;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Text;
using WPF_LoginForm;
using Tasarim1;
using System.Reflection;
using System.Windows.Controls.Primitives;
using ClosedXML.Excel;
using ExcelToPanorama.Class;


namespace ExcelToPanorama
{
    public partial class UrunAktarim : Window

    {


        //private DataTable dataTable;
        public UrunAktarim()
        {
            InitializeComponent();
            VersionRun.Text = GetVersionNumber();//version numarası yazıldı
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)//ekran küçültme
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        public string GetVersionNumber()//version numarasını aldık 
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            SecimEkrani secimEkrani = new SecimEkrani();
            secimEkrani.Show();
            this.Close();
        }

        private void btnBilgileriAktar_Click(object sender, RoutedEventArgs e)
        {

        }

        private string NormalizeSpaces(string input)//boşlukları kaldıran fonk
        {
            // Birden fazla ardışık boşluğu tek bir boşluk ile değiştirir
            return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");
        }
        private string ReplaceTurkishCharacters(string text)
        {
            return text
                .Replace("ı", "i")
                .Replace("İ", "I")
                .Replace("ş", "s")
                .Replace("Ş", "S")
                .Replace("ç", "c")
                .Replace("Ç", "C")
                .Replace("ü", "u")
                .Replace("Ü", "U")
                .Replace("ö", "o")
                .Replace("Ö", "O")
                .Replace("ğ", "g")
                .Replace("Ğ", "G");
        }
        private string RemoveAllSpaces(string input)
        {
            // Tüm boşlukları kaldırır
            return input.Replace(" ", string.Empty);
        }
        private List<IUrun> urunList = new List<IUrun>();
        public List<IUrun> ReadExcelFile(string filePath)
        {
            try
            {
                urunList.Clear();
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // İlk sayfayı seç
                    var rows = worksheet.RowsUsed().Skip(1); // İlk satırı başlık olarak say
                    var headers = worksheet.Row(1).Cells().Select(c => c.GetString()).ToList();
                    var columnIndices = headers.Select((header, index) => new { header, index }).ToDictionary(x => x.header, x => x.index + 1);

                    foreach (var row in rows)
                    {
                        var urun = new Urun
                        {
                            UrunKodu = row.Cell(columnIndices.ContainsKey("Ürün Kodu") ? columnIndices["Ürün Kodu"] : 1).GetString(),
                            UrunAdi = row.Cell(columnIndices.ContainsKey("Ürün Adı") ? columnIndices["Ürün Adı"] : 2).GetString(),
                            UrunKisaAdi = row.Cell(columnIndices.ContainsKey("Ürün Kısa Adı") ? columnIndices["Ürün Kısa Adı"] : 3).GetString(),
                            UrunGrupKodu = row.Cell(columnIndices.ContainsKey("Ürün Grup Kodu") ? columnIndices["Ürün Grup Kodu"] : 4).GetString(),
                            UrunEkGrupKodu = row.Cell(columnIndices.ContainsKey("Ürün Ek Grup Kodu") ? columnIndices["Ürün Ek Grup Kodu"] : 5).GetString(),
                            SeviyeliGrup1 = row.Cell(columnIndices.ContainsKey("Seviyeli Grup 1") ? columnIndices["Seviyeli Grup 1"] : 6).GetString(),
                            UreticiKodu = row.Cell(columnIndices.ContainsKey("Üretici Kodu") ? columnIndices["Üretici Kodu"] : 7).GetString(),
                            Birim1 = row.Cell(columnIndices.ContainsKey("Birim 1") ? columnIndices["Birim 1"] : 8).GetString(),
                            Barkod1 = row.Cell(columnIndices.ContainsKey("Barkod 1") ? columnIndices["Barkod 1"] : 9).GetString(),
                            Birim2 = row.Cell(columnIndices.ContainsKey("Birim 2") ? columnIndices["Birim 2"] : 10).GetString(),
                            Barkod2 = row.Cell(columnIndices.ContainsKey("Barkod 2") ? columnIndices["Barkod 2"] : 11).GetString(),

                            // decimal alanlar için TryParse kullanıyoruz
                            BirimCarpani2 = decimal.TryParse(row.Cell(columnIndices.ContainsKey("Birim Çarpanı 2") ? columnIndices["Birim Çarpanı 2"] : 12).GetString(), out var birimCarpani2) ? birimCarpani2 : 0,
                            Birim3 = row.Cell(columnIndices.ContainsKey("Birim 3") ? columnIndices["Birim 3"] : 13).GetString(),
                            Barkod3 = row.Cell(columnIndices.ContainsKey("Barkod 3") ? columnIndices["Barkod 3"] : 14).GetString(),
                            BirimCarpani3 = decimal.TryParse(row.Cell(columnIndices.ContainsKey("Birim Çarpanı 3") ? columnIndices["Birim Çarpanı 3"] : 15).GetString(), out var birimCarpani3) ? birimCarpani3 : 0,
                            SatisKDVOrani = decimal.TryParse(row.Cell(columnIndices.ContainsKey("Satış KDV Oranı") ? columnIndices["Satış KDV Oranı"] : 16).GetString(), out var satisKDVOrani) ? satisKDVOrani : 0,
                            UrunTip = row.Cell(columnIndices.ContainsKey("URUN TIP") ? columnIndices["URUN TIP"] : 17).GetString(),
                            AlisKDVOrani = decimal.TryParse(row.Cell(columnIndices.ContainsKey("ALIS KDV ORANI") ? columnIndices["ALIS KDV ORANI"] : 18).GetString(), out var alisKDVOrani) ? alisKDVOrani : 0,
                            UrunAciklama = row.Cell(columnIndices.ContainsKey("URUN ACIKLAMA") ? columnIndices["URUN ACIKLAMA"] : 19).GetString()

                        };
                        urunList.Add(urun); // Listeye ekleme
                    }
                    //_ = musteriList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }

            return urunList;
        }
        public List<IUrun> GetMusteriList()
        {
            return urunList; // Global listeyi döndürme
        }
        public void MusteriAL(List<IUrun> GuncellenmisUrunList)
        {
            urunList = GuncellenmisUrunList;
            dataGrid.ItemsSource = urunList;
            dataGrid.Items.Refresh(); // DataGrid'i yenile

            //return musteriList; // Global listeyi döndürme
        }
        private async void btnExcelYükle_Click(object sender, RoutedEventArgs e)
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
                List<IUrun> urun = ReadExcelFile(filePath);
                if (urun != null && urun.Any())
                {
                    this.MusteriAL(urun);
                }
                else
                {
                    MessageBox.Show("Veri yüklenemedi.");
                }
            }
        }
        private void btnKolonSabitleriniDegistir_Click(object sender, RoutedEventArgs e)
        {
            KolonIsterlerUrun ekran = new KolonIsterlerUrun();
            ekran.Show();
        }
        public enum RequiredColumns//zorunlu alanlar
        {
            UrunKodu,
            UrunAdi,
            UrunGrupKodu,
            UrunEkGrupKodu,
            SeviyeliGrup1,
            UreticiKodu,
            Birim1,
            SatisKDVOrani,
            AlisKDVOrani
        }
    }
}
