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

namespace SChat
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DError : Window
    {
        SpChBtn cbtnOK;

        public DError(String error)
        {
            InitializeComponent();
            lblErrormsg.Text = error;
            AddHotkeys();
            setupButtons();
        }

        public void setupButtons()
        {
            cbtnOK = new SpChBtn(btnOK, lblBtnOK, "drtl/btnDError", "drtl/btnDErrorPressed", 14);  
        }

        public void AddHotkeys()
        {
            try
            {
                RoutedCommand enter = new RoutedCommand();
                enter.InputGestures.Add(new KeyGesture(Key.Enter, ModifierKeys.None));
                CommandBindings.Add(new CommandBinding(enter, btnOK_Click));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void btnOK_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnOK.press();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            cbtnOK.dePress();
            this.Close();
        }

    }
}
