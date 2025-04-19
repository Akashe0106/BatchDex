using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using System.IO;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Shell;

namespace BatchDex
{
    /// <summary>
    /// Ana uygulama penceresinin kod-arkası (code-behind) mantığını içerir.
    /// Pencere olaylarını, menü işlemlerini ve metin düzenleyici etkileşimlerini yönetir.
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _currentFilePath = null;
        private bool _isDirty = false;
        private Thickness? _originalResizeBorderThickness = null;

        /// <summary>
        /// MainWindow sınıfının yeni bir örneğini başlatır.
        /// Bileşenleri başlatır, olayları bağlar ve başlangıç ayarlarını yapar.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            StateChanged += MainWindow_StateChanged;
            UpdateMaximizeButtonContent();

            UpdatePlaceholderVisibility();
            LoadBatchSyntaxHighlighting();
            AddLeftMarginSpacer(7.0);

            ScriptEditor.TextChanged += (sender, args) => { _isDirty = true; UpdateTitle(); };

            var saveCommandBinding = new CommandBinding(
            ApplicationCommands.Save,
            MenuItemSave_Executed,
            MenuItemSave_CanExecute);
            this.CommandBindings.Add(saveCommandBinding);

            UpdateTitle();
        }

        /// <summary>
        /// Simge durumuna küçültme düğmesine tıklandığında pencereyi simge durumuna küçültür.
        /// </summary>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Ekranı kapla/geri yükle düğmesine tıklandığında pencerenin durumunu değiştirir.
        /// </summary>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// Kapat düğmesine tıklandığında pencereyi kapatır.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Pencerenin durumu (Normal, Minimized, Maximized) değiştiğinde tetiklenir.
        /// Kenarlık kalınlıklarını ve maksimize butonunun içeriğini günceller.
        /// </summary>
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            WindowChrome windowChrome = WindowChrome.GetWindowChrome(this);

            if (this.WindowState == WindowState.Maximized)
            {
                MainContentBorder.BorderThickness = new Thickness(0);

                if (windowChrome != null)
                {
                    if (_originalResizeBorderThickness == null)
                    {
                        _originalResizeBorderThickness = windowChrome.ResizeBorderThickness;
                    }
                    windowChrome.ResizeBorderThickness = new Thickness(0);
                }

                MaximizeButton.Content = "❐";
                MaximizeButton.ToolTip = "Geri Yükle";
            }
            else
            {
                MainContentBorder.BorderThickness = new Thickness(1);

                if (windowChrome != null && _originalResizeBorderThickness.HasValue)
                {
                    windowChrome.ResizeBorderThickness = _originalResizeBorderThickness.Value;
                }

                MaximizeButton.Content = "☐";
                MaximizeButton.ToolTip = "Ekranı Kapla";
            }
        }

        /// <summary>
        /// Pencerenin mevcut durumuna göre ekranı kapla/geri yükle düğmesinin içeriğini ve araç ipucunu günceller.
        /// </summary>
        private void UpdateMaximizeButtonContent()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MaximizeButton.Content = "❐";
                MaximizeButton.ToolTip = "Geri Yükle";
            }
            else
            {
                MaximizeButton.Content = "☐";
                MaximizeButton.ToolTip = "Ekranı Kapla";
            }
        }

        /// <summary>
        /// Metin düzenleyicideki içerik değiştiğinde tetiklenir.
        /// Değişiklik bayrağını ayarlar, başlığı ve yer tutucu görünürlüğünü günceller.
        /// </summary>
        private void ScriptEditor_TextChanged(object sender, EventArgs e)
        {
            _isDirty = true;
            UpdateTitle();
            UpdatePlaceholderVisibility();
        }


        /// <summary>
        /// Metin düzenleyicinin içeriğine göre yer tutucu metnin görünürlüğünü ayarlar.
        /// </summary>
        private void UpdatePlaceholderVisibility()
        {
            if (string.IsNullOrEmpty(ScriptEditor.Text))
            {
                PlaceholderTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                PlaceholderTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Batch (.bat) dosyaları için özel sözdizimi vurgulama tanımını yükler ve uygular.
        /// </summary>
        private void LoadBatchSyntaxHighlighting()
        {
            string syntaxFileName = "BatchSyntax.xshd";

            try
            {
                string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string syntaxFilePath = System.IO.Path.Combine(appPath, syntaxFileName);

                if (File.Exists(syntaxFilePath))
                {
                    using (XmlTextReader reader = new XmlTextReader(syntaxFilePath))
                    {
                        ScriptEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
                else
                {
                    ScriptEditor.SyntaxHighlighting = null;
                    Console.WriteLine($"{syntaxFileName} bulunamadı. Syntax highlighting devre dışı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Syntax highlighting yüklenirken hata oluştu: {ex.Message}");
                ScriptEditor.SyntaxHighlighting = null;
            }
        }

        /// <summary>
        /// Metin düzenleyicinin sol kenar boşluğuna belirli bir genişlikte boş bir alan ekler.
        /// Genellikle satır numaralarından sonra kullanılır.
        /// </summary>
        /// <param name="width">Eklenecek boşluğun piksel cinsinden genişliği.</param>
        private void AddLeftMarginSpacer(double width)
        {
            var textArea = ScriptEditor.TextArea;
            int lineNumberMarginIndex = -1;
            for (int i = 0; i < textArea.LeftMargins.Count; i++)
            {
                if (textArea.LeftMargins[i] is LineNumberMargin)
                {
                    lineNumberMarginIndex = i;
                    break;
                }
            }

            if (lineNumberMarginIndex != -1)
            {
                var spacerMargin = new SpaceMargin(width);
                textArea.LeftMargins.Insert(lineNumberMarginIndex + 1, spacerMargin);
            }
            else
            {
                var spacerMargin = new SpaceMargin(width);
                textArea.LeftMargins.Insert(Math.Min(1, textArea.LeftMargins.Count), spacerMargin);
                Console.WriteLine("LineNumberMargin bulunamadı, boşluk margin'i varsayılan konuma eklendi.");
            }
        }

        /// <summary>
        /// Pencere başlığını, geçerli dosya adı ve kaydedilmemiş değişiklik durumunu (* işareti) içerecek şekilde günceller.
        /// </summary>
        private void UpdateTitle()
        {
            string fileName = _currentFilePath == null ? "Yeni Betik" : System.IO.Path.GetFileName(_currentFilePath);
            string dirtyMarker = _isDirty ? "*" : "";
            this.Title = $"{fileName}{dirtyMarker} - BatchDex";
        }

        /// <summary>
        /// 'Yeni' menü öğesine tıklandığında çalışır. Varsa değişiklikleri kaydetmeyi sorar ve editörü temizler.
        /// </summary>
        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            if (AskToSaveChanges())
            {
                ScriptEditor.Clear();
                _currentFilePath = null;
                _isDirty = false;
                UpdateTitle();
                UpdatePlaceholderVisibility();
            }
        }

        /// <summary>
        /// 'Aç' menü öğesine tıklandığında çalışır. Varsa değişiklikleri kaydetmeyi sorar ve bir dosya açma iletişim kutusu gösterir.
        /// </summary>
        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            if (!AskToSaveChanges()) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Batch Dosyaları (*.bat;*.cmd)|*.bat;*.cmd|Tüm Dosyalar (*.*)|*.*";
            openFileDialog.DefaultExt = ".bat";
            openFileDialog.Title = "Batch Betiği Aç";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ScriptEditor.Text = File.ReadAllText(openFileDialog.FileName, Encoding.Default);
                    _currentFilePath = openFileDialog.FileName;
                    _isDirty = false;
                    UpdateTitle();
                    UpdatePlaceholderVisibility();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Dosya okunurken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 'Kaydet' menü öğesine veya ilişkili komuta tıklandığında çalışır. Mevcut dosyayı kaydeder.
        /// </summary>
        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        /// <summary>
        /// 'Farklı Kaydet' menü öğesine tıklandığında çalışır. Dosyayı yeni bir konumda kaydetmek için iletişim kutusu gösterir.
        /// </summary>
        private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileAs();
        }

        /// <summary>
        /// Metin düzenleyicideki içeriği geçerli dosyaya kaydeder. Eğer geçerli dosya yoksa 'Farklı Kaydet' işlevini çağırır.
        /// </summary>
        /// <returns>Kaydetme işlemi başarılıysa true, aksi takdirde false.</returns>
        private bool SaveFile()
        {
            if (_currentFilePath == null)
            {
                return SaveFileAs();
            }
            else
            {
                try
                {
                    File.WriteAllText(_currentFilePath, ScriptEditor.Text, Encoding.Default);
                    _isDirty = false;
                    UpdateTitle();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Dosya kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        /// <summary>
        /// Metin düzenleyicideki içeriği kullanıcı tarafından seçilen yeni bir dosyaya kaydeder.
        /// </summary>
        /// <returns>Kaydetme işlemi başarılıysa true, aksi takdirde false.</returns>
        private bool SaveFileAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Batch Dosyaları (*.bat)|*.bat|Komut Dosyaları (*.cmd)|*.cmd|Tüm Dosyalar (*.*)|*.*";
            saveFileDialog.DefaultExt = ".bat";
            saveFileDialog.Title = "Batch Betiğini Farklı Kaydet";
            saveFileDialog.FileName = _currentFilePath == null ? "YeniBetik.bat" : System.IO.Path.GetFileName(_currentFilePath);

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, ScriptEditor.Text, Encoding.Default);
                    _currentFilePath = saveFileDialog.FileName;
                    _isDirty = false;
                    UpdateTitle();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Dosya kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Metin düzenleyicide kaydedilmemiş değişiklikler varsa kullanıcıya kaydetmeyi sorar.
        /// </summary>
        /// <returns>Kullanıcı 'Evet'e basıp kaydetme başarılıysa veya 'Hayır'a basarsa true; 'İptal'e basarsa veya kaydetme başarısız olursa false.</returns>
        private bool AskToSaveChanges()
        {
            if (!_isDirty) return true;

            string fileName = _currentFilePath == null ? "Yeni Betik" : System.IO.Path.GetFileName(_currentFilePath);
            MessageBoxResult result = MessageBox.Show(
                $"'{fileName}' dosyasındaki değişiklikleri kaydetmek istiyor musunuz?",
                "BatchDex - Değişiklikleri Kaydet?",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                return SaveFile();
            }
            else if (result == MessageBoxResult.No)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Pencere kapanmadan hemen önce çağrılır. Kaydedilmemiş değişiklikler varsa kullanıcıya sorar ve gerekirse kapanmayı iptal eder.
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!AskToSaveChanges())
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// 'Çalıştır' düğmesine tıklandığında çalışır. Metin düzenleyicideki içeriği geçici bir .bat dosyasına yazar ve çalıştırır.
        /// </summary>
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            string scriptContent = ScriptEditor.Text;

            if (string.IsNullOrWhiteSpace(scriptContent))
            {
                MessageBox.Show("Çalıştırılacak betik bulunamadı!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string tempBatPath = null;
            try
            {
                tempBatPath = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".bat");
                File.WriteAllText(tempBatPath, scriptContent, Encoding.Default);

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.Arguments = $"/K \"{tempBatPath}\"";
                psi.WorkingDirectory = System.IO.Path.GetDirectoryName(tempBatPath);
                psi.UseShellExecute = true;

                Process process = Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Betik çalıştırılırken bir hata oluştu:\n{ex.Message}", "Çalıştırma Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Geçici dosya silinmez (/K kullanıldığı için)
            }
        }

        /// <summary>
        /// 'Temizle' menü öğesine tıklandığında çalışır. Metin düzenleyicinin içeriğini temizler.
        /// </summary>
        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            ScriptEditor.Clear();
        }

        /// <summary>
        /// 'Geri Al' menü öğesine tıklandığında çalışır. Metin düzenleyicideki son değişikliği geri alır.
        /// </summary>
        private void MenuItemUndo_Click(object sender, RoutedEventArgs e)
        {
            ScriptEditor.Undo();
        }

        /// <summary>
        /// 'İleri Al' menü öğesine tıklandığında çalışır. Metin düzenleyicideki geri alınan son değişikliği yeniden uygular.
        /// </summary>
        private void MenuItemRedo_Click(object sender, RoutedEventArgs e)
        {
            ScriptEditor.Redo();
        }

        /// <summary>
        /// Kaydet komutu (örn. Ctrl+S) yürütüldüğünde çağrılır.
        /// </summary>
        private void MenuItemSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFile();
        }

        /// <summary>
        /// Kaydet komutunun yürütülüp yürütülemeyeceğini belirler (yalnızca değişiklik varsa).
        /// </summary>
        private void MenuItemSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _isDirty;
        }

        #region Win32 Maximize Fix

        private const int WM_GETMINMAXINFO = 0x0024;
        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int x; public int y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int left; public int top; public int right; public int bottom; }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        #endregion

        /// <summary>
        /// Pencerenin kaynakları başlatıldıktan sonra çağrılır.
        /// Orijinal pencere kenarlık kalınlığını saklar ve pencere mesaj kancasını ekler.
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowChrome chrome = WindowChrome.GetWindowChrome(this);
            if (chrome != null)
            {
                _originalResizeBorderThickness = chrome.ResizeBorderThickness;
            }

            if (PresentationSource.FromVisual(this) is HwndSource source)
            {
                source.AddHook(WndProc);
            }
        }

        /// <summary>
        /// Pencerenin Win32 mesajlarını işleyen özel bir yöntem.
        /// Özellikle WM_GETMINMAXINFO mesajını işleyerek tam ekran sorununu düzeltir.
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                try
                {
                    MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
                    IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
                    if (monitor != IntPtr.Zero)
                    {
                        MONITORINFO monitorInfo = new MONITORINFO();
                        monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                        GetMonitorInfo(monitor, ref monitorInfo);
                        RECT rcWorkArea = monitorInfo.rcWork;
                        RECT rcMonitorArea = monitorInfo.rcMonitor;
                        mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                        mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                        mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                        mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                        Marshal.StructureToPtr(mmi, lParam, true);
                    }
                    handled = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WM_GETMINMAXINFO handling error: {ex.Message}");
                }
            }
            return IntPtr.Zero;
        }
    }

    /// <summary>
    /// AvalonEdit metin düzenleyicisinin sol kenar boşluğuna sabit genişlikte bir boşluk eklemek için kullanılan özel bir kenar boşluğu öğesi.
    /// </summary>
    internal class SpaceMargin : AbstractMargin
    {
        private readonly double _width;

        /// <summary>
        /// Belirtilen genişlikte yeni bir SpaceMargin örneği oluşturur.
        /// </summary>
        /// <param name="width">Kenar boşluğunun piksel cinsinden genişliği.</param>
        public SpaceMargin(double width) { _width = width; }

        /// <summary>
        /// Kenar boşluğunun boyutunu ölçer. Genişlik sabittir, yükseklik sıfırdır.
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(_width, 0);
        }
    }
}