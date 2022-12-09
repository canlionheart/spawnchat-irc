using d2c;
using Ini;
using SCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
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
    public partial class D2ChatWindow : Window
    {
        TextRange tempRange;
        BitmapImage tempBmp;
        String tempString;
        public String chan { get; set; }
        public String nick { get; set; }
        String avBuilder;
        String tempName;
        String avatar;
        String gameID;
        String[] tempStrArray;
        String[] tempDelims;
        List<SpChBtn> btns;
        List<String> chatIcon;
        String appIcon;
        IniFile ini;
        Random rnd;
        MainWindow mw;
        SpChBtn cbtnSettings;
        SpChBtn cbtnQuit;
        SpChBtn cbtnSend;
        SpChBtn cbtnWhis;
        SpChBtn cbtnGem;
        SpChBtn cbtnChar;
        SpChBtn cbtnSquelch;
        SpChBtn cbtnUnsq;
        SpChBtn cbtnEmote;
        SpChBtn cbtnSound;
        SpChBtn arrowLeft;
        SpChBtn arrowLeftBig;
        SpChBtn arrowRight;
        SpChBtn arrowRightBig;
        ScrollViewer sv;
        Uri tempUri;
        int tempInt;
        int numChatIcons;
        int selectedUser;
        public int avChar;
        public int avArmor;
        public bool elnotif;
        bool tempBool;
        bool exitOnQuit = true;
        public bool timestamps = false;
        bool fullscreen;
        char[] tempCharArr;
        //private SoundPlayer wavBtnClick = new SoundPlayer(Application.GetResourceStream(ResourceHelper.LoadResource("sfx/guiclick.mp3")).Stream);
        public D2ChatClient spawnChat;
        d2avatar tempItem;
        //ObservableCollection<d2Item> items = new ObservableCollection<d2Item>();
        public D2ChatWindow(string username, string channel, MainWindow main, D2ChatClient client)
        {
            InitializeComponent();
            gameID = "D2DV";
            ini = new IniFile(System.IO.Path.Combine(Environment.CurrentDirectory, @"config.ini"));
            rnd = new Random();
            AddHotKeys();
            mw = main;
            d2avatar t = new d2avatar();
            
            //Border border = (Border)VisualTreeHelper.GetChild(lstUsers, 0);
            //sv = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
            DataContext = t;
            nick = username;
            chan = channel;
            //setBanner();
            btns = new List<SpChBtn>();
            spawnChat = client;
            try
            {
                elnotif = (Boolean.Parse(ini.IniReadValue("Settings", "enterleave")));
            }
            catch (FormatException e)
            {
                elnotif = false;
            }
            try
            {
                timestamps = (Boolean.Parse(ini.IniReadValue("Settings", "timestamp")));
            }
            catch (FormatException e)
            {
                timestamps = false;
            }
            try
            {
                fullscreen = (Boolean.Parse(ini.IniReadValue("Window", "fullscreen")));
            }
            catch (FormatException e)
            {
                fullscreen = false;
            }
            try
            {
                exitOnQuit = (Boolean.Parse(ini.IniReadValue("Settings", "exitonquit")));
            }
            catch (FormatException e)
            {
                exitOnQuit = false;
            }
            try
            {
                avChar = int.Parse(ini.IniReadValue(gameID, "char"));

            }
            catch (FormatException e)
            {
                avChar = 0;

            }
            try
            {
                avArmor = int.Parse(ini.IniReadValue(gameID, "armor"));
            }
            catch (FormatException e)
            {
                avArmor = 0;
            }
            //tempUri = new Uri(@"pack://application:,,,/" + Assembly.GetCallingAssembly().GetName().Name + ";component/" + avatar, UriKind.Absolute);
            //tempBmp = new BitmapImage(tempUri);

            setupButtons();

            if (fullscreen)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }
        }


        private void toggleEL(object sender, RoutedEventArgs e)
        {
            spawnChat.toggleEL();
        }

        private void toggleTime(object sender, RoutedEventArgs e)
        {
            if (timestamps)
            {
                timestamps = false;
                AddToChatWindow(3, null, "Chat Timestamps: OFF");
            }
            else
            {
                timestamps = true;
                AddToChatWindow(3, null, "Chat Timestamps: ON");
            }

            ini.IniWriteValue("Settings", "timestamp", timestamps.ToString());
        }


        private void toggleFullscreen(object sender, RoutedEventArgs e)
        {
            toggleFScreen();
        }

        private void toggleFScreen()
        {
            if (this.WindowState == WindowState.Maximized && this.WindowStyle == WindowStyle.None)
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                fullscreen = false;
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                fullscreen = true;
            }
            ini.IniWriteValue("Window", "fullscreen", fullscreen.ToString());
        }

        public Canvas getCanvas()
        {
            return canChat;
        }

        public ContentControl getPanel()
        {
            return menuPanel;
        }

        public Image getBanner()
        {
            return bannerAd;
        }

        public void updateSettings(bool timestamps, bool fullscreen, bool exonq)
        {
            exitOnQuit = exonq;
            this.timestamps = timestamps;
            if (!(this.fullscreen.Equals(fullscreen)))
            {
                toggleFScreen();
            }
            selectMsgBox();
        }

        public void pasteName(object sender, RoutedEventArgs e)
        {
            try
            {
                txtChat.AppendText(" " + spawnChat.getUserById(lstUsers.SelectedIndex) + " ");
                txtChat.SelectionStart = txtChat.Text.Length;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                AddToChatWindow(7, null, "You must first select a user from the list.");
            }
        }

        public void scrollToEnd()
        {
            rtbOutput.ScrollToEnd();
        }

        public bool scrolledToBot()
        {
            return rtbOutput.VerticalOffset + rtbOutput.ViewportHeight == rtbOutput.ExtentHeight;
        }

        public void theVoid()
        {
            chan = "#The Void";
            lblChan.Content = "The Void (0)";
        }

        public void AddToChatWindow(string message)
        {
            rtbOutput.AppendText(message + "\n");
            rtbOutput.ScrollToEnd();
        }

        public void AddToChatWindow(int type, string user, string message)
        {
            if (timestamps)
            {
                if (!(type == 3 || type == 7 || type == 8 || type == 10))
                {
                    tempString = "[" + System.DateTime.Now.ToShortTimeString() + "] ";
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = tempString.ToLower();
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#C4C4C4"));
                }
            }

            switch (type)
            {
                case 1: //Emote
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "*" + user + " " + message + "* \n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#505050"));
                    break;

                case 2: //Whisper
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = user + " whispers: " + message + " \n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#18FC00"));
                    break;

                case 3: //System
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = user + message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#5050AC"));
                    break;

                case 4: //User message
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<" + user + "> ";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Aqua);
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                    break;

                case 5: //User whisper
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "You whisper to " + user + ": " + message + " \n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#18FC00"));
                    break;

                case 6: //Channel activity
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = user + message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#505050"));
                    break;

                case 7: //Error
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#B04434"));
                    break;
                case 8: //Server
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<" + user + "> " + message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#5050AC"));
                    break;
                case 9: //Moderator message
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<" + user + "> " + message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#C4C4C4"));
                    break;
                case 10: //Gem
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = message + " \n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#A420FC"));
                    break;

                default:  //Normal message
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<" + user + "> ";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#948064"));
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#C4C4C4"));
                    break;
            }
            if (scrolledToBot())
            {
                rtbOutput.ScrollToEnd();
            }

        }

        public void clearList()
        {
            lstUsers.Items.Clear();
        }

        public void clearChatText()
        {
            rtbOutput.Document.Blocks.Clear();
        }

        public bool userHasAv(string user, int av)
        {
            return spawnChat.userHasAvatar(user, av);
        }

        public void insertUser(int index, d2avatar user)
        {
            try
            {
                lstUsers.Items.Insert(index, user);
            }
            catch (ArgumentOutOfRangeException e)
            {
                lstUsers.Items.Add(user);
            }
            refreshChannelHeader();
        }

        public void addUser(d2avatar user)
        {
            lstUsers.Items.Add(user);
            refreshChannelHeader();

        }

        public void removeUser(int user)
        {
            try
            {
                lstUsers.Items.RemoveAt(user);
                if (selectedUser == user) selectedUser = -1;
                refreshChannelHeader();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void refreshChannelHeader()
        {
            lblChan.Content = chan.Substring(1) + (" (" + lstUsers.Items.Count) + ")";
        }

        public void selectMsgBox()
        {
            txtChat.Focus();
        }

        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectMsgBox();
        }

        #region buttonFunctions

        private void setupButtons()
        {
            cbtnSettings = new SpChBtn(btnSettings, lblBtnSettings, gameID + "/btnD2RightTop", gameID + "/btnD2RightTopPressed", 11);
            btns.Add(cbtnSettings);
            cbtnQuit = new SpChBtn(btnQuit, lblBtnQuit, gameID + "/btnD2RightBot", gameID + "/btnD2RightBotPressed", 11);
            btns.Add(cbtnQuit);
            cbtnSound = new SpChBtn(btnSound, lblBtnSound, gameID + "/btnD2RightBot", gameID + "/btnD2RightBotPressed", 11);
            btns.Add(cbtnSound);
            cbtnSend = new SpChBtn(btnSend, lblBtnSend, gameID + "/btnD2LeftTop", gameID + "/btnD2LeftTopPressed", 11);
            btns.Add(cbtnSend);
            cbtnWhis = new SpChBtn(btnWhis, lblBtnWhis, gameID + "/btnD2LeftTop", gameID + "/btnD2LeftTopPressed", 11);
            btns.Add(cbtnWhis);
            cbtnGem = new SpChBtn(btnGem, gameID + "/chatgem/GemDeactivated", gameID + "/chatgem/GemDeactivatedPressed");
            btns.Add(cbtnGem);
            cbtnChar = new SpChBtn(btnChar, lblBtnChar, gameID + "/btnD2RightTop", gameID + "/btnD2RightTopPressed", 11);
            btns.Add(cbtnChar);
            cbtnSquelch = new SpChBtn(btnSquelch, lblBtnSquelch, gameID + "/btnD2LeftBotCorner", gameID + "/btnD2LeftBotCornerPressed", 11);
            btns.Add(cbtnSquelch);
            cbtnUnsq = new SpChBtn(btnUnsq, lblBtnUnsq, gameID + "/btnD2LeftBotMid", gameID + "/btnD2LeftBotMidPressed", 11);
            btns.Add(cbtnUnsq);
            cbtnEmote = new SpChBtn(btnEmote, lblBtnEmote, gameID + "/btnD2LeftBotCorner", gameID + "/btnD2LeftBotCornerPressed", 11);
            btns.Add(cbtnEmote);
            arrowLeft = new SpChBtn(btnArrowLeft, gameID + "/chatarrows/arrowLeft", gameID + "/chatarrows/arrowLeftPressed");
            btns.Add(arrowLeft);
            arrowRight = new SpChBtn(btnArrowRight, gameID + "/chatarrows/arrowRight", gameID + "/chatarrows/arrowRightPressed");
            btns.Add(arrowRight);
            arrowLeftBig = new SpChBtn(btnArrowLeftBig, gameID + "/chatarrows/arrowLeftBig", gameID + "/chatarrows/arrowLeftBigPressed");
            btns.Add(arrowLeftBig);
            arrowRightBig = new SpChBtn(btnArrowRightBig, gameID + "/chatarrows/arrowRightBig", gameID + "/chatarrows/arrowRightBigPressed");
            btns.Add(arrowRightBig);
            ChatGem.gem = cbtnGem;
            ChatGem.cw = this;
            ChatGem.activated = false;
            cbtnSound.disable();
            cbtnSquelch.disable();
            cbtnUnsq.disable();
            cbtnWhis.disable();
            cbtnSend.disable();
            cbtnEmote.disable();
        }

        /// <summary>
        /// Not used in D2
        /// </summary>
        /// <param name="button"></param>
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
                CommandBindings.Add(new CommandBinding(enter, btnSend_Click));

                RoutedCommand altQ = new RoutedCommand();
                altQ.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(altQ, btnQuit_Click));

                RoutedCommand secondSettings = new RoutedCommand();
                secondSettings.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(secondSettings, btnSend_Click));

                RoutedCommand altW = new RoutedCommand();
                altW.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(altW, btnWhis_Click));

                RoutedCommand altE = new RoutedCommand();
                altE.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(altE, btnSettings_Click));

                RoutedCommand altT = new RoutedCommand();
                altT.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(altT, toggleTime));

                RoutedCommand ctrlN = new RoutedCommand();
                ctrlN.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
                CommandBindings.Add(new CommandBinding(ctrlN, pasteName));

                RoutedCommand altV = new RoutedCommand();
                altV.InputGestures.Add(new KeyGesture(Key.V, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(altV, toggleEL));

                RoutedCommand altEnter = new RoutedCommand();
                altEnter.InputGestures.Add(new KeyGesture(Key.Enter, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(altEnter, toggleFullscreen));
            }
            catch (Exception err)
            {
                //handle exception error
            }
        }

        private void btnSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnSettings.press();
        }

        private void btnSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnSettings.dePress();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            cbtnSettings.dePress();
            spawnChat.openSettings();
            //playSound(wavBtnClick.Stream);
            //mediaPlayer.Open(ResourceHelper.LoadResource("sfx/guiclick.mp3"));
            //mediaPlayer.Play();
        }


        private void btnChar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnChar.press();
        }

        private void btnChar_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnChar.dePress();
        }

        private void btnChar_Click(object sender, RoutedEventArgs e)
        {
            cbtnChar.dePress();
            spawnChat.openChar();
        }

        private void btnSound_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnSound.press();
        }

        private void btnSound_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnSound.dePress();
        }

        private void btnSound_Click(object sender, RoutedEventArgs e)
        {
            cbtnSound.dePress();
            //spawnChat.openSound();
        }

        private void btnQuit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnQuit.press();
        }

        private void btnQuit_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnQuit.dePress();
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            cbtnQuit.dePress();
            spawnChat.DoDisconnect();
            if (exitOnQuit)
            {
                Application.Current.Shutdown();
            }
            else
            {
                this.Hide();
                mw.Show();
            }

        }

        private void btnSend_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnSend.press();
        }

        private void btnSend_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnSend.dePress();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            cbtnSend.dePress();
            tempString = txtChat.Text.Trim();
            spawnChat.Send(tempString);
            txtChat.Clear();
            txtChat.Focus();
        }


        private void btnEmote_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnEmote.press();
        }

        private void btnEmote_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnEmote.dePress();
        }

        private void btnEmote_Click(object sender, EventArgs e)
        {
            cbtnEmote.dePress();
            tempString = "/me " + txtChat.Text.Trim();
            spawnChat.Send(tempString);
            txtChat.Clear();
            txtChat.Focus();
        }

        private void btnWhis_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnWhis.press();
        }

        private void btnWhis_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnWhis.dePress();
        }

        private void btnWhis_Click(object sender, RoutedEventArgs e)
        {
            cbtnWhis.dePress();
            tempString = txtChat.Text.Trim();
            if (lstUsers.SelectedIndex > -1)
            {
                spawnChat.Whisper(lstUsers.SelectedIndex, tempString);
                txtChat.Clear();
                txtChat.Focus();
            }
            else
            {
                AddToChatWindow(7, null, "You must first select a user from the list.");
            }
        }

        private void btnSquelch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnSquelch.press();
        }

        private void btnSquelch_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnQuit.dePress();
        }

        private void btnSquelch_Click(object sender, RoutedEventArgs e)
        {
            cbtnSquelch.dePress();
            tempString = txtChat.Text.Trim();
            if (lstUsers.SelectedIndex > -1)
            {
                tempString = spawnChat.getUserById(lstUsers.SelectedIndex);
                spawnChat.squelchUser(tempString);
                AddToChatWindow(3, tempString, " has been squelched.");
                disableUserButtons();
                txtChat.Focus();
            }
            else
            {
                AddToChatWindow(7, null, "You must first select a user from the list.");
            }
        }

        private void btnUnsq_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnUnsq.press();
        }

        private void btnUnsq_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnQuit.dePress();
        }

        private void btnUnsq_Click(object sender, RoutedEventArgs e)
        {
            cbtnUnsq.dePress();
            tempString = txtChat.Text.Trim();
            if (lstUsers.SelectedIndex > -1)
            {
                tempString = spawnChat.getUserById(lstUsers.SelectedIndex);
                spawnChat.unsquelchUser(tempString);
                AddToChatWindow(3, tempString, " has been unsquelched.");
                disableUserButtons();
                txtChat.Focus();
            }
            else
            {
                AddToChatWindow(7, null, "You must first select a user from the list.");
            }
        }

        private void btnGem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnGem.press();
        }

        private void btnGem_MouseLeave(object sender, MouseEventArgs e)
        {
            cbtnGem.dePress();
        }

        private void btnGem_Click(object sender, RoutedEventArgs e)
        {
            ChatGem.toggle();
        }

        private void arrowLeft_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            arrowLeft.press();
        }

        private void arrowLeft_MouseLeave(object sender, MouseEventArgs e)
        {
            arrowLeft.dePress();
        }

        private void arrowLeft_Click(object sender, RoutedEventArgs e)
        {
            arrowLeft.dePress();
        }

        private void arrowRight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            arrowRight.press();
        }

        private void arrowRight_MouseLeave(object sender, RoutedEventArgs e)
        {
            arrowRight.dePress();
        }

        private void arrowRight_Click(object sender, RoutedEventArgs e)
        {
            arrowRight.dePress();
        }

        private void arrowLeftBig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            arrowLeftBig.press();
        }

        private void arrowLeftBig_MouseLeave(object sender, RoutedEventArgs e)
        {
            arrowLeftBig.dePress();
        }

        private void arrowLeftBig_Click(object sender, RoutedEventArgs e)
        {
            arrowLeftBig.dePress();
        }

        private void arrowRightBig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            arrowRightBig.press();
        }

        private void arrowRightBig_MouseLeave(object sender, RoutedEventArgs e)
        {
            arrowRightBig.dePress();
        }

        private void arrowRightBig_Click(object sender, RoutedEventArgs e)
        {
            arrowRightBig.dePress();
        }

        private void lstUsers_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tempInt = (sender as ListBox).SelectedIndex;
            if (tempInt == selectedUser)
            {
                (sender as ListBox).SelectedIndex = -1;
                tempInt = -1;
            }
            selectedUser = tempInt;
            if (selectedUser > -1)
            {
                if (spawnChat.userIsSquelched(spawnChat.getUserById(selectedUser)))
                {
                    cbtnSquelch.disable();
                    cbtnUnsq.enable();
                }
                else
                {
                    cbtnUnsq.disable();
                    cbtnSquelch.enable();
                }
                if (txtChat.Text.Length > 0)
                {
                    cbtnWhis.enable();
                }
            }
            else
            {
                disableUserButtons();
            }
        }

        private void disableUserButtons()
        {
            cbtnSquelch.disable();
            cbtnUnsq.disable();
            cbtnWhis.disable();
        }

        #endregion

        private void txtChat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtChat.Text.Length > 0)
            {
                cbtnSend.enable();
                cbtnEmote.enable();
                if (selectedUser > -1)
                {
                    cbtnWhis.enable();
                }
            }
            else
            {
                cbtnSend.disable();
                cbtnEmote.disable();
                cbtnWhis.disable();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            spawnChat.closeSettings();
            spawnChat.DoDisconnect();
            if (exitOnQuit)
            {
                Application.Current.Shutdown();
            }
            else
            {
                try
                {
                    mw.Show();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("MainWindow failed to show, probably because you exited. It's cool, don't worry.");
                }
            }
        }

    }
}