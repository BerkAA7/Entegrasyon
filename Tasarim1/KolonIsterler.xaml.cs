using ClosedXML.Excel;
using ExcelToPanorama;
using ExcelToPanorama.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
            var filePath = "path_to_your_excel_file.xlsx";
            _loginView.ReadExcelFile(filePath);
            var musteriler = _loginView.GetMusteriList();
            foreach (var musteri in musteriler)
            {
                MessageBox.Show(musteri.Durum);
            }
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
            if (musteriList != null || musteriList == null)
            {
                foreach (var musteri in musteriList)
                {
                    var lines = new List<string>
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
                    try
                    {
                        // Verileri text dosyasına yaz
                        File.WriteAllLines(filePath, lines);

                        // İlk mesajı göster
                        var mesaj1 = new Tasarim1.BildirimMesaji($"Dosya başarıyla kaydedildi: {filePath}");
                        mesaj1.Show();

                        // Mesajı belirli bir süre sonra kapat
                        await Task.Delay(2000); // 2 saniye bekle
                        mesaj1.Close();

                        // İkinci mesajı göster
                        var mesaj2 = new Tasarim1.BildirimMesaji("Bilgiler kaydediliyor..!");
                        mesaj2.Show();

                        // Kaydetme işlemi için kısa bir süre bekle
                        await Task.Delay(500); // 0.5 saniye bekle
                        mesaj2.Close();

                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        // Hata mesajını göster
                        var mesajHata = new Tasarim1.BildirimMesaji($"Bir hata oluştu: {ex.Message}");
                        mesajHata.Show();

                        // Hata mesajını belirli bir süre sonra kapat
                        await Task.Delay(2000); // 2 saniye bekle
                        mesajHata.Close();
                    }
                }
                
            }
        }
        public void LoadDataFromFile(string filePath, List<IMusteri> musteriList)
        {
            // Dosya var mı kontrol et
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);

                // Her bir satırı işle
                foreach (var line in lines)
                {
                    var keyValue = line.Split('=');
                    if (keyValue.Length == 2)
                    {
                        var key = keyValue[0].Trim();
                        var value = keyValue[1].Trim();

                        // Her bir 'Musteri' nesnesini kontrol et
                        foreach (var musteri in musteriList)
                        {
                            switch (key)
                            {
                                case "Durum":
                                    musteri.Durum = value;
                                    break;
                                case "MusteriKodu":
                                    musteri.MusteriKodu = value;
                                    break;
                                case "Unvan":
                                    musteri.Unvan = value;
                                    break;
                                case "IlgiliKisi":
                                    musteri.IlgiliKisi = value;
                                    break;
                                case "MusteriGrubu":
                                    musteri.MusteriGrubu = value;
                                    break;
                                case "MusteriEkGrubu":
                                    musteri.MusteriEkGrubu = value;
                                    break;
                                case "OdemeTipi":
                                    musteri.OdemeTipi = value;
                                    break;
                                case "KisaAdi":
                                    musteri.KisaAdi = value;
                                    break;
                                case "VergiTipi":
                                    musteri.VergiTipi = value;
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                // Dosya bulunamazsa kullanıcıya bilgi ver
                MessageBox.Show("Belirtilen dosya bulunamadı.");
            }
        }

    }
}