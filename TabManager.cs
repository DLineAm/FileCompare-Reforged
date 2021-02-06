using System.Windows;
using System.Windows.Controls;
using FileCompare_Reforged.View;
using FileCompare_Reforged.ViewModel;

namespace FileCompare_Reforged
{
    public class TabManager
    {
        private MainWindow MainWindow;
        public void SetMainTab(object vm)
        {
            if (vm is MainPageVM mainPage)
            {
                //MainWindow.MainTab = new TabItem() {Content = new MainPage() {DataContext = mainPage}, DataContext = mainPage};
                MainWindow.MainTab.Content = new MainPage(){DataContext = mainPage};
            }
        }

        public void SetSettingsTab(object vm)
        {
            if (vm is SettignsPageVM settignsPage)
            {
                MainWindow.SettingsTab.Content = new SettingsPage() {DataContext = settignsPage};
            }
        }

        public TabManager(MainWindow window)
        {
            if (window != null)
                MainWindow = window;
        }

        public void ShowView()
        {
            MainWindow.Show();
        }
    }
}