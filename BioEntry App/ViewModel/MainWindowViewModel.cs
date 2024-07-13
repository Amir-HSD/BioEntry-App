using BioEntry_App.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BioEntry_App.ViewModel
{
    internal class MainWindowViewModel: ViewModelBase
    {
        
        public RelayCommand LoadedCommand => new RelayCommand(onExecute => { OnLoaded(); }, onExecuteChanged => { return true; });
        public MainWindowViewModel()
        {
            
        }

        public async void OnLoaded()
        {
            var CheckApi = await ApiService.CheckStatusAsync("https://http.cat/200");
            var CheckBoard = await ApiService.CheckStatusAsync("https://http.cat/200");
            if (CheckApi != false || CheckBoard != false)
            {
                BiometricView BW = new BiometricView();
                BW.Show();
                Application.Current.MainWindow.Hide();
            }

        }

    }
}
