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
using System.IO;
using Ini;
using SCommon;

namespace SChat
{
    /// <summary>
    /// Interaction logic for DSettingsWindow.xaml
    /// </summary>
    public partial class WSettingsWindow : Window
    {
        bool titleSpawn;
        bool titleGame;
        bool exitonquit;
        char gameLetter = 'W';
        int tempInt;
        IniFile ini;
        List<SpChBtn> btns;
        String tempString;
        String gameTitle;
        String tempTitle;
        String gameID = "w2bn";
        SpChBtn cbtnOK;
        SpChBtn cbtnCancel;
        WChatWindow chat;
        WChatClient spawnChat;
        public WSettingsWindow(WChatWindow chat, WChatClient client)
        {
            InitializeComponent();
            ini = new IniFile(System.IO.Path.Combine(Environment.CurrentDirectory, @"config.ini"));
            AddHotKeys();
            gameTitle = "Warcraft II";
            this.chat = chat;
            spawnChat = client;
            btns = new List<SpChBtn>();
            loadSettings();
            setupButtons();
        }

        private void setupButtons()
        {
            cbtnOK = new SpChBtn(btnOK, lblBtnOK, gameID + "/btn" + gameLetter + "Chat", gameID + "/btn" + gameLetter + "chatPressed", 14);
            btns.Add(cbtnOK);
            cbtnCancel = new SpChBtn(btnCancel, lblBtnCancel, gameID + "/btn" + gameLetter + "Chat", gameID + "/btn" + gameLetter + "chatPressed", 14);
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

        public void setIcon(BitmapImage bmp) {
            this.Icon = bmp;
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
                cBoxDClass.SelectedIndex = int.Parse(ini.IniReadValue(gameID, "char"));
            }
            catch (FormatException e)
            {
                cBoxDClass.SelectedIndex = 0;
            }

            try
            {
                cboxWins.SelectedIndex = int.Parse(ini.IniReadValue(gameID, "wins"));
            }
            catch (FormatException e)
            {
                cboxWins.SelectedIndex = 0;
            }
            updateAvPreview();
            titleSpawn = chkTSpawn.IsChecked.Value;
            titleGame = chkTGame.IsChecked.Value;
            UpdateTitle();
        }

        public void UpdateTitle()
        {
            tempTitle = this.Title;
            if (titleSpawn)
            {
                this.Title = "SpawnChat";
                if (titleGame)
                {
                    this.Title = "SpawnChat - " + gameTitle;
                }
            }
            else if (titleGame)
            {
                this.Title = gameTitle;
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
            ini.IniWriteValue(gameID, "char", cBoxDClass.SelectedIndex.ToString());
            ini.IniWriteValue(gameID, "wins", cboxWins.SelectedIndex.ToString());
            this.Hide();
            chat.Title = this.Title;
            chat.updateSettings(chkTime.IsChecked.Value, chkFullscreen.IsChecked.Value, chkExit.IsChecked.Value, cBoxDClass.SelectedIndex, cboxWins.SelectedIndex);
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

        private void cBoxDClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cBoxDClass.SelectedIndex != 1)
                {
                    cboxWins.IsEnabled = false;
                }
                else
                {
                    cboxWins.IsEnabled = true;
                }
            }
            catch (NullReferenceException ex)
            {

            }
            updateAvPreview();
        }

        public void updateAvPreview()
        {
            tempString = gameID + "/";
            switch (cBoxDClass.SelectedIndex)
            {
                case 0:
                    tempString = "spawn";
                    break;
                case 1:
                    tempString += "wR";
                    break;
                case 2:
                    tempString += "blue";
                    break;
                case 3:
                    tempString += "iron";
                    break;
                default:
                    break;
            }

            if (cBoxDClass.SelectedIndex == 1)
            {
                tempString += cboxWins.SelectedIndex;
            }
            
            //Console.WriteLine("AVATAR PREVIEW SOURCE: " + tempString);
            imgAvPreview.Source = ResourceHelper.LoadBitmapFromResource("img/avatars/" + tempString + ".png");
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
            this.Title = tempTitle;
            this.Hide();
            chat.selectMsgBox();
        }

        private void cboxWins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateAvPreview();
        }
    }
}
