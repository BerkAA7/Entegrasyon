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


namespace WPF_LoginForm.View
{

    public partial class LoginView : Window, ILoginView
    {

        private CancellationTokenSource cancellationTokenSource;

        //private DataTable dataTable;
        public static LoginView CurrentInstance { get; private set; }


        public LoginView()
        {
            InitializeComponent();
            VersionRun.Text = GetVersionNumber();//version numarası yazıldı
            CurrentInstance = this; // Mevcut örneği sakla


        }
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            SecimEkrani secimEkrani = new SecimEkrani();
            secimEkrani.Show();
            this.Close();
        }


        private List<DataRow> GetCheckedRows()
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
        }
        public string GetVersionNumber()//version numarasını aldık 
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        // Helper method to get DataGridCell from DataGrid and DataRowView
        private DataGridCell GetDataGridCell(DataGrid dataGrid, DataRowView row)
        {
            var container = dataGrid.ItemContainerGenerator.ContainerFromItem(row) as DataGridRow;
            if (container != null)
            {
                var column = dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Seç");
                if (column != null)
                {
                    var cellContent = column.GetCellContent(container);
                    return GetDataGridCell(cellContent);
                }
            }
            return null;
        }

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

        // Helper method to get DataGridCell from cell content
        private DataGridCell GetDataGridCell(FrameworkElement cellContent)
        {
            var parent = VisualTreeHelper.GetParent(cellContent);

            while (parent != null && !(parent is DataGridCell))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as DataGridCell;
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

        // Tüm satırları seç
        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (musteriList != null)
            {
                // Tüm kayıtların "Seç" özelliğini true yap
                foreach (var musteri in musteriList)
                {
                    musteri.Secim = true; // Seçim kolonundaki değeri true yap
                }

                // DataGrid'in güncellenmesini sağlamak için
                dataGrid.ItemsSource = musteriList; // DataGrid'e yeni listeyi ata
                dataGrid.Items.Refresh(); // DataGrid'i yenile
            }
        }

        // Tüm seçimleri kaldır
        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (musteriList != null)
            {
                // Tüm kayıtların "Seç" özelliğini false yap
                foreach (var musteri in musteriList)
                {
                    musteri.Secim = false; // Seçim kolonundaki değeri false yap
                }

                // DataGrid'in güncellenmesini sağlamak için
                dataGrid.ItemsSource = musteriList; // DataGrid'e yeni listeyi ata
                dataGrid.Items.Refresh(); // DataGrid'i yenile
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
        private string NormalizeSpaces(string input)
        {
            // Birden fazla ardışık boşluğu tek bir boşluk ile değiştirir
            return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");
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
            return System.Text.RegularExpressions.Regex.Replace(input, @"(?<=\S) (?=\S)", "");
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btnBilgileriAktar_Click(object sender, RoutedEventArgs e)
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
                List<IMusteri> musteriList = GetMusteriList(); // Müşteri listesini alacak bir metot varsayıyoruz
                List<IMusteri> rowsToProcess = GetCheckedRowsFromMusteriList(musteriList);

                if (!rowsToProcess.Any())
                {
                    var mesaj = new Tasarim1.BildirimMesaji("Lütfen Gönderilecek Satırları Seçin!");
                    mesaj.Show();
                    return;
                }

                rtbErrorMessages.Document.Blocks.Clear(); // Önceki hata mesajlarını temizle

                if (rowsToProcess.Count == 0)
                {
                    var mesaj = new Tasarim1.BildirimMesaji("Lütfen Gönderilecek Satırları Seçin!");
                    mesaj.Show();
                    return;
                }

                rtbErrorMessages.Document.Blocks.Clear(); // Önceki hata mesajlarını temizle
                foreach (var musteri in rowsToProcess)
                {
                    try
                    {
                        // CancellationToken'ın iptal edilip edilmediğini kontrol edin
                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        // Tüm müşteri bilgileri boş mu kontrol et
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
                            return; // Veri aktarımını durdur
                        }

                        // Hücrelerin arka plan rengini temizleyin
                        ClearRowCellBackground(musteri);

                        var customers = new List<Tasarim1.CustomerIntegration> { MapMusteriToCustomer(musteri) };
                        string xmlData = ConvertCustomersToXML(customers, UserName, panServisSifresi, firmaKodu, calismaYili, dist);

                        var response = await panServisLinki
                            .WithHeader("Authorization", $"Bearer {panServisSifresi}")
                            .WithHeader("Content-Type", "text/xml")
                            .PostStringAsync(xmlData);

                        string responseString = await response.GetStringAsync();
                        string errorMessage = ParseErrorMessageFromResponse(responseString);

                        string musteriKodu = musteri.MusteriKodu;

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            HighlightInvalidCells(musteri, Colors.LightCoral); // Hata durumunda LightCoral rengi
                            AppendErrorMessage($"Hata: {errorMessage}", musteriKodu);
                        }
                        else
                        {
                            HighlightSuccessfulCells(musteri, Colors.LightGreen); // Başarılı durumunda LightGreen rengi
                            AppendErrorMessage("Başarılı bir şekilde aktarım gerçekleşti", musteriKodu);
                        }
                    }
                    catch (FlurlHttpException ex)
                    {
                        string errorResponse = await ex.GetResponseStringAsync();
                        string errorMessage = ParseErrorMessage(errorResponse);
                        string musteriKodu = musteri.MusteriKodu;
                        HighlightInvalidCells(musteri, Colors.LightCoral);
                        AppendErrorMessage($"Hata: {ex.Message}\nYanıt: {errorMessage}", musteriKodu);
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
                        HighlightInvalidCells(musteri, Colors.LightCoral);
                        AppendErrorMessage($"Hata: {ex.Message}", musteriKodu);
                    }

                    // Sunucuyu aşırı yüklememek için bekleme süresi
                    await Task.Delay(1000); // Gerekirse bekleme süresini ayarlayın
                }
            }
            catch (OperationCanceledException)
            {
                var mesaj = new Tasarim1.BildirimMesaji("Aktarım durduruldu.");
                mesaj.Show();
            }
            catch (Exception ex)
            {
                // Genel hataları işleme
                AppendErrorMessage($"İstek gönderilirken bir hata oluştu: {ex.Message}", "");
            }
        }
        private void SetAllCheckBoxes(bool isChecked)
        {
            // Musteri listesinin her bir öğesi üzerinde gezinin
            foreach (var musteri in musteriList)
            {
                // Her müşteri için seçim durumunu ayarlayın
                musteri.Secim = isChecked;
            }

            // DataGrid'in güncellenmesini sağlamak için
            dataGrid.ItemsSource = musteriList; // DataGrid'e yeni listeyi ata
            dataGrid.Items.Refresh(); // DataGrid'i yenile
        }
        private CheckBox GetCheckBoxForRow(Musteri musteri)
        {
            int rowIndex = musteriList.IndexOf(musteri);

            if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count)
                return null;

            var rowContainer = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;

            if (rowContainer == null)
            {
                // Eğer satır henüz oluşturulmadıysa, zorunlu olarak oluşturulmasını sağlar
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                rowContainer = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }

            if (rowContainer != null)
            {
                // CheckBox'ın bulunduğu hücreyi al
                var cellContent = dataGrid.Columns[0].GetCellContent(rowContainer);
                var checkBox = cellContent as CheckBox;

                return checkBox;
            }

            return null;
        }
        private void ClearRowCellBackground(IMusteri musteri)
        {
            // IMusteri nesnesinin indexini bul
            int rowIndex = musteriList.IndexOf(musteri); // Eğer musteriList bir List<IMusteri> ise

            if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count)
                return; // Geçersiz index kontrolü

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                var cell = dataGrid.Columns[i].GetCellContent(dataGrid.Items[rowIndex]);
                if (cell != null)
                {
                    var dataGridCell = GetDataGridCell(cell);
                    if (dataGridCell != null)
                    {
                        dataGridCell.Background = Brushes.White; // Varsayılan arka plan rengi
                    }
                }
            }
        }


        //AKTARILAN HÜCRELERİ BOYAMA
        private void HighlightInvalidCells(IMusteri musteri, Color color)
        {
            // IMusteri nesnesinin indexini bul
            int rowIndex = musteriList.IndexOf(musteri); // Eğer musteriList bir List<IMusteri> ise

            if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count)
                return; // Geçersiz index kontrolü

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                var cell = dataGrid.Columns[i].GetCellContent(dataGrid.Items[rowIndex]);
                if (cell != null)
                {
                    var dataGridCell = GetDataGridCell(cell);
                    if (dataGridCell != null)
                    {
                        dataGridCell.Background = new SolidColorBrush(color); // Geçersiz hücre arka plan rengi
                    }
                }
            }
        }


        //private void HighlightSuccessfulCells(IMusteri musteri, System.Windows.Media.Color color)
        //{
        //    // IMusteri nesnesinin indexini bul
        //    int rowIndex = musteriList.IndexOf(musteri); // Eğer musteriList bir List<IMusteri> ise

        //    if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count)
        //        return; // Geçersiz index kontrolü

        //    for (int i = 0; i < dataGrid.Columns.Count; i++)
        //    {
        //        var cell = dataGrid.Columns[i].GetCellContent(dataGrid.Items[rowIndex]);
        //        if (cell != null)
        //        {
        //            var dataGridCell = GetDataGridCell(cell);
        //            if (dataGridCell != null)
        //            {
        //                dataGridCell.Background = new SolidColorBrush(color); // Başarılı hücre arka plan rengi
        //            }
        //        }
        //    }
        //}
        private void HighlightSuccessfulCells(IMusteri musteri, System.Windows.Media.Color color)
        {
            // IMusteri nesnesini dataGrid'deki öğelerle eşleştir
            foreach (var item in dataGrid.Items)
            {
                if (item == musteri)
                {
                    var row = dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (row != null)
                    {
                        for (int i = 0; i < dataGrid.Columns.Count; i++)
                        {
                            var cell = dataGrid.Columns[i].GetCellContent(row);
                            if (cell != null)
                            {
                                var dataGridCell = GetDataGridCell(cell);
                                if (dataGridCell != null)
                                {
                                    dataGridCell.Background = new SolidColorBrush(color); // Başarılı hücre arka plan rengi
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }
        private void AppendErrorMessage(string message, string MusteriKodu)
        {
            string fullMessage = $"MusteriKodu: {MusteriKodu} - {message}";
            Paragraph paragraph = new Paragraph(new Run(fullMessage));
            rtbErrorMessages.Document.Blocks.Add(paragraph);
            rtbErrorMessages.ScrollToEnd();
        }




        private string ParseErrorMessage(string response)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);
            var errorNode = xmlDoc.SelectSingleNode("//error");
            return errorNode?.InnerText ?? "Bilinmeyen bir hata oluştu.";
        }



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


        private string ParseErrorMessageFromResponse(string responseString)
        {
            try
            {
                var xDoc = XDocument.Parse(responseString);
                var errorElements = xDoc.Descendants().Where(e => e.Name.LocalName == "Hata");
                List<string> errorMessages = new List<string>();
                foreach (var errorElement in errorElements)
                {
                    errorMessages.Add(errorElement.Value);
                }
                return string.Join("\n", errorMessages);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during XML parsing
                return $"XML Yanıtı çözümleme hatası: {ex.Message}";
            }
        }

        private List<IMusteri> GetCheckedRowsFromMusteriList(List<IMusteri> musteriList)
        {
            if (musteriList == null || !musteriList.Any())
                return new List<IMusteri>(); // Eğer liste boşsa, boş liste döndür

            var seçiliSatırlar = new List<IMusteri>(); // Seçili müşteri listesini oluştur

            foreach (var musteri in musteriList)
            {
                // Müşteri nesnesinin secim özelliğine doğrudan erişim
                if (musteri.Secim) // secim özelliği true ise
                {
                    seçiliSatırlar.Add(musteri); // Seçili müşteri listesine ekle
                }
            }

            return seçiliSatırlar; // Seçili müşterileri döndür
        }
        private bool ContainsInvalidXmlChars(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            string pattern = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.IsMatch(text, pattern);
        }

        private bool CheckRequiredColumns(List<IMusteri> musteriList)
        {
            List<string> missingColumns = new List<string>();

            foreach (RequiredColumns col in Enum.GetValues(typeof(RequiredColumns)))
            {
                // Check if any item in the list has the property corresponding to the required column
                bool hasColumn = musteriList.Any(musteri =>
                {
                    var propertyInfo = musteri.GetType().GetProperty(col.ToString());
                    return propertyInfo != null && propertyInfo.GetValue(musteri) != null;
                });

                if (!hasColumn)
                {
                    missingColumns.Add(col.ToString());
                }
            }

            if (missingColumns.Count > 0)
            {
                // Message to show when required columns are missing
                string errorMessage = "Gerekli sütunlar eksik: " + string.Join(", ", missingColumns);
                // MessageBox.Show(errorMessage, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);

                // Create notification message
                var notificationMessage = new Tasarim1.BildirimMesaji(errorMessage);
                notificationMessage.Show();

                return false;
            }

            return true;
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

        private string ConvertCustomersToXML(List<Tasarim1.CustomerIntegration> customers, string UserName, string panServisSifresi, string firmaKodu, string calismaYili, string dist)
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

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dataGrid_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }
    }
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


}