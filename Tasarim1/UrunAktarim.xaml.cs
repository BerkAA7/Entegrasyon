using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Documents;
using System.Globalization;
using System.Linq;
using Flurl.Http;
using System.Threading;
using System.Windows.Media;
using System.Text;
using OfficeOpenXml;
using System.Reflection;
using ExcelToPanorama;
using ExcelToPanorama.Interface;
using ClosedXML.Excel;
using Tasarim1;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using ExcelToPanorama.Class;
using ExcelToPanorama.Helpers;
using Microsoft.Win32;
using WPF_LoginForm.View;



namespace ExcelToPanorama
{
    public partial class UrunAktarim : Window

    {
    
        public UrunAktarim()
        {
            InitializeComponent();
            VersionRun.Text = GetVersionNumber();//version numarası yazıldı
        }
        public string GetVersionNumber()//version numarasını aldık 
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)//ekran küçültme
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        ExcelHelper excelHelper = new ExcelHelper();
        DataGridHelpers dataGridHelpers = new DataGridHelpers();
        ErrorHelpers errorHelpers = new ErrorHelpers();
        


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

        /* public List<IUrun> ReadExcelFile(string filePath)
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
         }*/




        #endregion
        #region CLICKMETHODS
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            SecimEkrani secimEkrani = new SecimEkrani();
            secimEkrani.Show();
            this.Close();
        }
        private void btnExcelYükle_Click(object sender, RoutedEventArgs e)
        {
            var excelHelper = new ExcelHelper();
            excelHelper.BtnExcelYukle<IUrun>(
                filePath => excelHelper.ReadExcelFile<IUrun>(filePath), // Excel dosyasını okuma fonksiyonu
                urun => this.Urunal(urun) // Alınacak fonksiyon
            );
        }

        private async void btnBilgileriAktar_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            BeklemeEkrani beklemeEkrani = new BeklemeEkrani
            {
                Owner = Window.GetWindow(this), // Ana pencereyi owner olarak ayarla
                WindowStartupLocation = WindowStartupLocation.CenterOwner // Ortalanmış açılması için
            };

            // Yükleniyor ekranını göster
            beklemeEkrani.Show();

            // Ana pencereyi devre dışı bırak
            this.IsEnabled = false;

            // Butonu disable yap
            btnLogin.IsEnabled = false;

            try
            {
                // Uzun süren işleminiz burada çalışıyor
                await UrunBilgileriAktarAsync();  // Bu metot uzun işlemleri yapacak
            }
            finally
            {
                // İşlem tamamlandığında bekleme ekranını kapat
                beklemeEkrani.Close();

                // Ana pencereyi tekrar aktif yap
                this.IsEnabled = true;

                // Butonu tekrar aktif yap
                btnLogin.IsEnabled = true;
            }
        } 
        #endregion
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

        #region KOLON SABİTLERİNİ DEĞİŞTİR BUTONU
        private void btnKolonSabitleriniDegistir_Click(object sender, RoutedEventArgs e)
        {
            KolonIsterlerUrun ekran = new KolonIsterlerUrun();
            ekran.Show();
        } 
        #endregion

       
        #region URUN OZEL
        public void Urunal(List<IUrun> GuncellenmisUrunList)
        {
            urunList = GuncellenmisUrunList;
            dataGrid.ItemsSource = urunList;
            dataGrid.Items.Refresh(); // DataGrid'i yenile

            //return musteriList; // Global listeyi döndürme
        }
        public List<IUrun> GetUrunList()
        {
            return urunList; // Global listeyi döndürme
        }
        private Tasarim1.ProductIntegration MapUrunToProduct(IUrun urun)
        {
            var returned = new Tasarim1.ProductIntegration
            {
                TextKod = urun.UrunKodu,
                Ad = urun.UrunAdi,
                KisaAd = urun.UrunKisaAdi,
                UrunGrupKod = urun.UrunGrupKodu,
                UrunEkGrupKod = urun.UrunEkGrupKodu,
                Hiyerarsi1TextKod = urun.SeviyeliGrup1,
                UreticiKodu = urun.UreticiKodu,
                Birim1 = urun.Birim1,
                Barkod1 = urun.Barkod1,
                Birim2 = urun.Birim2,
                Barkod2 = urun.Barkod2,
                Cevrim2 = (urun.BirimCarpani2 != null) ? Convert.ToDecimal(urun.BirimCarpani2) : (decimal?)null,
                Birim3 = urun.Birim3,
                Barkod3 = urun.Barkod3,
                Cevrim3 = (urun.BirimCarpani3 != null) ? Convert.ToDecimal(urun.BirimCarpani3) : (decimal?)null,
                KdvOran = (urun.SatisKDVOrani != null) ? Convert.ToDecimal(urun.SatisKDVOrani) : (decimal?)null,
                UrunTip = (urun.UrunTip != null && Enum.TryParse(urun.UrunTip.ToString(), true, out UrunTipiEnum urunTipiEnum)) ? (byte?)urunTipiEnum : (byte?)0,
                Kdvoranalis = (urun.AlisKDVOrani != null) ? Convert.ToDecimal(urun.AlisKDVOrani) : (decimal?)null,
                UrunAciklama = urun.UrunAciklama
                ,
            };
            
            return returned;

        }
        private async Task UrunBilgileriAktarAsync()
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
                List<IUrun> urunList = GetUrunList();
                List<IUrun> rowsToProcess = ExcelHelper.GetCheckedRowsFromList(urunList);

                if (!rowsToProcess.Any())
                {
                    var mesaj = new Tasarim1.BildirimMesaji("Lütfen Gönderilecek Satırları Seçin!");
                    mesaj.Show();
                    return;
                }

                rtbErrorMessages.Document.Blocks.Clear();

                foreach (var urun in rowsToProcess)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        if (string.IsNullOrEmpty(urun.UrunKodu) ||
                            string.IsNullOrEmpty(urun.UrunAdi) ||
                            string.IsNullOrEmpty(urun.UrunGrupKodu) ||
                            string.IsNullOrEmpty(urun.UrunEkGrupKodu) ||
                            string.IsNullOrEmpty(urun.SeviyeliGrup1) ||
                            string.IsNullOrEmpty(urun.UreticiKodu) ||
                            string.IsNullOrEmpty(urun.Birim1) ||
                            string.IsNullOrEmpty(urun.SatisKDVOrani) ||
                            string.IsNullOrEmpty(urun.AlisKDVOrani))
                        {
                            var mesaj = new Tasarim1.BildirimMesaji("Seçili satırda gerekli hücreler boş. Veri aktarımı durduruluyor.");
                            mesaj.Show();
                            return;
                        }

                        dataGridHelpers.ClearRowCellBackground(urun, urunList, dataGrid);

                        var products = new List<Tasarim1.ProductIntegration> { MapUrunToProduct(urun) };
                        string xmlData = ConvertProductsToXML(products, UserName, panServisSifresi, firmaKodu, calismaYili);

                        var response = await panServisLinki
                            .WithHeader("Authorization", $"Bearer {panServisSifresi}")
                            .WithHeader("Content-Type", "text/xml")
                            .PostStringAsync(xmlData);

                        string responseString = await response.GetStringAsync();
                        string errorMessage = errorHelpers.ParseErrorMessageFromResponse(responseString);

                        string urunKodu = urun.UrunKodu;

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            dataGridHelpers.HighlightInvalidCells(urun, urunList, dataGrid, Colors.LightCoral);
                            errorHelpers.AppendErrorMessage($"Hata: {errorMessage}", urunKodu, 1);
                        }
                        else
                        {
                            DataGridHelpers.HighlightSuccessfulCells(urun, dataGrid, Colors.LightGreen);
                            errorHelpers.AppendErrorMessage("Başarılı bir şekilde aktarım gerçekleşti", urunKodu, 1);
                        }
                    }
                    catch (FlurlHttpException ex)
                    {
                        string errorResponse = await ex.GetResponseStringAsync();
                        string errorMessage = errorHelpers.ParseErrorMessage(errorResponse);
                        string urunKodu = urun.UrunKodu;
                        dataGridHelpers.HighlightInvalidCells(urun, urunList, dataGrid, Colors.LightCoral);
                        errorHelpers.AppendErrorMessage($"Hata: {ex.Message}\nYanıt: {errorMessage}", urunKodu, 1);
                    }
                    catch (System.Security.SecurityException ex)
                    {
                        var mesaj = new Tasarim1.BildirimMesaji("Gerekli izinlere sahip olmadığınız için işlemi tamamlayamadık.");
                        mesaj.Show();
                        return;
                    }
                    catch (Exception ex)
                    {
                        string urunKodu = urun.UrunKodu;
                        dataGridHelpers.HighlightInvalidCells(urun, urunList, dataGrid, Colors.LightCoral);
                        errorHelpers.AppendErrorMessage($"Hata: {ex.Message}", urunKodu, 1);
                    }

                    await Task.Delay(1000);
                }
            }
            catch (OperationCanceledException)
            {
                var mesaj = new Tasarim1.BildirimMesaji("Aktarım durduruldu.");
                mesaj.Show();
            }
            catch (Exception ex)
            {
                errorHelpers.AppendErrorMessage($"İstek gönderilirken bir hata oluştu: {ex.Message}", "", 1);
            }
        }
        public string ConvertProductsToXML(List<Tasarim1.ProductIntegration> products, string UserName, string panServisSifresi, string firmaKodu, string calismaYili)
        {
            if (products == null || products.Count == 0)
                throw new InvalidOperationException("Products list is empty or invalid.");

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memoryStream, settings))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
                    xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    xmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
                    xmlWriter.WriteStartElement("soap", "Body", null);

                    xmlWriter.WriteStartElement("IntegrationSendEntitySetWithLogin", "http://integration.univera.com.tr");

                    xmlWriter.WriteElementString("strUserName", UserName);
                    xmlWriter.WriteElementString("strPassWord", panServisSifresi);
                    xmlWriter.WriteElementString("bytFirmaKod", firmaKodu);
                    xmlWriter.WriteElementString("lngCalismaYili", calismaYili);
                    //////////////////
                    xmlWriter.WriteStartElement("objPanIntEntityList"); // Start objPanIntEntityList

                    xmlWriter.WriteStartElement("Urunler");

                    foreach (var product in products)
                    {
                        xmlWriter.WriteStartElement("clsUrunIntegration");
                        // xmlWriter.WriteElementString("GrupKod", "99");
                        // xmlWriter.WriteElementString("EkGrupKod", "99");
                        xmlWriter.WriteElementString("Referans", $"{product.TextKod}");

                        foreach (var prop in product.GetType().GetProperties())
                        {
                            var value = prop.GetValue(product);

                            if (value == null)
                            {
                                if (prop.PropertyType == typeof(decimal?) || prop.PropertyType == typeof(decimal))
                                {
                                    xmlWriter.WriteElementString(prop.Name, "0");
                                }
                                else if (prop.PropertyType == typeof(int?) || prop.PropertyType == typeof(int))
                                {
                                    xmlWriter.WriteElementString(prop.Name, "0");
                                }
                                else
                                {
                                    xmlWriter.WriteElementString(prop.Name, string.Empty);
                                }
                            }
                            else
                            {
                                string stringValue = value.ToString();

                                if (prop.PropertyType == typeof(decimal?) || prop.PropertyType == typeof(decimal))
                                {
                                    stringValue = ((decimal?)value).GetValueOrDefault().ToString("G", CultureInfo.InvariantCulture);
                                }
                                else if (prop.PropertyType == typeof(int?) || prop.PropertyType == typeof(int))
                                {
                                    stringValue = ((int?)value).GetValueOrDefault().ToString();
                                }

                                xmlWriter.WriteElementString(prop.Name, stringValue);

                            }
                        }

                        xmlWriter.WriteEndElement(); // Close clsMusteriIntegration
                    }

                    xmlWriter.WriteEndElement(); // Close Urunler

                    xmlWriter.WriteElementString("SatirBazliTransaction", "true");
                    xmlWriter.WriteElementString("LogKategori", "0");

                    xmlWriter.WriteStartElement("IntegrationGorevSonucTip");
                    xmlWriter.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
                    xmlWriter.WriteEndElement(); // Close IntegrationGorevSonucTip

                    xmlWriter.WriteElementString("SCCall", "false");
                    xmlWriter.WriteElementString("ReturnLoglist", "true");

                    xmlWriter.WriteEndElement(); // Close objPanIntEntityList
                    xmlWriter.WriteEndElement(); // Close IntegrationSendEntitySetWithLogin
                    xmlWriter.WriteEndElement(); // Close soap:Body
                    xmlWriter.WriteEndElement(); // Close soap:Envelope

                    xmlWriter.WriteEndDocument();
                }

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        #endregion

        #region ENUMS
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
        public enum UrunTipiEnum
        {
            Normal = 0,
            KarmaKoli = 1,
            POP = 2,
            Bedelsiz = 3,
            Depozito = 4,
            Demirbas = 5,
            EIskontoHediye = 6,
            PaketUrun = 7,
            Hizmet = 8,
            Sayac = 9,
            Stand = 10,
            Palye = 11
        } 
        #endregion

    }
    }
