using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ExamWpfApp.Models.Dictionary;
using ExamWpfApp.ViewModels;

namespace ExamWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();

            var langDict = new LanguageDictionary();
            var mainWindowViewModel = new MainWindowViewModel(langDict, mainWindow);
            
            mainWindow.DataContext = mainWindowViewModel;
            mainWindow.Show();
        }
    }
}