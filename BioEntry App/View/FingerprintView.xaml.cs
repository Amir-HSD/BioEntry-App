using BioEntry_App.ViewModel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BioEntry_App.View
{
    /// <summary>
    /// Interaction logic for FingerprintView.xaml
    /// </summary>
    public partial class FingerprintView : Window
    {
        FingerPrintViewModel viewModel;
        public BiometricView BiometricView;
        public FingerprintView(BiometricView biometricView)
        {
            viewModel = new FingerPrintViewModel(this);
            this.DataContext = viewModel;
            InitializeComponent();
            BiometricView = biometricView;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FingerPrintWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.ScanFinger();
        }

        private void FingerPrintWindow_Closed(object sender, EventArgs e)
        {
            viewModel.OpenCloseSerialPort();
            BiometricView.Visibility = Visibility.Visible;
        }
    }
}
