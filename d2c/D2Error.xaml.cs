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

namespace d2c
{
    /// <summary>
    /// Interaction logic for D2BlankPanel.xaml
    /// </summary>
    public partial class D2Error : UserControl
    {
        public D2Error(String message)
        {
            InitializeComponent();
            tbError.Text = message;
        }
    }
}
