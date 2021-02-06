using FileCompare_Reforged.ViewModel;
using System.Windows;

namespace FileCompare_Reforged
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RunProgramLogic();
        }

        public static MainPageVM MainPage { get; set; }

        public static SettignsPageVM SettingsPage { get; set; }

        private TabManager _manager;

        private void RunProgramLogic()
        {
            MainPage = new MainPageVM();
            MainWindow window = new MainWindow();
            SettingsPage = new SettignsPageVM();
            _manager = new TabManager(window);
            _manager.SetMainTab(MainPage);
            _manager.SetSettingsTab(SettingsPage);
            _manager.ShowView();
        }

    }
}
