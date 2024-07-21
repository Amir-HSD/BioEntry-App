using System;
using System.Collections.Generic;
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
    /// Interaction logic for BiometricView.xaml
    /// </summary>
    public partial class BiometricView : Window
    {
        public BiometricView()
        {
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FingerprintBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            FingerprintView fw = new FingerprintView();
            bool? check = fw.ShowDialog();
            if (check != null && check != true)
            {
                this.Visibility = Visibility.Visible;
            }
            else
            {
                if(Application.Current.MainWindow.IsActive)
                {
                    this.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Error To Load MainWindow");
                    App.Current.Shutdown();
                }
            }
            
        }

        private void FaceRecognitionBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            FaceRecognitionView frw = new FaceRecognitionView();
            bool? check = frw.ShowDialog();
            if (check != null && check != true)
            {
                this.Visibility = Visibility.Visible;
            }
            else
            {
                if (Application.Current.MainWindow.IsActive)
                {
                    this.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Error To Load MainWindow");
                    App.Current.Shutdown();
                }
            }
        }
    }
}
