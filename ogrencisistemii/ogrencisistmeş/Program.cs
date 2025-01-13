using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

// Personel Arayüzü
public interface IPersonal
{
    void BilgileriGoster();
}

// Temel Sınıf: Birey
public abstract class Birey : IPersonal
{
    public string Ad { get; set; }
    public string Soyad { get; set; }

    public Birey(string ad, string soyad)
    {
        Ad = ad;
        Soyad = soyad;
    }

    public abstract void BilgileriGoster();
}

// Öğrenci Sınıfı
public class Ogrenci : Birey
{
    public string OgrenciNo { get; set; }

    public Ogrenci(string ad, string soyad, string ogrenciNo) : base(ad, soyad)
    {
        OgrenciNo = ogrenciNo;
    }

    public override void BilgileriGoster()
    {
        Console.WriteLine($"Öğrenci: {Ad} {Soyad}, Öğrenci No: {OgrenciNo}");
    }
}

// Öğretim Görevlisi Sınıfı
public class OgretimGorevlisi : Birey
{
    public string Bolum { get; set; }

    public OgretimGorevlisi(string ad, string soyad, string bolum) : base(ad, soyad)
    {
        Bolum = bolum;
    }

    public override void BilgileriGoster()
    {
        Console.WriteLine($"Öğretim Görevlisi: {Ad} {Soyad}, Bölüm: {Bolum}");
    }
}

// Ders Sınıfı
public class Ders
{
    public string DersAdi { get; set; }
    public double Krediler { get; set; }
    public OgretimGorevlisi OgretimGorevlisi { get; set; }
    public List<Ogrenci> KayitliOgrenciler { get; set; }

    public Ders(string dersAdi, double krediler, OgretimGorevlisi ogretimGorevlisi)
    {
        DersAdi = dersAdi;
        Krediler = krediler;
        OgretimGorevlisi = ogretimGorevlisi;
        KayitliOgrenciler = new List<Ogrenci>();
    }

    public void OgrenciKayitEt(Ogrenci ogrenci)
    {
        KayitliOgrenciler.Add(ogrenci);
    }

    public void OgrenciKayitSil(Ogrenci ogrenci)
    {
        KayitliOgrenciler.Remove(ogrenci);
    }
}

// Veri İşleme Sınıfı (JSON İşlemleri İçin)
public static class VeriIsleyici
{
    public static void VeriyiKaydet<T>(string dosyaAdi, List<T> veri)
    {
        var json = JsonConvert.SerializeObject(veri, Formatting.Indented); // JSON stringini oluştur
        File.WriteAllText(dosyaAdi, json); // JSON stringini dosyaya yaz
    }

    public static List<T> VeriyiYukle<T>(string dosyaAdi)
    {
        if (!File.Exists(dosyaAdi)) return new List<T>(); // Dosya yoksa boş liste döndür
        var json = File.ReadAllText(dosyaAdi); // Dosyadan JSON metni oku
        return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>(); // JSON stringini nesneye dönüştür
    }
}

// Ana Program
public class Program
{
    public static void Main()
    {
        bool devamEt = true;
        var ogretimGorevlileri = VeriIsleyici.VeriyiYukle<OgretimGorevlisi>("ogretim_gorevlileri.json");
        var dersler = VeriIsleyici.VeriyiYukle<Ders>("dersler.json");
        var ogrenciler = VeriIsleyici.VeriyiYukle<Ogrenci>("ogrenciler.json");

        while (devamEt)
        {
            Console.Clear();
            Console.WriteLine("\n1. Öğrenci Girişi");
            Console.WriteLine("2. Öğretim Görevlisi ve Ekle");
            Console.WriteLine("3. Derse Kayıt Ol");
            Console.WriteLine("4. Dersten Kayıt Sil");
            Console.WriteLine("5. Öğrenci Listesini Görüntüle");
            Console.WriteLine("6. Çıkış");
            Console.Write("Seçiminizi yapın: ");
            string secim = Console.ReadLine();

            switch (secim)
            {
                case "1":
                    Console.Write("Öğrenci Adı: ");
                    string ad = Console.ReadLine();
                    Console.Write("Öğrenci Soyadı: ");
                    string soyad = Console.ReadLine();

                    Console.Write("Öğrenci No (6 haneli): ");
                    string ogrenciNo;
                    while (true)
                    {
                        ogrenciNo = Console.ReadLine();
                        if (ogrenciNo.Length == 6 && long.TryParse(ogrenciNo, out _))
                            break;
                        else
                            Console.Write("Geçersiz öğrenci numarası! Lütfen geçerli bir 6 haneli öğrenci numarası girin: ");
                    }

                    ogrenciler.Add(new Ogrenci(ad, soyad, ogrenciNo));
                    Console.WriteLine("Öğrenci başarıyla tanımlandı!");
                    break;

                case "2":
                    Console.Write("Öğretim Görevlisi eklemek için şifre girin: ");
                    string sifre = Console.ReadLine();
                    if (sifre != "admin")
                    {
                        Console.WriteLine("Hatalı şifre! Öğretim görevlisi ekleyemezsiniz.");
                        break;
                    }

                    Console.WriteLine("Öğretim Görevlisi Ekle:");
                    Console.Write("Ad: ");
                    string ogretimAd = Console.ReadLine();
                    Console.Write("Soyad: ");
                    string ogretimSoyad = Console.ReadLine();
                    Console.Write("Bölüm: ");
                    string bolum = Console.ReadLine();
                    var ogretimGorevlisi = new OgretimGorevlisi(ogretimAd, ogretimSoyad, bolum);
                    ogretimGorevlileri.Add(ogretimGorevlisi);
                    Console.WriteLine("Öğretim görevlisi başarıyla eklendi!");

                    Console.WriteLine("Ders Ekle:");
                    Console.Write("Ders Adı: ");
                    string dersAdi = Console.ReadLine();
                    Console.Write("Kredi Sayısı: ");
                    double kredi;
                    while (!double.TryParse(Console.ReadLine(), out kredi) || kredi <= 0)
                    {
                        Console.Write("Geçersiz kredi sayısı! Lütfen pozitif bir değer girin: ");
                    }

                    var ders = new Ders(dersAdi, kredi, ogretimGorevlisi);
                    dersler.Add(ders);
                    Console.WriteLine("Ders başarıyla eklendi!");
                    break;

                case "3":
                    if (ogrenciler.Count == 0 || dersler.Count == 0)
                    {
                        Console.WriteLine("Öğrenci veya ders bulunmamaktadır.");
                        break;
                    }
                    Console.WriteLine("Öğrenci Seçin:");
                    for (int i = 0; i < ogrenciler.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {ogrenciler[i].Ad} {ogrenciler[i].Soyad} - Öğrenci No: {ogrenciler[i].OgrenciNo}");
                    }
                    int ogrenciSecim = int.Parse(Console.ReadLine()) - 1;

                    Console.WriteLine("Ders Seçin:");
                    for (int i = 0; i < dersler.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {dersler[i].DersAdi} - Öğretim Görevlisi: {dersler[i].OgretimGorevlisi.Ad} {dersler[i].OgretimGorevlisi.Soyad}");
                    }
                    int dersSecim = int.Parse(Console.ReadLine()) - 1;

                    dersler[dersSecim].OgrenciKayitEt(ogrenciler[ogrenciSecim]);
                    Console.WriteLine("Öğrenci başarıyla derse kaydedildi!");
                    break;

                case "4":
                    if (ogrenciler.Count == 0 || dersler.Count == 0)
                    {
                        Console.WriteLine("Öğrenci veya ders bulunmamaktadır.");
                        break;
                    }
                    Console.WriteLine("Öğrenci Seçin:");
                    for (int i = 0; i < ogrenciler.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {ogrenciler[i].Ad} {ogrenciler[i].Soyad} - Öğrenci No: {ogrenciler[i].OgrenciNo}");
                    }
                    int ogrenciSecimSil = int.Parse(Console.ReadLine()) - 1;

                    Console.WriteLine("Ders Seçin:");
                    for (int i = 0; i < dersler.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {dersler[i].DersAdi} - Öğretim Görevlisi: {dersler[i].OgretimGorevlisi.Ad} {dersler[i].OgretimGorevlisi.Soyad}");
                    }
                    int dersSecimSil = int.Parse(Console.ReadLine()) - 1;

                    dersler[dersSecimSil].OgrenciKayitSil(ogrenciler[ogrenciSecimSil]);
                    Console.WriteLine("Öğrenci başarıyla dersten silindi!");
                    break;

                case "5":
                    if (ogrenciler.Count > 0)
                    {
                        Console.WriteLine("Öğrenci Listesi:");
                        foreach (var ogrenci in ogrenciler)
                        {
                            Console.WriteLine($"- {ogrenci.Ad} {ogrenci.Soyad}, Öğrenci No: {ogrenci.OgrenciNo}");
                            Console.WriteLine("  Aldığı Dersler:");
                            foreach (var dersKaydi in dersler)
                            {
                                if (dersKaydi.KayitliOgrenciler.Contains(ogrenci))
                                {
                                    Console.WriteLine($"    * Ders Adı: {dersKaydi.DersAdi}, Krediler: {dersKaydi.Krediler}, Öğretim Görevlisi: {dersKaydi.OgretimGorevlisi.Ad} {dersKaydi.OgretimGorevlisi.Soyad}");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Henüz öğrenci bulunmamaktadır.");
                    }
                    break;

                case "6":
                    Console.WriteLine("Çıkıyor...");
                    devamEt = false;
                    break;

                default:
                    Console.WriteLine("Geçersiz seçim! Lütfen 1 ile 6 arasında bir seçenek girin.");
                    break;
            }

            if (devamEt)
            {
                Console.WriteLine("Devam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }

        // Verileri kaydet
        VeriIsleyici.VeriyiKaydet("ogretim_gorevlileri.json", ogretimGorevlileri);
        VeriIsleyici.VeriyiKaydet("dersler.json", dersler);
        VeriIsleyici.VeriyiKaydet("ogrenciler.json", ogrenciler);
    }
}