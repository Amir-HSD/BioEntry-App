using BioEntry_App.ViewModel;
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
            try
            {
                this.Visibility = Visibility.Hidden;
                FingerprintView fw = new FingerprintView(this);
                fw.Show();
                this.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                MessageBox.Show("Cant Open FingerRecognition");
                this.Visibility = Visibility.Visible;
            }
        }

        private void FaceRecognitionBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Visibility = Visibility.Hidden;
                FaceRecognitionView frw = new FaceRecognitionView(this);
                frw.Show();
                this.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                this.Visibility = Visibility.Visible;
                MessageBox.Show("Cant Open FaceRecognition");
            }
        }

        public void ShowSuccessView(int Id, string Name, string Family)
        {
            SuccessView successView = new SuccessView(this,Id,Name, Family);
            this.Visibility = Visibility.Hidden;
            successView.Show();
        }
    }
}
