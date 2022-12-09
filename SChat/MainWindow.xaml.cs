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
using System.IO;
using Ini;

namespace SChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IniFile settings;
        Random rnd;
        bool autoconnect = false;
        int game;
        public MainWindow()
        {
            InitializeComponent();
            AddHotKeys();
            rnd = new Random();
            settings = new IniFile(System.IO.Path.Combine(Environment.CurrentDirectory, @"config.ini"));
            txtName.Text = settings.IniReadValue("Settings", "nick");
            if (txtName.Text.Length > 15)
            {
                txtName.Text = txtName.Text.Substring(0, 15);
            }
            txtChan.Text = settings.IniReadValue("Settings", "channel");
            if (txtChan.Text.Length > 22)
            {
                txtChan.Text = txtChan.Text.Substring(0, 22);
            }
            cboxServer.Text = (settings.IniReadValue("Settings", "server"));
            try
            {
                chkNickServ.IsChecked = (Boolean.Parse(settings.IniReadValue("NickServ", "enabled")));
            }
            catch (FormatException e)
            {
                chkNickServ.IsChecked = false;
            }
            if (chkNickServ.IsChecked.Value)
            {
                txtNSname.IsEnabled = true;
                chkRememberPass.IsEnabled = true;
                pboxNSpass.IsEnabled =true;
                txtNSname.Text = settings.IniReadValue("NickServ", "botname");
                try
                {
                    chkRememberPass.IsChecked = (Boolean.Parse(settings.IniReadValue("NickServ", "rempass")));
                }
                catch (FormatException e)
                {
                    chkRememberPass.IsChecked = false;
                }
                if (chkRememberPass.IsChecked.Value)
                {
                    pboxNSpass.Password = settings.IniReadValue("NickServ", "pass");
                }
                else
                {
                    pboxNSpass.Focus();
                }
            }
            cboxServer.Items.Add("irc.rizon.net");
            cboxServer.Items.Add("irc.freenode.net");
            cboxServer.Items.Add("irc.quakenet.org");
            cboxServer.Items.Add("irc.mibbit.net");
            cboxServer.Items.Add("irc.synirc.net");
            cboxGame.Items.Add("STAR");
            cboxGame.Items.Add("W2BN");
            cboxGame.Items.Add("D2DV");
            cboxGame.Items.Add("CHAT");

            try
            {
                game = (int.Parse(settings.IniReadValue("Settings", "client")));
            }
            catch (FormatException e)
            {
                game = 4;
            }
            try
            {
                cboxGame.SelectedIndex = game;
                //cboxServer.SelectedIndex = 0; //DEBUG CLIENT OVERRIDE
            }
            catch (ArgumentOutOfRangeException e)
            {
                cboxGame.SelectedIndex = 0;
                game = cboxGame.SelectedIndex;
            }

            try
            {
                autoconnect = Boolean.Parse(settings.IniReadValue("Settings", "autoconnect"));
            }
            catch (FormatException e)
            {
                autoconnect = false;
            }
            if (autoconnect)
            {
                connect();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            game = cboxGame.SelectedIndex;
            if (game == 4)
            {
                game = rnd.Next(4);
            }
            settings.IniWriteValue("Settings", "nick", txtName.Text);
            settings.IniWriteValue("Settings", "channel", txtChan.Text);
            settings.IniWriteValue("Settings", "server", cboxServer.Text);
            settings.IniWriteValue("Settings", "client", cboxGame.SelectedIndex.ToString());
            settings.IniWriteValue("NickServ", "enabled", chkNickServ.IsChecked.ToString());
            if (chkNickServ.IsChecked.Value)
            {
                settings.IniWriteValue("NickServ", "botname", txtNSname.Text);
                settings.IniWriteValue("NickServ", "rempass", chkRememberPass.IsChecked.ToString());
                if (chkRememberPass.IsChecked.Value)
                {
                    settings.IniWriteValue("NickServ", "pass", pboxNSpass.Password);
                }
                else
                {
                    settings.IniWriteValue("NickServ", "pass", ""); 
                }
            }
            connect();
        }

        private void AddHotKeys()
        {
            try
            {
                RoutedCommand enter = new RoutedCommand();
                enter.InputGestures.Add(new KeyGesture(Key.Enter, ModifierKeys.None));
                CommandBindings.Add(new CommandBinding(enter, btnOK_Click));

                RoutedCommand altX = new RoutedCommand();
                altX.InputGestures.Add(new KeyGesture(Key.X, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(altX, btnExit_Click));
            }
            catch (Exception err)
            {
                //handle exception error
            }
        }

        public void connect()
        {
            switch (game)
            {
                case 1:
                    SChatClient starChat = new SChatClient(txtName.Text, cboxServer.Text, "#" + txtChan.Text, game, this, chkNickServ.IsChecked.Value, pboxNSpass.Password, txtNSname.Text);
                    break;
                case 2:
                    WChatClient war2Chat = new WChatClient(txtName.Text, cboxServer.Text, "#" + txtChan.Text, game, this, chkNickServ.IsChecked.Value, pboxNSpass.Password, txtNSname.Text);
                    break;
                case 3:
                    D2ChatClient d2Chat = new D2ChatClient(txtName.Text, cboxServer.Text, "#" + txtChan.Text, game, this, chkNickServ.IsChecked.Value, pboxNSpass.Password, txtNSname.Text);
                    break;
                default:
                    DChatClient diabloChat = new DChatClient(txtName.Text, cboxServer.Text, "#" + txtChan.Text, game, this, chkNickServ.IsChecked.Value, pboxNSpass.Password, txtNSname.Text);
                    break;
            }
            this.Hide();
        }

        private void cboxServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cboxGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (game == 4)
            {
                game = rnd.Next(4);
            }
            else
            {
                game = cboxGame.SelectedIndex;
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
                Application.Current.Shutdown();
        }

        private void chkNickServ_Checked(object sender, RoutedEventArgs e)
        {
            pboxNSpass.IsEnabled = true;
            txtNSname.IsEnabled = true;
            chkRememberPass.IsEnabled = true;
        }

        private void chkNickServ_Unchecked(object sender, RoutedEventArgs e)
        {
            pboxNSpass.IsEnabled = false;
            txtNSname.IsEnabled = false;
            chkRememberPass.IsEnabled = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
