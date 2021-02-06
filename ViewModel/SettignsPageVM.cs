using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FileCompare_Reforged.Properties;
using Prism.Commands;
using Prism.Mvvm;
using static FileCompare_Reforged.DownloadTimer;

namespace FileCompare_Reforged.ViewModel
{
    public class SettignsPageVM : BindableBase
    {
        Properties.Settings settings = new Settings();

        private bool _autoDownload;
        public bool AutoDownload
        {
            get => _autoDownload = settings.AutoDownload;
            set
            {
                SetProperty(ref _autoDownload, value);

                settings.AutoDownload = _autoDownload;
                settings.Save();

                SetTimer();
                CheckChecks();
            }
        }

        private bool _autoLaunch;
        public bool AutoLaunch
        {
            get
            {
                return _autoLaunch = settings.AutoLaunch;
            }
            set
            {
                SetProperty(ref _autoLaunch, value);

                settings.AutoLaunch = _autoLaunch;
                settings.Save();

                SetAutoload(_autoLaunch);
                CheckChecks();
            }
        }

        private void CheckChecks()
        {
            if (AutoDownload && AutoLaunch)
                Checks++;
            if (Checks == 4)
                ButtonVisibility = Visibility.Visible;
        }

        private int Checks = 0;
        private void SetTimer()
        {
            if (AutoDownload)
                Timer.Start();
            else
                Timer.Stop();
        }

        private void SetAutoload(bool set) {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
            if (set) {
                key.SetValue("FileCompare Reforged", "\"" + AppDomain.CurrentDomain.BaseDirectory + "FileCompare Reforged.exe" + "\"");
            } else {
                key.DeleteValue("FileCompare Reforged", false);
            }
            key.Close();
        }

        private Visibility _buttonVisibility = Visibility.Hidden;
        public Visibility ButtonVisibility
        {
            get => _buttonVisibility;
            set => SetProperty(ref _buttonVisibility, value);
        }

        private int _progressValue;

        public int ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        private DelegateCommand _openFileCommand;
        public DelegateCommand OpenFileCommand =>
            _openFileCommand ??= new DelegateCommand(() =>
            {
                Process.Start(MainWindowVM.FilePath);
            });

        public DelegateCommand ApplyCommand { get; set; }

        public SettignsPageVM()
        {
            SetTimer();
            ProgressValue = 5;
            ApplyCommand = new DelegateCommand(() => { Process.Start(settings.FilePath); }, () => true);
        }
    }
}
