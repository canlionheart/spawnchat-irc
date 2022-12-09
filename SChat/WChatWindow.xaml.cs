using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Ini;
using System.Reflection;
using SCommon;


namespace SChat
{
    /// <summary>
    /// Interaction logic for ChatWIndow.xaml
    /// </summary>
    public partial class WChatWindow : Window
    {
        TextRange tempRange;
        BitmapImage tempBmp;
        String tempString;
        public String chan {get; set;}
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
        String tempColour;
        IniFile ini;
        Random rnd;
        MainWindow mw;
        SpChBtn cbtnSettings;
        SpChBtn cbtnQuit;
        SpChBtn cbtnSend;
        SpChBtn cbtnWhis;
        Uri tempUri;
        char gameLetter = 'W';
        int tempInt;
        int selectedUser;
        int numChatIcons;
        public int avChar;
        public int avWins;
        public bool elnotif;
        bool tempBool;
        bool exitOnQuit = true;
        public bool timestamps = false;
        bool fullscreen;
        char[] tempCharArr;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        public WChatClient spawnChat;
        public WChatWindow(string username, string channel, MainWindow main, WChatClient client)
        {
            InitializeComponent();
            gameID = "W2BN";
            ini = new IniFile(System.IO.Path.Combine(Environment.CurrentDirectory, @"config.ini"));
            rnd = new Random();
            AddHotKeys();
            mw = main;
            nick = username;
            chan = channel;
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
                avWins = int.Parse(ini.IniReadValue(gameID, "wins"));
            }
            catch (FormatException e)
            {
                avWins = 0;
            }
            //tempUri = new Uri(@"pack://application:,,,/" + Assembly.GetCallingAssembly().GetName().Name + ";component/" + avatar, UriKind.Absolute);
            //tempBmp = new BitmapImage(tempUri);

            DefineChatIcons();
            setupButtons();
            mediaPlayer.Volume = 0.00;
        
            if (fullscreen)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }
        }

        private void setupButtons()
        {
            cbtnSettings = new SpChBtn(btnSettings, lblBtnSettings, lblKeySettings, gameID + "/btn" + gameLetter + "Ch", gameID + "/btn" + gameLetter + "chPressed", 17);
            btns.Add(cbtnSettings);
            cbtnQuit = new SpChBtn(btnQuit, lblBtnQuit, lblKeyQuit, gameID + "/btn" + gameLetter + "Ch", gameID + "/btn" + gameLetter + "chPressed", 17);
            btns.Add(cbtnQuit);
            cbtnSend = new SpChBtn(btnSend, lblBtnSend, lblKeySend, gameID + "/btn" + gameLetter + "Chat", gameID + "/btn" + gameLetter + "chatPressed", 14);
            btns.Add(cbtnSend);
            cbtnWhis = new SpChBtn(btnWhis, lblBtnWhis, lblKeyWhis, gameID + "/btn" + gameLetter + "Chat", gameID + "/btn" + gameLetter + "chatPressed", 14);
            btns.Add(cbtnWhis);
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

        

        private void toggleEL(object sender, RoutedEventArgs e)
        {
            spawnChat.toggleEL();
        }

        private void toggleTime(object sender, RoutedEventArgs e)
        {
            if (timestamps)
            {
                timestamps = false;
                AddToChatWindow(6, null, "Chat Timestamps: OFF");
            }
            else
            {
                timestamps = true;
                AddToChatWindow(6, null, "Chat Timestamps: ON");
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

        public Image getBanner()
        {
            return bannerAd;
        }

        public void updateSettings(bool timestamps, bool fullscreen, bool exonq, int avclass, int wins)
        {
            exitOnQuit = exonq;
            this.timestamps = timestamps;
            if (!(this.fullscreen.Equals(fullscreen)))
            {
                toggleFScreen();
            }
            avChar = avclass;
            avWins = wins;
            buildUserAvatar();
            if (!userHasAv(nick, "op"))
            {
                updateUserAvatar(spawnChat.getUserByName(nick));
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

                RoutedCommand altN = new RoutedCommand();
                altN.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Alt));
                CommandBindings.Add(new CommandBinding(altN, pasteName));

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

        public void AddToChatWindow(string message)
        {
            rtbOutput.AppendText(message + "\n");
            rtbOutput.ScrollToEnd();
        }

        public void AddToChatWindow(int type, string user, string message)
        {
            if (timestamps)
            {
                if (!(type == 3 || type == 7 || type == 8))
                {
                    tempString = "[" + System.DateTime.Now.ToShortTimeString() + "] ";
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = tempString;
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                }
            }
            
            switch (type)
            {
                case 1: //Emote
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<" + user + " " + message + "> \n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
                    break;

                case 2: //Whisper
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<From: " + user + "> ";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Gray);
                    break;

                case 3: //System
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = user + message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
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
                    tempRange.Text = "<To: " + user + "> ";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Aqua);
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Gray);
                    break;

                case 6: //Channel activity
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = user + message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)new BrushConverter().ConvertFromString("#00FF00"));
                    break;

                case 7: //Error
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                    break;
                case 8: //Server
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<" + user + "> " + message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Aqua);
                    break;
                case 9: //Moderator message
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<" + user + "> " + message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                    break;
                case 10: //Moderator emote
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<" + user + " " + message + "> \n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                    break;

                default:  //Normal message
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = "<" + user + "> ";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Yellow);
                    tempRange = new TextRange(rtbOutput.Document.ContentEnd, rtbOutput.Document.ContentEnd);
                    tempRange.Text = message + "\n";
                    tempRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
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

        public String getRandomIcon()
        {
            
            return chatIcon.ElementAt(rnd.Next(numChatIcons));
        }

        public string getUserColour(string avatar)
        {
            if (avatar.Equals("op") || avatar.Equals("spkr"))
            {
                return "Yellow"; 
            }
            else
            {
                for (int i = numChatIcons - 1; i > 3; i--)
                {
                    if (Equals(avatar, chatIcon.ElementAt(i)))
                    {
                        return "Yellow";
                    }
                }
                return "White";
            }
           

        }

        public User getUserProfile(string user)
        {
            //Check User Types
            if (user.StartsWith("+")) //Speaker
            {
                tempString = "spkr";
                return new User(tempString, user.Substring(1));

            }
            else if (user.StartsWith("@") || user.StartsWith("&") || user.StartsWith("%")) //Op/Admin/Halfop
            {
                tempString = "op";
                return new User(tempString, user);
            }
            else if (user.Equals(nick))   //This client
            {
                tempString = avatar;

                return new User(tempString, user);
            }
            else                        //Everyone else
            {
                tempString = getRandomIcon();
                if (tempString.StartsWith("w2bn/wR"))
                {
                    tempName =  tempString.Substring(0,7) + rnd.Next(11);
                    return new User(tempName, user);
                }
                return new User(tempString, user);
            }
        }

        public bool userHasAv(string user, string av)
        {
            return Equals((lstUsers.Items.GetItemAt(spawnChat.getUserByName(user)) as Item).PictureID, av);
        }

        public void insertUser(int index, String avatar, String name)
        {
            try
            {
                lstUsers.Items.Insert(index, new Item(avatar, name));
            }
            catch (ArgumentOutOfRangeException e)
            {
                lstUsers.Items.Add(new Item(avatar, name));
            }
            refreshChannelHeader();
        }

        public void populateUserList(List<User> users)
        {
            lstUsers.Items.Clear();
            foreach (User element in users)
            {
                lstUsers.Items.Add(new Item(element.PictureID, element.Name));
                
            }
            refreshChannelHeader();
        }

        public void addUser(string avatar, string name)
        {
            //Console.WriteLine("\nUSER: " + avatar + " " + name);
            //Console.WriteLine("ATTEMPTED DIABLO AVATAR: " + new Item(avatar, name).PictureString);
            if (name.Equals(nick))
            {
                lstUsers.Items.Add(new Item(avatar, name, "White"));
            }
            else
            {
                lstUsers.Items.Add(new Item(avatar, name, getUserColour(avatar)));
            }

            
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

        private void btnSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnSettings.press();
            depressOtherButtons(cbtnSettings);
        }

        private void btnSettings_MouseLeave(object sender, MouseEventArgs e)
        {
            //cbtnSettings.dePress();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            cbtnSettings.dePress();
            spawnChat.openSettings();
            mediaPlayer.Open(ResourceHelper.LoadResource("sfx/guiclick.mp3"));
            mediaPlayer.Play();
        }

        private void btnQuit_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnQuit.press();
            depressOtherButtons(cbtnQuit);
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            cbtnQuit.dePress();
            spawnChat.DoDisconnect();
            mediaPlayer.Open(ResourceHelper.LoadResource("sfx/guiclick.mp3"));
            mediaPlayer.Play();
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
            depressOtherButtons(cbtnSend);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            cbtnSend.dePress();
            tempString = txtChat.Text.Trim();
            spawnChat.Send(tempString);
            txtChat.Clear();
            txtChat.Focus();
        }

        private void btnWhis_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cbtnWhis.press();
            depressOtherButtons(cbtnWhis);
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

        public void buildUserAvatar()
        {
            //BUILD USER AVATAR;
            if (avWins > 10)
            {
                avatar = "chat";
            }
            switch (avChar)
            {
                case 0:
                    avatar = "spawn";
                    break;
                case 1:
                    avBuilder = "R";
                    break;
                case 2:
                    avatar = "w2bn/blue";
                    break;
                case 3:
                    avatar = "w2bn/iron";
                    break;
                case 4:
                    avatar = "nerd";
                    break;
                default:
                    avatar = "chat";
                    break;
            }
            if (avChar == 1 && avWins < 11)
            {
                avatar = gameID + "/w" + avBuilder + avWins;
            }
        }

        public void updateUserAvatar(int pos)
        {
            if (spawnChat.isConnected())
            {
                try
                {
                    removeUser(pos);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    removeUser(spawnChat.getUserByName(nick));
                }
                spawnChat.setUserAvatar(nick, avatar);
                try
                {
                    lstUsers.Items.Insert(pos, new Item(avatar, nick, "White"));
                }
                catch (ArgumentOutOfRangeException e)
                {
                    lstUsers.Items.Add(new Item(avatar, nick, "White"));
                }
                refreshChannelHeader();
                /*TEMPORARY FIX UNTIL BETTER ONE IS FOUND
                tempList = spawnChat.getUsersByName(nick);
                Console.WriteLine("\n----TEMPLIST CONTENTS----");
                for (int i = 1; i < tempList.Count; i++)
                {
                    removeUser(i);
                    Console.WriteLine("\nItem: " + i);
                }
                Console.WriteLine("\n----END OF LIST----");*/

            }
        }

        private void DefineChatIcons()
        {
            chatIcon = new List<String>();
            buildUserAvatar();
            standardIcons();
        }

        public void standardIcons()
        {
            chatIcon.Add("w2bn/iron");
            chatIcon.Add("w2bn/wR0");
            chatIcon.Add("w2bn/blue");
            chatIcon.Add("spawn");
            chatIcon.Add("drtl");
            chatIcon.Add("dshr");
            chatIcon.Add("d2dv");
            chatIcon.Add("sexp");
            chatIcon.Add("sshr");
            chatIcon.Add("star");
            chatIcon.Add("w3xp");
            chatIcon.Add("war3");
            chatIcon.Add("jstr");
            chatIcon.Add("d2xp");
            chatIcon.Add("chat");
            numChatIcons = chatIcon.Count();
        }

        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectMsgBox();
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

        private void lstUsers_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tempInt = (sender as ListBox).SelectedIndex;
            if (tempInt == selectedUser)
            {
                (sender as ListBox).SelectedIndex = -1;
                tempInt = -1;
            }
            selectedUser = tempInt;
        }

    }
}
