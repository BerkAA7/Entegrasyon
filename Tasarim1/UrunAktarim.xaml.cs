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
using DocumentFormat.OpenXml.Spreadsheet;


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


        #region EXCEL YÜKLEME İÇİN...
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
        private CancellationTokenSource cancellationTokenSource;

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
                            UrunKodu = GetCellValue(row, columnIndices, "Ürün Kodu", 1),
                            UrunAdi = GetCellValue(row, columnIndices, "Ürün Adı", 2),
                            UrunKisaAdi = GetCellValue(row, columnIndices, "Ürün Kısa Adı", 3),
                            UrunGrupKodu = GetCellValue(row, columnIndices, "Ürün Grup Kodu", 4),
                            UrunEkGrupKodu = GetCellValue(row, columnIndices, "Ürün Ek Grup Kodu", 5),
                            SeviyeliGrup1 = GetCellValue(row, columnIndices, "Seviyeli Grup 1", 6),
                            UreticiKodu = GetCellValue(row, columnIndices, "Üretici Kodu", 7),
                            Birim1 = GetCellValue(row, columnIndices, "Birim 1", 8),
                            Barkod1 = GetCellValue(row, columnIndices, "Barkod 1", 9),
                            Birim2 = GetCellValue(row, columnIndices, "Birim 2", 10),
                            Barkod2 = GetCellValue(row, columnIndices, "Barkod 2", 11),
                            BirimCarpani2 = GetCellValue(row, columnIndices, "Birim Çarpanı 2", 12),
                            Birim3 = GetCellValue(row, columnIndices, "Birim 3", 13),
                            Barkod3 = GetCellValue(row, columnIndices, "Barkod 3", 14),
                            BirimCarpani3 = GetCellValue(row, columnIndices, "Birim Çarpanı 3", 15),
                            SatisKDVOrani = GetCellValue(row, columnIndices, "Satış KDV Oranı", 16),
                            UrunTip = GetCellValue(row, columnIndices, "URUN TIP", 17),
                            AlisKDVOrani = GetCellValue(row, columnIndices, "ALIS KDV ORANI", 18),
                            UrunAciklama = GetCellValue(row, columnIndices, "URUN ACIKLAMA", 19)

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
        public List<IUrun> GetUrunList()
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


        private string GetCellValue(IXLRow row, Dictionary<string, int> columnIndices, string columnName, int defaultIndex)
        {
            var cell = row.Cell(columnIndices.ContainsKey(columnName) ? columnIndices[columnName] : defaultIndex);

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
        #endregion

        #region KOLON SABİTLERİNİ DEĞİŞTİR BUTONU
        private void btnKolonSabitleriniDegistir_Click(object sender, RoutedEventArgs e)
        {
            KolonIsterlerUrun ekran = new KolonIsterlerUrun();
            ekran.Show();
        } 
        #endregion

        #region BİLGİLERİ AKTAR BUTONU
        private async void btnBilgileriAktar_Click(object sender, RoutedEventArgs e)
        {
            string panServisLinki = txtLink.Text;
            string panServisSifresi = txtSifre.Text;
            string dist = txtDist.Text;
            string firmaKodu = txtFirmaKodu.Text;
            string calismaYili = txtCalismaYili.Text;
            string UserName = txtKullaniciTipi.Text;

            if (urunList == null || !urunList.Any())
            {
                var mesaj = new Tasarim1.BildirimMesaji("Lütfen Bir Excel Dosyası Yükleyin!");
                mesaj.Show();
                return;
            }


            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                //List<IUrun> urunList = GetUrunList(); // Müşteri listesini alacak bir metot varsayıyoruz
                //List<IUrun> rowsToProcess = GetCheckedRowsFromMusteriList(urunList);
            }
            catch { }
        } 
        #endregion

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
