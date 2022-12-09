using d2c;
using d2music;
using Ini;
using SCommon;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// THIS FEATURE HAS NOT YET BEEN IMPLEMENTED.
    /// </summary>
    public partial class D2SoundSettings : UserControl
    {
        string gameLetter = "D2";
        int tempInt;
        double musicVolume;
        bool musicClassic;
        bool musicExpansion;
        IniFile ini;
        List<SpChBtn> btns;
        String tempString;
        String gameID = "D2DV";
        SpChBtn cbtnOK;
        SpChBtn cbtnCancel;
        d2avatar av;
        d2avatar avPreview;
        D2ChatClient spawnChat;
        Stream fileTester;
        D2MusicPlayer musicPlayer;
        List<string> classicList;
        List<string> lodList;
        public D2SoundSettings(D2ChatClient client)
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
                musicVolume = double.Parse(ini.IniReadValue(gameID, "musicvol"));
                if (musicVolume < 0)
                {
                    musicVolume = 0;
                }
                if (musicVolume > 100)
                {
                    musicVolume = 100;
                }
            }
            catch (FormatException e)
            {
                musicVolume = 50;
            }
            slideMusicVol.Value = musicVolume;
            try
            {
                chkClassic.IsChecked = (Boolean.Parse(ini.IniReadValue(gameID, "classicmusic")));
            }
            catch (FormatException e)
            {
                chkClassic.IsChecked = false;
            }
            musicClassic = chkClassic.IsChecked.Value;
            try
            {
                chkExpansion.IsChecked = (Boolean.Parse(ini.IniReadValue(gameID, "expmusic")));
            }
            catch (FormatException e)
            {
                chkExpansion.IsChecked = false;
            }
            musicExpansion = chkExpansion.IsChecked.Value;
            testFiles();
            if (musicPlayer == null)
            {
                musicPlayer = new D2MusicPlayer(chkClassic.IsChecked.Value, chkExpansion.IsChecked.Value, musicVolume);
            }

        }

        public void testFiles()
        {
            classicList = new List<string>();
            lodList = new List<string>();
            classicList.Add("options");
            classicList.Add("caves");
            classicList.Add("monastery");
            classicList.Add("crypt");
            classicList.Add("harem");
            classicList.Add("spider");
            lodList.Add("baal");
            lodList.Add("sewer");
            lodList.Add("desert");
            lodList.Add("kurastsewer");
            lodList.Add("diablo");
            lodList.Add("icecaves");
        }

        public void saveSettings()
        {
            ini.IniWriteValue(gameID, "classicmusic", chkClassic.IsChecked.ToString());
            ini.IniWriteValue(gameID, "expmusic", chkExpansion.IsChecked.ToString());
            ini.IniWriteValue(gameID, "musicvol", Math.Floor(musicVolume).ToString());
            spawnChat.closeSettings();
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            musicVolume = slideMusicVol.Value;

        }

    }
}
