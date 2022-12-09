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
    public partial class D2SettingsWindow : UserControl
    {
        bool titleSpawn;
        bool titleGame;
        bool exitonquit;
        string gameLetter = "D2";
        int tempInt;
        int icon;
        IniFile ini;
        List<SpChBtn> btns;
        String tempString;
        String gameTitle;
        String tempTitle;
        String gameID = "D2DV";
        public String windowTitle = "SpawnChat";
        SpChBtn cbtnOK;
        SpChBtn cbtnCancel;
        ImageSource windowIcon;
        BitmapImage d2Icon;
        D2ChatWindow chat;
        D2ChatClient spawnChat;
        public D2SettingsWindow(D2ChatWindow chat, D2ChatClient client)
        {
            InitializeComponent(); ini = new IniFile(System.IO.Path.Combine(Environment.CurrentDirectory, @"config.ini"));
            //windowIcon = new ImageSource();
            AddHotKeys();
            gameTitle = "Diablo II";
            this.chat = chat;
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

        public void setIcon(BitmapImage bmp)
        {
            windowIcon = bmp;
        }

        public void loadSettings()
        {
            try
            {
                chkAutoconnect.IsChecked = (Boolean.Parse(ini.IniReadValue("Settings", "autoconnect")));
            }
            catch (FormatException e)
            {
                chkAutoconnect.IsChecked = false;
            }
            try
            {
                chkELnotif.IsChecked = (Boolean.Parse(ini.IniReadValue("Settings", "enterleave")));
            }
            catch (FormatException e)
            {
                chkELnotif.IsChecked = false;
            }
            try
            {
                chkTime.IsChecked = (Boolean.Parse(ini.IniReadValue("Settings", "timestamp")));
            }
            catch (FormatException e)
            {
                chkTime.IsChecked = false;
            }
            try
            {
                chkExit.IsChecked = (Boolean.Parse(ini.IniReadValue("Settings", "exitonquit")));
            }
            catch (FormatException e)
            {
                chkExit.IsChecked = false;
            }
            try
            {
                chkAds.IsChecked = (Boolean.Parse(ini.IniReadValue("Settings", "ads")));
            }
            catch (FormatException e)
            {
                chkAds.IsChecked = false;
            }
            try
            {
                chkTSpawn.IsChecked = (Boolean.Parse(ini.IniReadValue("Window", "windowTSpawn")));
            }
            catch (FormatException e)
            {
                chkTSpawn.IsChecked = false;
            }
            try
            {
                chkTGame.IsChecked = (Boolean.Parse(ini.IniReadValue("Window", "windowTGame")));
            }
            catch (FormatException e)
            {
                chkTGame.IsChecked = false;
            }
            try
            {
                chkFullscreen.IsChecked = (Boolean.Parse(ini.IniReadValue("Window", "fullscreen")));
            }
            catch (FormatException e)
            {
                chkFullscreen.IsChecked = false;
            }
            try
            {
                icon = int.Parse(ini.IniReadValue(gameID, "icon"));
            }
            catch (FormatException e)
            {
                icon = 0;
            }
            switch (icon)
            {
                case 1:
                    radIconD2X.IsChecked = true;
                    break;
                default:
                    radIconD2.IsChecked = true;
                    break;
            }
            
            //updateIconPreview();
            titleSpawn = chkTSpawn.IsChecked.Value;
            titleGame = chkTGame.IsChecked.Value;
            UpdateTitle();
            windowIcon = d2Icon;
            chat.Icon = windowIcon;
        }

        public void UpdateTitle()
        {
            tempTitle = windowTitle;
            if (titleSpawn)
            {
                windowTitle = "SpawnChat";
                if (titleGame)
                {
                    windowTitle = "SpawnChat - " + gameTitle;
                }
            }
            else if (titleGame)
            {
                windowTitle = gameTitle;
            }

        }


        public void saveSettings()
        {
            ini.IniWriteValue("Settings", "autoconnect", chkAutoconnect.IsChecked.ToString());
            ini.IniWriteValue("Settings", "enterleave", chkELnotif.IsChecked.ToString());
            ini.IniWriteValue("Settings", "timestamp", chkTime.IsChecked.ToString());
            ini.IniWriteValue("Settings", "exitonquit", chkExit.IsChecked.ToString());
            ini.IniWriteValue("Settings", "ads", chkAds.IsChecked.ToString());
            ini.IniWriteValue("Window", "windowTSpawn", chkTSpawn.IsChecked.ToString());
            ini.IniWriteValue("Window", "windowTGame", chkTGame.IsChecked.ToString());
            ini.IniWriteValue("Window", "fullscreen", chkFullscreen.IsChecked.ToString());
            ini.IniWriteValue(gameID, "icon", icon.ToString());
            windowIcon = d2Icon;
            chat.Title = windowTitle;
            chat.Icon = windowIcon;
            chat.updateSettings(chkTime.IsChecked.Value, chkFullscreen.IsChecked.Value, chkExit.IsChecked.Value);
            spawnChat.updateSettings(chkELnotif.IsChecked.Value);
        }

        private void chkAutoconnect_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkELnotif_Checked(object sender, RoutedEventArgs e)
        {

        }


        private void chkTSpawn_Checked(object sender, RoutedEventArgs e)
        {
            titleSpawn = true;
            UpdateTitle();
        }

        private void chkTSpawn_Unchecked(object sender, RoutedEventArgs e)
        {
            titleSpawn = false;
            UpdateTitle();
        }

        private void chkTGame_Checked(object sender, RoutedEventArgs e)
        {
            titleGame = true;
            UpdateTitle();
        }

        private void chkTGame_Unchecked(object sender, RoutedEventArgs e)
        {
            titleGame = false;
            UpdateTitle();
        }

        public void updateIconPreview()
        {
            switch (icon)
            {
                case 0:
                    tempString = "icon_d2";
                    break;
                default:
                    tempString = "icon_d2x";
                    break;
            }
            try
            {
                d2Icon = ResourceHelper.LoadBitmapFromResource("img/icons/" + tempString + ".png");
            }
            catch (NullReferenceException e)
            {

            }
            
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
            windowTitle = tempTitle;
            chat.selectMsgBox();
        }

        private void cboxIcon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateIconPreview();
        }

        private void radIconD2_Checked(object sender, RoutedEventArgs e)
        {
            icon = 0;
            updateIconPreview();
        }

        private void radIconD2X_Checked(object sender, RoutedEventArgs e)
        {
            icon = 1;
            updateIconPreview();
        }
    }
}
