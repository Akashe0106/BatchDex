# BatchDex

BatchDex, Windows için geliştirilmiş, sözdizimi vurgulama özelliğine sahip basit ve kullanıcı dostu bir Batch script düzenleyicisidir (`.bat`, `.cmd`). Batch dosyaları oluşturmayı ve düzenlemeyi kolaylaştırmak için tasarlanmıştır.

![BatchDex Ana Ekran](https://github.com/Akashe0106/BatchDex/blob/main/SCREENSHOT.jpg)

## Özellikler

*   Batch (`.bat`, `.cmd`) dosyaları için **Sözdizimi Vurgulama** (renklendirme).
*   Basit, temiz ve modern **kullanıcı arayüzü (WPF)**.
*   Yazılan betiği **doğrudan çalıştırma** özelliği (yeni bir komut istemcisi penceresinde).
*   Temel **dosya işlemleri** (Yeni, Aç, Kaydet, Farklı Kaydet).
*   **Satır numaraları** gösterimi.
*   **Geri Al / İleri Al** desteği.
*   Koyu tema arayüz.
*   Özelleştirilebilir sözdizimi vurgulama ( `BatchSyntax.xshd` dosyası ile).

## Kurulum

Uygulamanın en son sürümünü kolayca kurmak için aşağıdaki adımları izleyin:

1.  **GitHub Releases Sayfasına Gidin:** Projenin en son yayınlanmış sürümünü  adresinden bulun.

2.  **Kurulum Dosyalarını İndirin:** Yayınlanan sürümü `setup.exe` ve `BatchDexSetup.msi` dosyalarını bilgisayarınıza **aynı klasöre** indirin.

3.  **Setup.exe'yi Çalıştırın:** **ÖNEMLİ:** Kurulumu başlatmak için **`setup.exe`** dosyasını çalıştırın. Bu dosya, gerekli olan .NET Framework 4.8'in bilgisayarınızda kurulu olup olmadığını kontrol edecektir. Eğer yüklü ise doğrudan `BatchDexSetup.msi` açabilirsiniz.

4.  **Kullanıcı Hesabı Denetimi (UAC) Uyarısı:** "Bilinmeyen bir yayıncıya ait..." şeklinde bir uyarı alabilirsiniz. Bu normaldir, çünkü kurulum dosyaları şu anda bir kod imzalama sertifikası ile imzalanmamıştır. Devam etmek için "Evet"i seçin.

5.  **Kurulum Sihirbazını Takip Edin:** Ekrandaki adımları izleyerek kurulumu tamamlayın.

**Gereksinim:** Kurulumun başarıyla tamamlanması ve uygulamanın çalışması için bilgisayarınızda **.NET Framework 4.8 veya üzeri** bir sürümün yüklü olması gerekmektedir. `setup.exe` bu kontrolü yapacaktır.

## Kullanım

1.  Kurulum tamamlandıktan sonra Masaüstü'nde veya Başlat Menüsü'nde oluşturulan "BatchDex" kısayolunu kullanarak uygulamayı başlatın.
2.  Doğrudan kod yazmaya başlayın veya "Dosya" menüsünden mevcut bir `.bat` ya da `.cmd` dosyasını "Aç..." seçeneği ile açın.
3.  Değişikliklerinizi "Dosya" menüsündeki "Kaydet" (Ctrl+S) veya "Farklı Kaydet..." seçenekleriyle kaydedin.
4.  Yazdığınız betiği çalıştırmak için araç çubuğundaki yeşil **▶ (Çalıştır)** butonuna tıklayın. Betik yeni bir komut istemcisi penceresinde çalıştırılacaktır.
5.  "Araçlar" menüsünden editörü temizleyebilir, geri alabilir veya ileri alabilirsiniz.

## Kullanılan Teknolojiler

*   ![.NET Framework 4.8](https://img.shields.io/badge/.NET_Framework_4.8-5C2D91?style=flat&logo=.net&logoColor=white)
*   ![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)
*   ![WPF](https://img.shields.io/badge/WPF-5C2D91?style=flat&logo=windows&logoColor=white)
*   **[ICSharpCode.AvalonEdit](https://github.com/icsharpcode/AvalonEdit)** (Metin düzenleyici ve sözdizimi vurgulama için)
*   **Visual Studio Installer Projects** (Kurulum paketi oluşturmak için)

## Uyumluluk

Bu proje .NET Framework 4.8 kullanılarak geliştirilmiştir. Windows 7 SP1, Windows 8.1, Windows 10 (v1903 ve sonrası) ve Windows 11 gibi .NET Framework 4.8'i destekleyen işletim sistemlerinde çalışması hedeflenmiştir. `setup.exe` dosyası, kurulum öncesinde .NET Framework 4.8 gereksinimini kontrol eder.

## Katkıda Bulunma

Projeye katkıda bulunmak isterseniz, lütfen bir "Issue" açarak hataları bildirin veya geliştirmeler önerin. "Pull Request" göndermek isterseniz, önce bir issue açarak planlanan değişikliği tartışmanız önerilir. [KATKIDA BULUNMA](CONTRIBUTING.md)

## Lisans

Bu proje **MIT Lisansı** altında lisanslanmıştır. Daha fazla bilgi için [LICENSE](LICENSE)

## İletişim

Sorularınız veya geri bildirimleriniz için `yilmazefeondu@gmail.com` ile iletişime geçebilirsiniz veya bir GitHub Issue açabilirsiniz.
