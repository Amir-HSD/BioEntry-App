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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BioEntry_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel MWVM = new MainWindowViewModel();
        public MainWindow()
        {
            this.DataContext = MWVM;
            InitializeComponent();
        }
        
        public void PageLoaded(object sender, RoutedEventArgs e)
        {
            MWVM.OnLoaded();
        }
    }
}
