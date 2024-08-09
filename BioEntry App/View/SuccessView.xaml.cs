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
    /// Interaction logic for SuccessView.xaml
    /// </summary>
    
    public partial class SuccessView : Window
    {
        public BiometricView BiometricView;
        public SuccessView(BiometricView biometricView, int Id, string Name, string Family)
        {
            DataContext = new SuccessViewModel(this,Id, Name, Family);
            BiometricView = biometricView;
            InitializeComponent();
        }
    }
}
