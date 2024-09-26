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
        private List<IMusteri> musteriList = new List<IMusteri>();
        public List<IMusteri> ReadExcelFile(string filePath)
        {
            try
            {
                musteriList.Clear();
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // İlk sayfayı seç
                    var rows = worksheet.RowsUsed().Skip(1); // İlk satırı başlık olarak say
                    var headers = worksheet.Row(1).Cells().Select(c => c.GetString()).ToList();
                    var columnIndices = headers.Select((header, index) => new { header, index }).ToDictionary(x => x.header, x => x.index + 1);

                    foreach (var row in rows)
                    {
                        var musteri = new Musteri
                        {
                            Durum = row.Cell(columnIndices.ContainsKey("Durum") ? columnIndices["Durum"] : 1).GetString(),
                            MusteriKodu = row.Cell(columnIndices.ContainsKey("MusteriKodu") ? columnIndices["MusteriKodu"] : 2).GetString(),
                            Unvan = row.Cell(columnIndices.ContainsKey("Unvan") ? columnIndices["Unvan"] : 3).GetString(),
                            IlgiliKisi = row.Cell(columnIndices.ContainsKey("IlgiliKisi") ? columnIndices["IlgiliKisi"] : 4).GetString(),
                            Adres = row.Cell(columnIndices.ContainsKey("Adres") ? columnIndices["Adres"] : 5).GetString(),
                            Sehir = row.Cell(columnIndices.ContainsKey("Şehir") ? columnIndices["Şehir"] : 6).GetString(),
                            Ilce = row.Cell(columnIndices.ContainsKey("İlçe") ? columnIndices["İlçe"] : 7).GetString(),
                            TcNo = row.Cell(columnIndices.ContainsKey("Tc No") ? columnIndices["Tc No"] : 8).GetString(),
                            Telefon = row.Cell(columnIndices.ContainsKey("Telefon") ? columnIndices["Telefon"] : 9).GetString(),
                            VergiDairesi = row.Cell(columnIndices.ContainsKey("Vergi Dairesi") ? columnIndices["Vergi Dairesi"] : 10).GetString(),
                            VergiNumarasi = row.Cell(columnIndices.ContainsKey("Vergi Numarası") ? columnIndices["Vergi Numarası"] : 11).GetString(),
                            MusteriGrubu = row.Cell(columnIndices.ContainsKey("MusteriGrubu") ? columnIndices["MusteriGrubu"] : 12).GetString(),
                            MusteriEkGrubu = row.Cell(columnIndices.ContainsKey("MusteriEkGrubu") ? columnIndices["MusteriEkGrubu"] : 13).GetString(),
                            OdemeTipi = row.Cell(columnIndices.ContainsKey("OdemeTipi") ? columnIndices["OdemeTipi"] : 14).GetString(),
                            KisaAdi = row.Cell(columnIndices.ContainsKey("KisaAdi") ? columnIndices["KisaAdi"] : 15).GetString(),
                            VergiTipi = row.Cell(columnIndices.ContainsKey("VergiTipi") ? columnIndices["VergiTipi"] : 16).GetString(),
                            KoordinatX = row.Cell(columnIndices.ContainsKey("Koordinat X") ? columnIndices["Koordinat X"] : 17).GetString(),
                            KoordinatY = row.Cell(columnIndices.ContainsKey("Koordinat Y") ? columnIndices["Koordinat Y"] : 18).GetString(),
                            VadeGunu = row.Cell(columnIndices.ContainsKey("VADE GÜNÜ") ? columnIndices["VADE GÜNÜ"] : 19).GetString(),
                            Iskonto = row.Cell(columnIndices.ContainsKey("İSKONTO") ? columnIndices["İSKONTO"] : 20).GetString()


                        };
                        musteriList.Add(musteri); // Listeye ekleme
                    }
                    //_ = musteriList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }

            return musteriList;
        }
        public List<IMusteri> GetMusteriList()
        {
            return musteriList; // Global listeyi döndürme
        }
        public void MusteriAL(List<IMusteri> GuncellenmisMustList)
        {
            musteriList = GuncellenmisMustList;
            dataGrid.ItemsSource = musteriList;
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
                List<IMusteri> musteri = ReadExcelFile(filePath);
                if (musteri != null && musteri.Any())
                {
                    this.MusteriAL(musteri);
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
