using FileCompare_Reforged.Properties;
using FileCompare_Reforged.ViewModel;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Forms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileCompare_Reforged
{
    public class MainPageVM : BindableBase
    {
        private static Properties.Settings settings = new Settings();

        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private NotifyIcon notifyIcon;

        private string _infoText;

        public string InfoText
        {
            get => _infoText;
            set => SetProperty(ref _infoText, value);
        }



        private DelegateCommand _openFileCommand;

        public DelegateCommand OpenFileCommand =>
            _openFileCommand ??= new DelegateCommand(() => { Process.Start(MainWindowVM.AltFilePath); });

        private Visibility _buttonVisibility;

        public Visibility ButtonVisibility
        {
            get => _buttonVisibility;

            set => SetProperty(ref _buttonVisibility, value);
        }

        private void ResultHandler(FileResult.FileHandlerResult value)
        {
            Bitmap bmp;
            string notification = String.Empty;

            switch (value)
            {
                case FileResult.FileHandlerResult.NoChanges:
                    bmp = Properties.Resources.Error_document1320196654394535651_512;
                    InfoText = "Новой версии файла не найдено";
                    break;
                case FileResult.FileHandlerResult.WithChanges:
                    bmp = Properties.Resources.check_document_file_icon_icon_1320196654394535651_512;
                    notification = "Найдена новая версия файла";
                    InfoText = notification;
                    break;
                case FileResult.FileHandlerResult.NoFiles:
                    bmp = Properties.Resources.Error_document1320196654394535651_512;
                    InfoText = "Файла предыдущей версии не найдено, скачивание файла!";
                    break;
                case FileResult.FileHandlerResult.WithFindWord:
                    bmp = Properties.Resources.check_document_file_icon_icon_1320196654394535651_512;
                    notification =
                        $"Найдена новая версия файла {settings.FileName}! В нем найдено слово {settings.FindWord}";
                    InfoText = notification;
                    break;
                default:
                    return;
            }

            if (notification != String.Empty)
                ShowNotification("FileCompare Reforged", notification);

            var screenCapture = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(128, 128));
            ImageSource = screenCapture;

        }

        private FileResult.FileHandlerResult _result;

        public FileResult.FileHandlerResult Result
        {
            get => _result;
            set
            {
                SetProperty(ref _result, value);
                ResultHandler(Result);

                switch (Result)
                {
                    case FileResult.FileHandlerResult.WithFindWord:
                        ButtonVisibility = Visibility.Visible;
                        break;
                    case FileResult.FileHandlerResult.WithChanges:
                        ButtonVisibility = Visibility.Visible;
                        break;
                    default:
                        ButtonVisibility = Visibility.Hidden;
                        break;
                }
            }
        }

        public MainPageVM()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            notifyIcon.BalloonTipClosed += (s, e) => notifyIcon.Visible = false;
            notifyIcon.BalloonTipClicked += (s, e) => Process.Start(MainWindowVM.AltFilePath);
        }

        private void ShowNotification(string title, string message)
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(3000, title, message, ToolTipIcon.Info);
        }
    }
}
