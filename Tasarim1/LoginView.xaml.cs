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


namespace WPF_LoginForm.View
{

    public partial class LoginView : Window, ILoginView
    {
        ////*        private readonly ILoginView _loginView;
        //private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KolonIsterlerData.txt");
        //public KolonIsterler(ILoginView loginView)
        //{
        //    InitializeComponent();
        //    _loginView = loginView;
        //    musteriList = _loginView.GetMusteriList();
        //    //LoadDataFromFile(filePath, musteri);

        //}/
        
        private CancellationTokenSource cancellationTokenSource;
        public static LoginView CurrentInstance { get; private set; }
        ExcelHelper excelHelper = new ExcelHelper();
        DataGridHelpers dataGridHelpers = new DataGridHelpers();
        ErrorHelpers errorHelpers = new ErrorHelpers();

        public LoginView()
        {
            InitializeComponent();
            VersionRun.Text = GetVersionNumber();//version numarası yazıldı
            CurrentInstance = this; // Mevcut örneği sakla


        }
        public string GetVersionNumber()//version numarasını aldık 
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        } 

        /*private List<DataRow> GetCheckedRows()
        {
            var checkedRows = new List<DataRow>();

            // DataGrid içerisindeki tüm satırlara erişin
            foreach (var item in dataGrid.Items)
            {
                var row = item as DataRowView;
                if (row != null)
                {
                    // CheckBox'ın işaretli olup olmadığını kontrol edin
                    var cell = GetDataGridCell(dataGrid, row);
                    if (cell != null)
                    {
                        var checkBox = GetVisualChild<CheckBox>(cell);
                        if (checkBox != null && checkBox.IsChecked == true)
                        {
                            checkedRows.Add(row.Row);
                        }
                    }
                }
            }

            return checkedRows;
        }*/

        // Helper method to get VisualChild of a given type
        private T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    foundChild = (T)child;
                    break;
                }
                else
                {
                    foundChild = GetVisualChild<T>(child);
                    if (foundChild != null)
                    {
                        break;
                    }
                }
            }

            return foundChild;
        }

        #region CHECK METHODS
        /*GENERİC*/
        private void ToggleSelection<T>(List<T> itemList, bool isChecked) where T : ISelectable
        {
            if (itemList != null)
            {
                // Tüm kayıtların "Seç" özelliğini ayarla
                foreach (var item in itemList)
                {
                    item.Secim = isChecked; // Seçim kolonundaki değeri ayarla
                }

                // DataGrid'in güncellenmesini sağlamak için
                dataGrid.ItemsSource = itemList; // DataGrid'e yeni listeyi ata
                dataGrid.Items.Refresh(); // DataGrid'i yenile
            }
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSelection(musteriList, true);
        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSelection(musteriList, false);
        }

        
        #endregion

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private List<IMusteri> musteriList = new List<IMusteri>();

        #region CLICK METHODS
        private void btnExcelYükle_Click(object sender, RoutedEventArgs e)
        {

            excelHelper.BtnExcelYukle<IMusteri>(
                filePath => excelHelper.ReadExcelFile<IMusteri>(filePath), // Excel dosyasını okuma fonksiyonu
                musteri => this.MusteriAL(musteri) // Alınacak fonksiyon
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
                await MusteriBilgileriAktarAsync();  // Bu metot uzun işlemleri yapacak
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


        private void btnKolonSabitleriniDegistir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // KolonIsterler penceresinin zaten açık olup olmadığını kontrol et
                var existingWindow = Application.Current.Windows.OfType<KolonIsterler>().FirstOrDefault();
                if (existingWindow != null)
                {
                    // Pencere zaten açık, hata mesajı göster
                    var mesaj = new Tasarim1.BildirimMesaji("Pencere zaten açık.");
                    mesaj.Show();
                }
                else
                {
                    // Pencere açık değil, yeni bir pencere oluştur ve göster
                    //LoginView loginView = new LoginView();
                    KolonIsterler ekran = new KolonIsterler(CurrentInstance);
                    ekran.Show();
                }
            }
            catch (Exception ex)
            {
                var mesaj = new Tasarim1.BildirimMesaji($"Bilinmeyen bir hata oluştu: {ex.Message}");
                mesaj.Show();
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            SecimEkrani secimEkrani = new SecimEkrani();
            secimEkrani.Show();
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        //private async void btnExcelYükle_Click(object sender, RoutedEventArgs e)
        //{
        //    var openFileDialog = new OpenFileDialog
        //    {
        //        Filter = "Excel Dosyaları|*.xls;*.xlsx;*.xlsm"
        //    };

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        string dosyaAdı = openFileDialog.FileName;

        //        // Bekleme ekranını oluştur ve göster (en başta)
        //        var beklemeEkrani = new BeklemeEkrani();
        //        beklemeEkrani.Topmost = true;
        //        beklemeEkrani.Show();
        //        await Task.Delay(3000);

        //        Excel.Application excelUygulama = null;
        //        Excel.Workbook çalışmaKitabı = null;
        //        Excel.Worksheet çalışmaSayfası = null;

        //        try
        //        {
        //            // Text dosyasından verileri oku
        //            var kolonIsterlerData = File.ReadAllLines("KolonIsterlerData.txt")
        //                .Select(line => line.Split('='))
        //                .ToDictionary(parts => parts[0], parts => parts.Length > 1 ? parts[1] : string.Empty);

        //            excelUygulama = new Excel.Application();
        //            çalışmaKitabı = excelUygulama.Workbooks.Open(dosyaAdı);
        //            çalışmaSayfası = çalışmaKitabı.Worksheets[1];

        //            dataTable?.Clear();
        //            dataTable = new DataTable();

        //            int sütunSayısı = çalışmaSayfası.UsedRange.Columns.Count;
        //            int satırSayısı = çalışmaSayfası.UsedRange.Rows.Count;

        //            // Sütun isimlerini tek seferde al
        //            var sütunAdları = new string[sütunSayısı];
        //            for (int sütun = 1; sütun <= sütunSayısı; sütun++)
        //            {
        //                Excel.Range başlıkHücresi = çalışmaSayfası.Cells[1, sütun];
        //                string sütunAdı = başlıkHücresi.Value2?.ToString().Replace(" ", "") ?? "";
        //                sütunAdı = ReplaceTurkishCharacters(sütunAdı);
        //                sütunAdları[sütun - 1] = sütunAdı;
        //                dataTable.Columns.Add(sütunAdı);
        //            }

        //            // Satırları ve hücreleri işleyerek dataTable'ı doldur
        //            object[,] hücreVerileri = çalışmaSayfası.UsedRange.Value2;
        //            for (int satır = 2; satır <= satırSayısı; satır++)
        //            {
        //                DataRow yeniSatır = dataTable.NewRow();
        //                for (int sütun = 1; sütun <= sütunSayısı; sütun++)
        //                {
        //                    string hücreVerisi = hücreVerileri[satır, sütun]?.ToString() ?? "";

        //                    if (sütunAdları[sütun - 1] == "Adres")
        //                    {
        //                        hücreVerisi = hücreVerisi.Replace("-", "").Replace(".", "");
        //                        hücreVerisi = NormalizeSpaces(hücreVerisi);
        //                    }
        //                    else if (sütunAdları[sütun - 1] == "OdemeTipi")
        //                    {
        //                        hücreVerisi = RemoveAllSpaces(hücreVerisi);
        //                    }
        //                    else if (sütunAdları[sütun - 1] == "KisaAdi" && hücreVerisi.Length > 30)
        //                    {
        //                        hücreVerisi = hücreVerisi.Substring(0, 30);
        //                        hücreVerileri[satır, sütun] = hücreVerisi; // Değişikliği Excel'e kaydet
        //                    }

        //                    yeniSatır[sütun - 1] = hücreVerisi;
        //                }
        //                dataTable.Rows.Add(yeniSatır);
        //            }

        //            // Boş hücreleri doldur
        //            foreach (DataRow row in dataTable.Rows)
        //            {
        //                foreach (DataColumn column in dataTable.Columns)
        //                {
        //                    if (string.IsNullOrWhiteSpace(row[column].ToString()) && kolonIsterlerData.TryGetValue(column.ColumnName, out var value))
        //                    {
        //                        row[column] = value;
        //                    }
        //                }
        //            }

        //            çalışmaKitabı.Save();
        //            dataGrid.ItemsSource = dataTable.DefaultView;
        //            dataGrid.Items.Refresh();

        //            // DataGrid sütunlarına stil uygula
        //            foreach (var column in dataGrid.Columns)
        //            {
        //                if (new[] { "DURUM", "MusteriKodu", "Unvan", "IlgiliKisi", "MusteriGrubu", "MusteriEkGrubu", "OdemeTipi", "KisaAdi", "VergiTipi" }
        //                    .Contains(column.Header.ToString()))
        //                {
        //                    var headerStyle = new Style(typeof
        //
        //                    ataGridColumnHeader));
        //                    headerStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, Brushes.Red));
        //                    column.HeaderStyle = headerStyle;
        //                }
        //            }

        //            var mesaj1 = new Tasarim1.BildirimMesaji("Excel Dosyası Başarıyla Yüklendi!");
        //            mesaj1.Show();
        //        }
        //        catch (Exception ex)
        //        {
        //            var mesaj = new Tasarim1.BildirimMesaji($"Bir hata oluştu: {ex.Message}");
        //            mesaj.Show();
        //        }
        //        finally
        //        {
        //            // Bekleme ekranını kapat
        //            beklemeEkrani.Close();

        //            if (çalışmaKitabı != null)
        //            {
        //                çalışmaKitabı.Close(false);
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(çalışmaKitabı);
        //            }
        //            if (çalışmaSayfası != null)
        //            {
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(çalışmaSayfası);
        //            }
        //            if (excelUygulama != null)
        //            {
        //                excelUygulama.Quit();
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelUygulama);
        //            }

        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //        }
        //    }
        //}
        // Boşlukları normalleştiren yardımcı yöntem


        #region MUSTERIOZEL
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

        private Tasarim1.CustomerIntegration MapMusteriToCustomer(IMusteri musteri)
        {
            var returned = new Tasarim1.CustomerIntegration
            {
                Durum = (musteri.Durum != null && Enum.TryParse(musteri.Durum.ToString(), true, out DurumEnum durum)) ? (int?)durum : (int?)null,
                ErpKod2 = musteri.MusteriKodu,
                Unvan = musteri.Unvan,
                IlgiliKisi = musteri.IlgiliKisi,
                Adres1 = musteri.Adres.Replace("-", string.Empty),
                Adres2 = "",
                MerkezIlTextKod = musteri.Sehir,
                Ilce = musteri.Ilce,
                TCKimlikNo = musteri.TcNo,
                CepTelNo = musteri.Telefon,
                VD = musteri.VergiDairesi,
                VN = musteri.VergiNumarasi,
                MusteriGrupTextKod = musteri.MusteriGrubu,
                MusteriEkGrupTextKod = musteri.MusteriEkGrubu,
                OdemeTipi = (musteri.OdemeTipi != null && Enum.TryParse(musteri.OdemeTipi.ToString(), true, out OdemeTipiEnum odemeTipiEnum)) ? (int?)odemeTipiEnum : (int?)null,
                KisaAd = musteri.KisaAdi,
                KdvMuaf = (musteri.VergiTipi != null && Enum.TryParse(musteri.VergiTipi.ToString(), true, out VergiTipiEnum vergiTipiEnum)) ? (int?)vergiTipiEnum : (int?)null,
                KoordinatX = (musteri.KoordinatX != null) ? Convert.ToDecimal(musteri.KoordinatX) : (decimal?)null,
                KoordinatY = (musteri.KoordinatY != null) ? Convert.ToDecimal(musteri.KoordinatY) : (decimal?)null,
                VadeGun = (musteri.VadeGunu != null) ? Convert.ToInt32(musteri.VadeGunu) : (int?)null,
                IskontoOran = (musteri.Iskonto != null) ? Convert.ToDecimal(musteri.Iskonto) : (decimal?)null
            };
            if (!string.IsNullOrWhiteSpace(returned.Adres1) && returned.Adres1.Length > 45)
            {
                // Adres1'in ilk 45 karakteri
                returned.Adres2 = returned.Adres1.Substring(45); // 45. karakterden itibaren geri kalanlar Adres2
                returned.Adres1 = returned.Adres1.Substring(0, 45); // İlk 45 karakter Adres1
            }
            return returned;

        }

        private async Task MusteriBilgileriAktarAsync()
        {
            string panServisLinki = txtLink.Text;
            string panServisSifresi = txtSifre.Text;
            string dist = txtDist.Text;
            string firmaKodu = txtFirmaKodu.Text;
            string calismaYili = txtCalismaYili.Text;
            string UserName = txtKullaniciTipi.Text;

            if (musteriList == null || !musteriList.Any())
            {
                var mesaj = new Tasarim1.BildirimMesaji("Lütfen Bir Excel Dosyası Yükleyin!");
                mesaj.Show();
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                List<IMusteri> musteriList = GetMusteriList();
                List<IMusteri> rowsToProcess = ExcelHelper.GetCheckedRowsFromList(musteriList);

                if (!rowsToProcess.Any())
                {
                    var mesaj = new Tasarim1.BildirimMesaji("Lütfen Gönderilecek Satırları Seçin!");
                    mesaj.Show();
                    return;
                }

                rtbErrorMessages.Document.Blocks.Clear();

                foreach (var musteri in rowsToProcess)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        if (string.IsNullOrEmpty(musteri.Durum) ||
                            string.IsNullOrEmpty(musteri.MusteriKodu) ||
                            string.IsNullOrEmpty(musteri.Unvan) ||
                            string.IsNullOrEmpty(musteri.IlgiliKisi) ||
                            string.IsNullOrEmpty(musteri.MusteriGrubu) ||
                            string.IsNullOrEmpty(musteri.MusteriEkGrubu) ||
                            string.IsNullOrEmpty(musteri.OdemeTipi) ||
                            string.IsNullOrEmpty(musteri.KisaAdi) ||
                            string.IsNullOrEmpty(musteri.VergiTipi))
                        {
                            var mesaj = new Tasarim1.BildirimMesaji("Seçili satırda gerekli hücreler boş. Veri aktarımı durduruluyor.");
                            mesaj.Show();
                            return;
                        }

                        dataGridHelpers.ClearRowCellBackground(musteri, musteriList, dataGrid);

                        var customers = new List<Tasarim1.CustomerIntegration> { MapMusteriToCustomer(musteri) };
                        string xmlData = ConvertCustomersToXML(customers, UserName, panServisSifresi, firmaKodu, calismaYili, dist);

                        var response = await panServisLinki
                            .WithHeader("Authorization", $"Bearer {panServisSifresi}")
                            .WithHeader("Content-Type", "text/xml")
                            .PostStringAsync(xmlData);

                        string responseString = await response.GetStringAsync();
                        string errorMessage = errorHelpers.ParseErrorMessageFromResponse(responseString);

                        string musteriKodu = musteri.MusteriKodu;

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            dataGridHelpers.HighlightInvalidCells(musteri, musteriList, dataGrid, Colors.LightCoral);
                            errorHelpers.AppendErrorMessage($"Hata: {errorMessage}", musteriKodu, 0);
                        }
                        else
                        {
                            DataGridHelpers.HighlightSuccessfulCells(musteri, dataGrid, Colors.LightGreen);
                            errorHelpers.AppendErrorMessage("Başarılı bir şekilde aktarım gerçekleşti", musteriKodu,0);
                        }
                    }
                    catch (FlurlHttpException ex)
                    {
                        string errorResponse = await ex.GetResponseStringAsync();
                        string errorMessage = errorHelpers.ParseErrorMessage(errorResponse);
                        string musteriKodu = musteri.MusteriKodu;
                        dataGridHelpers.HighlightInvalidCells(musteri, musteriList, dataGrid, Colors.LightCoral);
                        errorHelpers.AppendErrorMessage($"Hata: {ex.Message}\nYanıt: {errorMessage}", musteriKodu,0);
                    }
                    catch (System.Security.SecurityException ex)
                    {
                        var mesaj = new Tasarim1.BildirimMesaji("Gerekli izinlere sahip olmadığınız için işlemi tamamlayamadık.");
                        mesaj.Show();
                        return;
                    }
                    catch (Exception ex)
                    {
                        string musteriKodu = musteri.MusteriKodu;
                        dataGridHelpers.HighlightInvalidCells(musteri, musteriList, dataGrid, Colors.LightCoral);
                        errorHelpers.AppendErrorMessage($"Hata: {ex.Message}", musteriKodu, 0);
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
                errorHelpers.AppendErrorMessage($"İstek gönderilirken bir hata oluştu: {ex.Message}", "", 0);
            }
        }

        public string ConvertCustomersToXML(List<Tasarim1.CustomerIntegration> customers, string UserName, string panServisSifresi, string firmaKodu, string calismaYili, string dist)
        {
            if (customers == null || customers.Count == 0)
                throw new InvalidOperationException("Customer list is empty or invalid.");

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
                    xmlWriter.WriteElementString("lngDistributorKod", dist);
                    //////////////////
                    xmlWriter.WriteStartElement("objPanIntEntityList"); // Start objPanIntEntityList

                    xmlWriter.WriteStartElement("Musteriler");

                    foreach (var customer in customers)
                    {
                        xmlWriter.WriteStartElement("clsMusteriIntegration");
                        // xmlWriter.WriteElementString("GrupKod", "99");
                        // xmlWriter.WriteElementString("EkGrupKod", "99");
                        xmlWriter.WriteElementString("Referans", $"{dist}-{customer.ErpKod2}");
                        xmlWriter.WriteElementString("DistKod", dist);

                        foreach (var prop in customer.GetType().GetProperties())
                        {
                            var value = prop.GetValue(customer);

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

                    xmlWriter.WriteEndElement(); // Close Musteriler

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

        private void dataGrid_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        public void RbtErrorMessageErrorHelpers(Paragraph paragraph)
        {
            rtbErrorMessages.Document.Blocks.Add(paragraph);
            rtbErrorMessages.ScrollToEnd();
        }

        #region 0 REFERANSLI KODLAR
        private string NormalizeSpaces(string input)
        {
            // Birden fazla ardışık boşluğu tek bir boşluk ile değiştirir
            return Regex.Replace(input, @"\s+", " ");
        }
        // Tüm boşlukları kaldıran yardımcı yöntem
        private string RemoveAllSpaces(string input)
        {
            // Tüm boşlukları kaldırır
            return input.Replace(" ", string.Empty);
        }
        // Tek harfli boşlukları kaldıran yardımcı yöntem
        private string RemoveSingleCharacterSpaces(string input)
        {
            // Tek harfli boşlukları kaldırmak için regex kullanabiliriz
            return Regex.Replace(input, @"(?<=\S) (?=\S)", "");
        }
        private string ReplaceTurkishCharacters(string text)
        {
            return text
                .Trim()
                .ToUpper()
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
        private bool ContainsInvalidXmlChars(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            string pattern = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.IsMatch(text, pattern);
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }


        #region GereksizCheckBox
        //private void SetAllCheckBoxes(bool isChecked)
        //{
        //    // Musteri listesinin her bir öğesi üzerinde gezinin
        //    foreach (var musteri in musteriList)
        //    {
        //        // Her müşteri için seçim durumunu ayarlayın
        //        musteri.Secim = isChecked;
        //    }

        //    // DataGrid'in güncellenmesini sağlamak için
        //    dataGrid.ItemsSource = musteriList; // DataGrid'e yeni listeyi ata
        //    dataGrid.Items.Refresh(); // DataGrid'i yenile
        //}
        /*GENERİC*/
        //private CheckBox GetCheckBoxForRow<T>(List<T> itemList, T item) where T : IMusteri, IUrun
        //{
        //    int rowIndex = itemList.IndexOf(item);

        //    if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count)
        //        return null;

        //    var rowContainer = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;

        //    if (rowContainer == null)
        //    {
        //        // Eğer satır henüz oluşturulmadıysa, zorunlu olarak oluşturulmasını sağlar
        //        dataGrid.UpdateLayout();
        //        dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
        //        rowContainer = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
        //    }

        //    if (rowContainer != null)
        //    {
        //        // CheckBox'ın bulunduğu hücreyi al
        //        var cellContent = dataGrid.Columns[0].GetCellContent(rowContainer);
        //        var checkBox = cellContent as CheckBox;

        //        return checkBox;
        //    }

        //    return null;
        //}

        #endregion
        private (bool hasExceptions, string exceptionMessages) ParseResponseForExceptions(string response)
        {
            var exceptionMessages = new List<string>();

            var startIndex = 0;
            while ((startIndex = response.IndexOf("@Message       :", startIndex)) != -1)
            {
                startIndex += "@Message       :".Length;
                var endIndex = response.IndexOf("@", startIndex);
                if (endIndex == -1) endIndex = response.Length;

                var message = response.Substring(startIndex, endIndex - startIndex).Trim();
                exceptionMessages.Add(message);

                startIndex = endIndex;
            }

            return (exceptionMessages.Count > 0, string.Join("\n", exceptionMessages));
        }

        //public static bool CheckRequiredColumns<T, TEnum>(List<T> itemList) where TEnum : Enum
        //{
        //    List<string> missingColumns = new List<string>();

        //    foreach (TEnum col in Enum.GetValues(typeof(TEnum)))
        //    {
        //        // Check if any item in the list has the property corresponding to the required column
        //        bool hasColumn = itemList.Any(item =>
        //        {
        //            var propertyInfo = item.GetType().GetProperty(col.ToString());
        //            return propertyInfo != null && propertyInfo.GetValue(item) != null;
        //        });

        //        if (!hasColumn)
        //        {
        //            missingColumns.Add(col.ToString());
        //        }
        //    }

        //    if (missingColumns.Count > 0)
        //    {
        //        // Message to show when required columns are missing
        //        string errorMessage = "Gerekli sütunlar eksik: " + string.Join(", ", missingColumns);
        //        // MessageBox.Show(errorMessage, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);

        //        // Create notification message
        //        var notificationMessage = new Tasarim1.BildirimMesaji(errorMessage);
        //        notificationMessage.Show();

        //        return false;
        //    }

        //    return true;
        //}


        #endregion

    }
    #region ENUMS
    public enum RequiredColumns//zorunlu alanlar
    {
        Durum,
        MusteriKodu,
        Unvan,
        IlgiliKisi,
        MusteriGrubu,
        MusteriEkGrubu,
        OdemeTipi,
        KisaAdi,
        VergiTipi
    }


    public enum VergiTipiEnum
    {
        KDVdenMuaf = 1,
        GercekKisi = 2,
        TuzelKisi = 3,
        YabanciUyruk = 4
    }


    public enum OdemeTipiEnum
    {
        Nakit = 0,
        Cek = 1,
        Senet = 2,
        KrediKarti = 3,
        AcikHesap = 4,
        TicariKart = 5,
        DBS = 6,
        HavaleEFT = 7
    }

    public enum DurumEnum
    {
        Aktif = 0,
        Pasif = 1,
        Iptal = 2,
        Silindi = 3,
        PotansiyelPasif = 4,
        PotansiyelAktif = 5
    }
    #endregion


}