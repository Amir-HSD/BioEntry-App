using BioEntry_App.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioEntry_App.ViewModel
{
    internal class SuccessViewModel : ViewModelBase
    {
        SuccessView SuccessView;

        public RelayCommand TrueCommand => new RelayCommand(onExecute => { TrueUser(); }, onExecuteChanged => { return true; });
        public RelayCommand FalseCommand => new RelayCommand(onExecute => { FalseUser(); }, onExecuteChanged => { return true; });
        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged(); }
        }

        public SuccessViewModel(SuccessView successview, int Id, string Name, string Family)
        {
            SuccessView = successview;
            Username = Name + " " + Family;
        }

        private void TrueUser()
        {
            SuccessView.BiometricView.Visibility = System.Windows.Visibility.Visible;
            SuccessView.Close();
            // Do something else
        }

        private void FalseUser()
        {
            SuccessView.BiometricView.Visibility = System.Windows.Visibility.Visible;
            SuccessView.Close();
            // Do something else
        }

    }
}
