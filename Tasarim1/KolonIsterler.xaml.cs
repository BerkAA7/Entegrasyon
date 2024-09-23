using ExcelToPanorama;
using ExcelToPanorama.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tasarim1;

namespace WPF_LoginForm.View
{
    public partial class KolonIsterler : Window
    {
        private readonly ILoginView _loginView;
        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KolonIsterlerData.txt");
        public KolonIsterler(ILoginView loginView)
        {
            InitializeComponent();
            _loginView = loginView;
            //var filePath = "path_to_your_excel_file.xlsx";
            //_loginView.ReadExcelFile(filePath);
            musteriList = _loginView.GetMusteriList();
            //LoadDataFromFile(filePath, musteri);

        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private List<IMusteri> musteriList = new List<IMusteri>();

        private async void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (musteriList != null)
            {
                var lines = new List<string>();

                // Kullanıcıdan alınan değer (örneğin bir textbox'tan alınabilir)

                foreach (var musteri in musteriList)
                {
                    // Boş alanları kontrol et ve sadece boş olanlara değer atama yap
                    if (string.IsNullOrEmpty(musteri.Durum))
                        musteri.Durum = txtDurum.Text;// Kullanıcıdan alınacak değer gelmeli
                    if (string.IsNullOrEmpty(musteri.IlgiliKisi))
                        musteri.IlgiliKisi = txtIlgiliKisi.Text; // Kullanıcıdan alınacak değer gelmeli
                    if (string.IsNullOrEmpty(musteri.MusteriGrubu))
                        musteri.MusteriGrubu = txtMüsteriGrubu.Text; // Kullanıcıdan alınacak değer gelmeli
                    if (string.IsNullOrEmpty(musteri.MusteriEkGrubu))
                        musteri.MusteriEkGrubu = txtMusteriEkgrup.Text; // Kullanıcıdan alınacak değer gelmeli
                    if (string.IsNullOrEmpty(musteri.OdemeTipi))
                        musteri.OdemeTipi = txtOdemeTipi.Text; // Kullanıcıdan alınacak değer gelmeli
                    if (string.IsNullOrEmpty(musteri.KisaAdi))
                        musteri.KisaAdi = txtKisaAdi.Text; // Kullanıcıdan alınacak değer gelmeli

                    // Boş MusteriKodu alanını doldur
                   

                   var line = new List<string>
           {
                $"Durum={musteri.Durum}",
                $"MusteriKodu={musteri.MusteriKodu}",
                $"Unvan={musteri.Unvan}",
                $"IlgiliKisi={musteri.IlgiliKisi}",
                $"MusteriGrubu={musteri.MusteriGrubu}",
                $"MusteriEkGrubu={musteri.MusteriEkGrubu}",
                $"OdemeTipi={musteri.OdemeTipi}",
                $"KisaAdi={musteri.KisaAdi}",
                $"VergiTipi={musteri.VergiTipi}"
           };

                    lines.Add(string.Join(Environment.NewLine, line));
                }

                try
                {
                    // Tüm satırları dosyaya yaz
                    File.WriteAllLines(filePath, lines);

                    // Başarı mesajı göster
                    var mesaj1 = new Tasarim1.BildirimMesaji($"Dosya başarıyla kaydedildi: {filePath}");
                    mesaj1.Show();
                    await Task.Delay(2000); // 2 saniye bekle
                    mesaj1.Close();
                }
                catch (Exception ex)
                {
                    // Hata mesajını göster
                    var mesajHata = new Tasarim1.BildirimMesaji($"Bir hata oluştu: {ex.Message}");
                    mesajHata.Show();
                    await Task.Delay(2000); // 2 saniye bekle
                    mesajHata.Close();
                }

                // Dosyadan verileri yükle
                var musteriq = _loginView.GetMusteriList();
                LoadDataFromFile(filePath, musteriq);
            }
        }
        public void LoadDataFromFile(string filePath, List<IMusteri> GuncellenecekMusteriList)
        {

            // Dosya var mı kontrol et
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);

                // Her bir satırı işle
                //foreach (var line in lines)
                //{
                //    var keyValue = line.Split('=');
                //    if (keyValue.Length == 2)
                //    {
                //        var key = keyValue[0].Trim();
                //        var value = keyValue[1].Trim();

                //        // Her bir 'Musteri' nesnesini kontrol et
                //        foreach (var musteri in GuncellenecekMusteriList)
                //        {
                //            switch (key)
                //            {
                //                case "Durum":
                //                    musteri.Durum = value;
                //                    break;
                //                case "MusteriKodu":
                //                    musteri.MusteriKodu = value;
                //                    break;
                //                case "Unvan":
                //                    musteri.Unvan = value;
                //                    break;
                //                case "IlgiliKisi":
                //                    musteri.IlgiliKisi = value;
                //                    break;
                //                case "MusteriGrubu":
                //                    musteri.MusteriGrubu = value;
                //                    break;
                //                case "MusteriEkGrubu":
                //                    musteri.MusteriEkGrubu = value;
                //                    break;
                //                case "OdemeTipi":
                //                    musteri.OdemeTipi = value;
                //                    break;
                //                case "KisaAdi":
                //                    musteri.KisaAdi = value;
                //                    break;
                //                case "VergiTipi":
                //                    musteri.VergiTipi = value;
                //                    break;
                //            }
                //        }
                        // loginview'daki global değişkene musteriList'i gonder
                        _loginView.MusteriAL(GuncellenecekMusteriList);
                    //}
                //}
            }
            else
            {
                // Dosya bulunamazsa kullanıcıya bilgi ver
                MessageBox.Show("Belirtilen dosya bulunamadı.");
            }
        }
    }
}