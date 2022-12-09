using d2c;
using Ini;
using SCommon;
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

namespace SChat
{
    /// <summary>
    /// Interaction logic for D2SettingsWindow.xaml
    /// </summary>
    public partial class D2CharBuilder : UserControl
    {
        string gameLetter = "D2";
        int tempInt;
        int avChar;
        int avArmor;
        IniFile ini;
        List<SpChBtn> btns;
        String tempString;
        String gameID = "D2DV";
        SpChBtn cbtnOK;
        SpChBtn cbtnCancel;
        d2avatar av;
        d2avatar avPreview;
        D2ChatClient spawnChat;
        public D2CharBuilder(D2ChatClient client)
        {
            InitializeComponent(); ini = new IniFile(System.IO.Path.Combine(Environment.CurrentDirectory, @"config.ini"));
            av = new d2avatar();
            DataContext = av;
            AddHotKeys();
            spawnChat = client;
            btns = new List<SpChBtn>();
            loadSettings();
            setupButtons();
        }

        private void setupButtons()
        {
            cbtnOK = new SpChBtn(btnOK, lblBtnOK, gameID + "/btn" + gameLetter + "Menu", gameID + "/btn" + gameLetter + "MenuPressed", 16);
            btns.Add(cbtnOK);
            cbtnCancel = new SpChBtn(btnCancel, lblBtnCancel, gameID + "/btn" + gameLetter + "Menu", gameID + "/btn" + gameLetter + "MenuPressed", 16);
            btns.Add(cbtnCancel);
        }

        private void depressOtherButtons(SpChBtn button)
        {
            foreach (SpChBtn b in btns)
            {
                if (!b.Equals(button))
                {
                    b.dePress();
                }
            }
        }

        private void AddHotKeys()
        {
            try
            {
                RoutedCommand enter = new RoutedCommand();
                enter.InputGestures.Add(new KeyGesture(Key.Enter, ModifierKeys.None));
                CommandBindings.Add(new CommandBinding(enter, btnOK_Click));

                RoutedCommand cancel = new RoutedCommand();
                cancel.InputGestures.Add(new KeyGesture(Key.Escape, ModifierKeys.None));
                CommandBindings.Add(new CommandBinding(cancel, btnCancel_Click));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void loadSettings()
        {
            try
            {
                avChar = int.Parse(ini.IniReadValue("D2DV", "char"));
                if (avChar < 0 || avChar > 6)
                {
                    avChar = 5;
                }
            }
            catch (FormatException e)
            {
                avChar = 5;
            }
            if (avChar >= 0 && avChar < 5)
            {
                cboxClass.SelectedIndex = int.Parse(ini.IniReadValue("D2DV", "char"));
            }
            switch (avChar)
            {
                case 5:
                    radDefault.IsChecked = true;
                    break;
                case 6:
                    radDead.IsChecked = true;
                    break;
                default:
                    radCustom.IsChecked = true;
                    cboxClass.IsEnabled = true;
                    cboxArmor.IsEnabled = true;
                    break;
            }

            try
            {
                avArmor = int.Parse(ini.IniReadValue("D2DV", "armor"));
            }
            catch (FormatException e)
            {
                avArmor = 0;
            }
            try
            {
                cboxArmor.SelectedIndex = avArmor;
            }
            catch (ArgumentOutOfRangeException e)
            {
                cboxArmor.SelectedIndex = 0;
            }
            updateAvPreview();
        }

        public void updateAvPreview()
        {
            try
            {
                //avatar.setClass(avChar);
                //avatar.setPreset(avArmor);
                avPreview = new d2avatar(avChar, avArmor);
                charPreview.Content = avPreview;
                //avatarDisplay.DataContext = avPreview;
                //avatarDisplay.Content = avPreview;
            }
            catch (NullReferenceException e)
            {

            }
        }


        public void saveSettings()
        {
            ini.IniWriteValue("D2DV", "char", avChar.ToString());
            ini.IniWriteValue("D2DV", "armor", avArmor.ToString());
            spawnChat.updateUserAvatar(avChar, avArmor);
            spawnChat.closeSettings();
        }

        private void cboxClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            avChar = cboxClass.SelectedIndex;
            updateAvPreview();
        }

        private void cboxArmor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            avArmor = cboxArmor.SelectedIndex;
            updateAvPreview();
        }

        private void radDefault_Checked(object sender, RoutedEventArgs e)
        {
            avChar = 5;
            cboxClass.IsEnabled = false;
            cboxArmor.IsEnabled = false;
            updateAvPreview();
        }

        private void radCustom_Checked(object sender, RoutedEventArgs e)
        {
            avChar = cboxClass.SelectedIndex;
            cboxClass.IsEnabled = true;
            cboxArmor.IsEnabled = true;
            updateAvPreview();
        }

        private void radDead_Checked(object sender, RoutedEventArgs e)
        {
            avChar = 6;
            cboxClass.IsEnabled = false;
            cboxArmor.IsEnabled = false;
            updateAvPreview();
        }

        private void btnOK_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnOK.press();
            depressOtherButtons(cbtnOK);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            cbtnOK.dePress();
            saveSettings();
        }

        private void btnCancel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnCancel.press();
            depressOtherButtons(cbtnCancel);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            cbtnCancel.dePress();
            spawnChat.closeSettings();
        }

    }
}
