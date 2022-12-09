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
using SCommon;
using d2c;

namespace SChat
{
    public class D2ChatClient
    {
        String[] tempStrArray;
        String nick;
        String chan;
        String server;
        String tempName;
        String tempString;
        String lastWhisperer = "";
        MainWindow mw;
        D2ChatWindow cw;
        D2SettingsWindow sw;
        D2CharBuilder cb;
        //D2SoundSettings soundsettings;
        D2BlankPanel bp;
        List<d2avatar> users;
        List<int> tempList;
        Random rnd;
        IniFile ini;
        d2avatar tempUser;
        Window error;
        SpawnMaster sm;
        string nickservbot;
        string userpass;
        string gameID = "D2DV";
        int tempInt;
        int clientId;
        public int avChar;
        public int avArmor;
        bool identifyToNickServ;
        bool dnd;
        bool elnotif;
        bool tempBool;
        bool firstRun;
        bool secondRun;
        bool suppresskickmsg;
        IrcClient client;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nick"></param>
        /// <param name="server"></param>
        /// <param name="chan"></param>
        /// <param name="game"></param>
        /// <param name="mainWin"></param>
        /// <param name="identify"></param>
        /// <param name="pass">used to indetify with NickServ</param>
        /// <param name="bot">Name of NickServ bot</param>
        public D2ChatClient(String nick, String server, String chan, int game, MainWindow mainWin, bool identify, string pass, string bot)
        {
            mw = mainWin;
            this.nick = nick;
            this.server = server;
            this.chan = chan;
            clientId = game;
            identifyToNickServ = identify;
            userpass = pass;
            nickservbot = bot;
            bp = new D2BlankPanel();
            ini = new IniFile(System.IO.Path.Combine(Environment.CurrentDirectory, @"config.ini"));
            rnd = new Random();
            users = new List<d2avatar>();

            try
            {
                firstRun = (Boolean.Parse(ini.IniReadValue("Settings", "firstrun")));
            }
            catch (FormatException e)
            {
                firstRun = false;
            }
            try
            {
               secondRun = (Boolean.Parse(ini.IniReadValue("Settings", "2ndrun")));
            }
            catch (FormatException e)
            {
                secondRun = false;
            }
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
            cw = new D2ChatWindow(nick, chan, mw, this);
            closeSettings();
            sm = new SpawnMaster(nick, server, chan, cw.getBanner());
            sw = new D2SettingsWindow(cw, this);
            cb = new D2CharBuilder(this);
            //soundsettings = new D2SoundSettings(this);
            
            DoConnect();
            if (firstRun)
            {
                cw.AddToChatWindow(8, null, sm.clientMsg[0]);
                ini.IniWriteValue("Settings", "2ndrun", true.ToString());
                ini.IniWriteValue("Settings", "firstrun", false.ToString());
            }
            if (secondRun)
            {
                cw.AddToChatWindow(8, null, sm.clientMsg[1]);
                ini.IniWriteValue("Settings", "2ndrun", false.ToString());
            }

        }

        /// <summary>
        /// Send messages and parse commands
        /// </summary>
        /// <param name="content"></param>
        public void Send(String content)
        {
            if (client != null && client.Connected && !String.IsNullOrEmpty(content))
            {
                /*
                 * COMMANDS
                 **/
                if (content.StartsWith("/"))
                {
                    /*
                    * EMOTE
                    **/
                    if ((content.StartsWith("/me ")) || (content.StartsWith("/emote ")))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 2);
                        cw.AddToChatWindow(1, client.Nick, tempStrArray[1]);
                        if (chan != "The Void")
                        {
                            client.Emote(chan, tempStrArray[1]);
                        }
                        if (channelIsEmpty())
                        {
                            cw.AddToChatWindow(3, null, sm.clientMsg[5]);
                        }
                    }
                    /*
                    * WHISPER
                    **/
                    else if ((content.StartsWith("/w ")) || (content.StartsWith("/whisper ")) || (content.StartsWith("/msg ")) || (content.StartsWith("/m ")))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 3);
                        client.SendMessage(tempStrArray[1], tempStrArray[2]);
                        cw.AddToChatWindow(5, tempStrArray[1], tempStrArray[2]);
                    }
                    /*
                    * REPLY
                    **/
                    else if ((content.StartsWith("/r ")) || (content.StartsWith("/reply ")))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 2);
                        client.SendMessage(lastWhisperer, tempStrArray[2]);
                        cw.AddToChatWindow(5, lastWhisperer, tempStrArray[2]);
                    }
                    /*
                    * JOIN
                    **/
                    else if ((content.StartsWith("/j ")) || (content.StartsWith("/join ")))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 2);
                        content = tempStrArray[1];
                        if (content.Equals("The Void", StringComparison.InvariantCultureIgnoreCase))
                        {
                            joinTheVoid(false);
                        }
                        else
                        {
                            joinChannel(content);
                        }

                    }
                    /*
                    * SQUELCH
                    **/
                    else if (content.StartsWith("/squelch ") || content.StartsWith("/ignore "))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 2);
                        squelchUser(tempStrArray[1]);
                        cw.AddToChatWindow(3, tempStrArray[1], " has been ignored.");

                    }
                    /*
                    * UNSQUELCH
                    **/
                    else if (content.StartsWith("/unsquelch ") || content.StartsWith("/unignore "))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 2);
                        unsquelchUser(tempStrArray[1]);
                        cw.AddToChatWindow(3, tempStrArray[1], " has been unignored.");
                    }
                    /*
                    * WHOIS
                    **/
                    else if (content.StartsWith("/whois ") || content.StartsWith("/where ") || content.StartsWith("/whereis "))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 2);
                        client.SendRaw("WHOIS " + tempStrArray[1]);
                    }
                    /*
                    * WHOAMI
                    **/
                    else if (content.Equals("/whoami"))
                    {
                        client.SendRaw("WHOIS " + nick);
                    }
                    /*
                    * TIME
                    **/
                    else if (content.Equals("/time"))
                    {
                        client.SendRaw("TIME");
                        cw.AddToChatWindow(3, null, "Your local time: " + System.DateTime.Now.ToLocalTime());
                    }
                    /*
                    * WHO (Not Implemented)
                    
                    else if (content.StartsWith("/who "))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 2);
                        client.SendRaw("NAMES " + tempStrArray[1]);
                    }**/
                    /*
                    * USERS
                    **/
                    else if (content.Equals("/users"))
                    {
                        client.SendRaw("LUSERS");
                    }
                    /*
                    * AWAY
                    **/
                    else if (content.StartsWith("/away"))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 2);
                        if (tempStrArray.Length > 1)
                        {
                            client.SendRaw("AWAY " + tempStrArray[1]);
                        }
                        else
                        {
                            client.SendRaw("AWAY");
                        }
                    }
                    /*
                    * DND
                    **/
                    else if (content.StartsWith("/dnd"))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 2);
                        if (tempStrArray.Length > 1)
                        {
                            dnd = true;
                            client.SendRaw("AWAY " + tempStrArray[1]);
                        }
                        else
                        {
                            dnd = false;
                            client.SendRaw("AWAY");
                        }
                    }
                    /*
                    * NICK
                    **/
                    else if (content.StartsWith("/nick "))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 3);
                        if (tempStrArray[1].Length > 15)
                        {
                            tempStrArray[1] = tempStrArray[1].Substring(0, 15);
                        }
                        client.SendRaw("NICK " + tempStrArray[1]);
                        client.Nick = tempStrArray[1];
                        nick = tempStrArray[1];
                        sm.nick = tempStrArray[1];
                        cw.nick = tempStrArray[1];
                    }
                    /**
                     * REJOIN
                     **/
                    else if (content.StartsWith("/rejoin"))
                    {
                        join(chan.Substring(1));
                    }
                    /*
                    * KICK
                    **/
                    else if (content.StartsWith("/kick ") || content.StartsWith("/ban "))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 3);
                        if (validateOpAction(tempStrArray[1]))
                        {
                            if (content.StartsWith("/ban "))
                            {
                                client.SendRaw("MODE " + chan + " +b " + tempStrArray[1]);
                                suppresskickmsg = true;
                            }
                            else
                            {
                                suppresskickmsg = false;
                            }
                            if (tempStrArray.Length > 2)     //kick message provided
                            {
                                client.Kick(chan, tempStrArray[1], tempStrArray[2]);
                            }
                            else
                            {
                                client.Kick(chan, tempStrArray[1], null);
                            }
                        }
                    }
                    /*
                     * UNBAN
                     **/
                    else if (content.StartsWith("/unban "))
                    {
                        tempStrArray = content.Split((new Char[] { ' ' }), 3);
                        client.SendRaw("MODE " + chan + " -b " + tempStrArray[1]);
                    }
                    /*
                     * IGNOREPERSIST
                     **/
                    else if (content.StartsWith("/ignorepersist"))
                    {
                        sm.toggleIgnore();
                        cw.AddToChatWindow(3, null, sm.clientMsg[13]);
                    }
                    /*
                     * D2NOTIFY
                     **/
                    else if (content.StartsWith("/d2notify"))
                    {
                        toggleEL();
                    }
                    /*
                     * DISCONNECT
                     **/
                    else if (content.StartsWith("/disconnect") || content.StartsWith("/dc"))
                    {
                        DoDisconnect();
                        cw.AddToChatWindow(3, null, "Connection closed.");
                    }
                    /*
                     * RECONNECT
                     **/
                    else if (content.StartsWith("/reconnect") || content.StartsWith("/rc"))
                    {
                        DoDisconnect();
                        DoConnect();
                    }
                    /*
                     * ANYTHING ELSE
                     **/
                    else
                    {
                        cw.AddToChatWindow(7, null, sm.clientMsg[12]);
                    }

                }
                else //Regular message
                {
                    cw.AddToChatWindow(0, client.Nick, content);
                    if (chan != "The Void")
                    {
                        client.SendMessage(chan, content);

                        if (channelIsEmpty())
                        {
                            cw.AddToChatWindow(3, null, sm.clientMsg[5]);
                        }
                    }
                }
            }
            else if ((client == null || !client.Connected) && !String.IsNullOrEmpty(content))
            {
                /*
                 * RECONNECT
                 **/
                if (content.StartsWith("/reconnect") || content.StartsWith("/rc"))
                {
                    DoDisconnect();
                    DoConnect();
                }
                else
                {
                    cw.AddToChatWindow(7, null, "You are not connected.");
                }
            }
        }

        public void Whisper(int target, string message)
        {
            if (!(String.IsNullOrEmpty(message)))
            {
                tempName = getUserById(target);
                client.SendMessage(tempName, message);
                cw.AddToChatWindow(5, tempName, message);
            }
        }

        /// <summary>
        /// Toggle notifications for users entering/leaving the channel.
        /// </summary>
        public void toggleEL()
        {
            if (elnotif)
            {
                elnotif = false;
                cw.AddToChatWindow(3, null, sm.clientMsg[6]);
            }
            else
            {
                elnotif = true;
                cw.AddToChatWindow(3, null, sm.clientMsg[7]);
            }

            ini.IniWriteValue("Settings", "enterleave", elnotif.ToString());
        }

        public void updateSettings(bool joinleave)
        {
            elnotif = joinleave;
            sm.update();
            closeSettings();
        }

        public void updateUserAvatar(int d2class, int d2armor)
        {
            tempInt = getUserByName(nick);
            avChar = d2class;
            avArmor = d2armor;
            if (isConnected())
            {
                try
                {
                    cw.removeUser(tempInt);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                }
                setUserAvatar(nick, avChar, avArmor);
                try
                {
                    cw.insertUser(getUserByName(nick), new d2avatar(d2class, d2armor, nick));
                }
                catch (ArgumentOutOfRangeException e)
                {
                    cw.addUser(new d2avatar(d2class, d2armor, nick));
                }
                cw.refreshChannelHeader();
            }
        }

        private void joinChannel(string channel)
        {
            join(channel);
            cw.AddToChatWindow(6, null, "You have joined channel: " + channel);
        }

        private void join(string channel)
        {
            if (chan != "The Void")
            {
                client.PartChannel(chan);
            }
            users.Clear();
            client.alreadyGotList = false;
            cw.clearList();
            client.JoinChannel("#" + channel);
            chan = "#" + channel;
            cw.chan = "#" + channel;
            sm.chan = "#" + channel;

        }

        public void setUserAvatar(string nick, int d2class, int d2armor)
        {
            tempInt = getUserByName(nick);
            try
            {
                if (!(users.ElementAt(tempInt).isOperator()))
                {
                    users.ElementAt(tempInt).setClass(d2class);
                    users.ElementAt(tempInt).setPreset(d2armor);
                }
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
        }

        public bool userIsSquelched(string user)
        {
            if (userIsPresent(user))
            {
                return users[getUserByName(user)].isSquelched();
            }
            return sm.userIsSquelched(user);
        }



        public void squelchUser(string user)
        {
            tempInt = getUserByName(user);

            if (!(user.Equals(nick)))
            {
                if (!(userIsSquelched(user)))//Ensure user is not already squelched
                {
                    sm.squelchedUsers.Add(user);
                    if (userIsPresent(user))
                    {
                        //cw.AddToChatWindow(3, users[tempInt].Name, " has been ignored."); //Ensure accurate capitalization
                        users[tempInt].squelch();
                        cw.removeUser(tempInt);
                        try
                        {
                            cw.insertUser(tempInt, new d2avatar(users[tempInt].classId, users[tempInt].armorId, true, users[tempInt].Name));    //Replace user avatar
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            cw.addUser(new d2avatar(users[tempInt].classId, users[tempInt].armorId, true, users[tempInt].Name));
                        }
                    }
                    else
                    {
                        //cw.AddToChatWindow(3, user, " has been ignored.");
                    }
                }
            }
            
        }

        //Unsquelch
        public void unsquelchUser(string user)
        {
            tempInt = getUserByName(user);
            if (!(user.Equals(nick)))
            {
                if (userIsSquelched(user)) //Ensure user is not already unsquelched
                {
                    sm.squelchedUsers.RemoveAll(n => n.Equals(user, StringComparison.InvariantCultureIgnoreCase));
                    if (userIsPresent(user))
                    {
                        try
                        {
                            //cw.AddToChatWindow(3, users[tempInt].Name, " has been unignored.");
                            users[tempInt].unsquelch();
                            cw.removeUser(tempInt);
                            cw.insertUser(tempInt, new d2avatar(users[tempInt].classId, users[tempInt].armorId, false, users[tempInt].Name));    //Replace user avatar
                        }

                        catch (ArgumentOutOfRangeException e)
                        {
                            cw.addUser(new d2avatar(users[tempInt].classId, users[tempInt].armorId, false, users[tempInt].Name));
                        }
                    }
                    else
                    {
                        //cw.AddToChatWindow(3, user, " has been unignored.");
                    }
                }

            }
        }

        public string getUserById(int id)
        {
            return users.ElementAt(id).Name;
        }

        public bool userHasAvatar(string user, int d2class)
        {
            return users.ElementAt(getUserByName(nick)).classId == d2class;
        }

        public bool userIsPresent(string user)
        {
            return !(getUserByName(user) == -1);
        }


        public d2avatar getRandomAvatar()
        {
            return d2AvatarGenerator.generate();
        }

        public d2avatar getUserProfile(string user)
        {
            //Check User Types
            if (user.StartsWith("+")) //Speaker
            {
                return new d2avatar(14, user.Substring(1));

            }
            else if (user.StartsWith("@") || user.StartsWith("&") || user.StartsWith("%")) //Op/Admin/Halfop
            {
                return new d2avatar(13, user);
            }
            else if (user.Equals(nick))   //This client
            {
                return new d2avatar(avChar, avArmor, user);
            }
            else                        //Everyone else
            {
                return d2AvatarGenerator.generate(user);
            }
        }

        public bool channelIsEmpty()
        {
            return users.Count < 2;
        }

        public void DoConnect()
        {

            client = new IrcClient(server, 6667);
            cw.Title = sw.windowTitle;
            AddEvents();
            cw.clearChatText(); // in case they reconnect and have old stuff there
            client.Nick = nick;
            client.Connect();
            cw.Show();
        }

        public void DoDisconnect()
        {
            try
            {
                sm.close();
                cw.clearList();
                client.Disconnect();
                client = null;
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("\n\nGoodbye!");
            }
        }

        public bool validateOpAction(string target)
        {
            if (users.ElementAt(getUserByName(nick)).isOperator()) //Client is op
            {
                if (userIsPresent(target))  //Target is in the channel
                {
                    if (!(users.ElementAt(getUserByName(target)).isOperator())) //Target is not op
                    {
                        return true;
                    }
                    else
                    {
                        cw.AddToChatWindow(7, null, sm.clientMsg[2]);
                    }
                }
                else
                {
                    cw.AddToChatWindow(7, null, sm.clientMsg[3]);
                }
            }
            else
            {
                cw.AddToChatWindow(7, null, sm.clientMsg[4]);
            }
            return false;
        }

        public void openSettings()
        {
            try
            {
                sw.loadSettings();
                cw.getPanel().Content = sw;
            }
            catch (InvalidOperationException e)
            {

                sw = new D2SettingsWindow(cw, this);
                cw.getPanel().Content = sw;
            }
        }

        public void openChar()
        {
            try
            {
                cb.loadSettings();
                cw.getPanel().Content = cb;
            }
            catch (InvalidOperationException e)
            {

                cb = new D2CharBuilder(this);
                cw.getPanel().Content = cb;
            }
        }

        /*public void openSound()   Not implemented.
        {
            try
            {
                soundsettings.loadSettings();
                cw.getPanel().Content = soundsettings;
            }
            catch (InvalidOperationException e)
            {

                soundsettings = new D2SoundSettings(this);
                cw.getPanel().Content = soundsettings;
            }
        }*/

        public void closeSettings()
        {
            try
            {
                cw.getPanel().Content = bp;
            }
            catch (InvalidOperationException e)
            {

            }
        
        }

        /*
         * Return the index of the first user in the list that goes by 'username'
         */
        public int getUserByName(string username)
        {
            foreach (d2avatar element in users)
            {
                if (element.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    return users.IndexOf(element);
                }
            }
            return -1;
        }

         /*
         * Return a list of all indexes where the associated username is equal to 'username'
         */
        public List<int> getUsersByName(string username)
        {
            tempList = new List<int>();
            foreach (d2avatar element in users)
            {
                if (element.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    tempList.Add(users.IndexOf(element));
                }
            }
            return tempList;
        }


        public bool isConnected()
        {
            return client.Connected;
        }

        private void AddEvents()
        {
            client.ChannelMessage += client_ChannelMessage;
            client.ChannelEmote += client_ChannelEmote;
            client.ChannelModeSet += client_ChannelModeSet;
            client.ExceptionThrown += client_ExceptionThrown;
            client.NoticeMessage += client_NoticeMessage;
            client.OnConnect += client_OnConnect;
            client.PrivateMessage += client_PrivateMessage;
            client.ServerMessage += client_ServerMessage;
            client.KickUser += client_KickUser;
            client.UpdateUsers += client_UpdateUsers;
            client.UserJoined += client_UserJoined;
            client.UserLeft += client_UserLeft;
            client.UserNickChange += client_UserNickChange;
        }

 
        public void addUserToList(string user)
        {
            //Check User Types
            tempUser = getUserProfile(user);

            if (user.StartsWith("@") || user.StartsWith("&") || user.StartsWith("%")) //Op/Admin/Halfop
            {
                tempUser.Name = tempUser.Name.Substring(1);
                if (sm.userIsSquelched(tempUser.Name))
                {
                    tempUser.squelch();
                }
                cw.insertUser(0, tempUser);
                users.Insert(0, tempUser);
            }
            else
            {
                if (sm.userIsSquelched(tempUser.Name))
                {
                    tempUser.squelch();
                }
                cw.addUser(tempUser);
                users.Add(tempUser);
            }

        }

        public void removeUser(string user)
        {
            if (elnotif)
            {
                if (!(user.Equals(nick)))
                {
                    cw.AddToChatWindow(6, user, " has left the channel.");
                }
            }
            tempInt = getUserByName(user);
            if (tempInt >= 0)
            {
                cw.removeUser(tempInt);
                users.RemoveAt(tempInt);
            }
        }

        public void joinTheVoid(bool fromKick)
        {
            if (!fromKick)
            {
                client.PartChannel(chan);
            }
            chan = "The Void";
            cw.AddToChatWindow(6, null, "You have joined channel: The Void");
            cw.theVoid();
            cw.AddToChatWindow(3, null, sm.clientMsg[10]);
            cw.clearList();
        }

        #region Event Listeners

        void client_OnConnect(object sender, EventArgs e)
        {
            cw.selectMsgBox();
            client.JoinChannel(chan);
            cw.AddToChatWindow(6, null, "You have joined channel: " + chan.Substring(1));
            if (identifyToNickServ)
            {
                client.SendMessage(nickservbot, "IDENTIFY " + userpass);
            }

        }

        void client_UserNickChange(object sender, UserNickChangedEventArgs e)
        {
            tempInt = getUserByName(e.Old); //Gets the index where the old name is found in the userlist.
            if (tempInt >= 0)
            {
                tempBool = userIsSquelched(e.Old);   //Remember if user was squelched
                tempUser = users.ElementAt(tempInt);    //Save avatar
                cw.removeUser(tempInt);   //Remove user
                users.RemoveAt(tempInt);
                users.Insert(tempInt, new d2avatar(tempUser.classId, tempUser.armorId, tempBool, e.New));
                cw.insertUser(tempInt, new d2avatar(tempUser.classId, tempUser.armorId, tempBool, e.New));    //Re-add user at old index with new name
                if (!tempBool)
                {
                    cw.AddToChatWindow(3, e.Old, " is now known as " + e.New); 
                }
            }

        }

        void client_UserLeft(object sender, UserLeftEventArgs e)
        {
            removeUser(e.User);
        }

        void client_UserJoined(object sender, UserJoinedEventArgs e)
        {
            if (!(e.User.Equals(nick)))
            {
                if (elnotif)
                {
                    cw.AddToChatWindow(6, e.User, " has joined the channel.");
                }
                addUserToList(e.User);
            }
        }

        void client_UpdateUsers(object sender, UpdateUsersEventArgs e)
        {

            cw.clearList();
            foreach (string element in e.UserList)
            {
                addUserToList(element);

            }
            cw.scrollToEnd();
        }

        void client_ServerMessage(object sender, StringEventArgs e)
        {
            if (!(e.Result.Contains("End of")))
            {
                cw.AddToChatWindow(3, null, e.Result);
            }
            Console.WriteLine(e.Result);
        }

        void client_PrivateMessage(object sender, PrivateMessageEventArgs e)
        {
            tempInt = getUserByName(e.From);
            try
            {
                if (!(userIsSquelched(e.From)))
                {
                    lastWhisperer = e.From;
                    if (!dnd)
                    {
                        cw.AddToChatWindow(2, e.From, e.Message);
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        void client_NoticeMessage(object sender, NoticeMessageEventArgs e)
        {

            if (!(userIsSquelched(e.From)))
            {
                cw.AddToChatWindow(8, e.From, e.Message);
            }
        }

        void client_ChannelModeSet(object sender, ModeSetEventArgs e)
        {
            tempBool = userIsSquelched(e.To);   //Remember if user was squelched
            tempInt = getUserByName(e.To); //Gets the index where the target user is found in the user list.
            Console.WriteLine("MODE SET OUTPUT: "
                + "\nCHANNEL: " + e.Channel
                + "\nFROM: " + e.From
                + "\nMODE: " + e.Mode
                + "\nTO: " + e.To);
            /*
             * ADDING MODES
             */
            if (e.Mode.StartsWith("+"))
            {
                /*
                 * OP and Halfop
                 */
                if (e.Mode.Contains("o") || e.Mode.Contains("h"))
                {
                    if (tempInt >= 0)
                    {   
                        cw.removeUser(tempInt);   //Remove user
                        users.RemoveAt(tempInt);
                        tempInt = 0;
                        users.Insert(tempInt, new d2avatar(13, tempBool, e.To));
                        cw.AddToChatWindow(3, e.To, " is now a channel operator.");
                        cw.insertUser(tempInt, new d2avatar(13, tempBool, e.To));    //Re-add user at old index
                        //Cleanup if need be
                        tempList = getUsersByName(tempString);
                        if (tempList.Count > 1)
                        {
                            cw.removeUser(tempList.ElementAt(tempList.Count - 1));
                        }
                    }
                }
                /*
                 * VOICE mode
                 */
                else if (e.Mode.Contains("v"))
                {
                    if (tempInt >= 0)
                    {     
                        cw.removeUser(tempInt);   //Remove user
                        users.RemoveAt(tempInt);
                        cw.AddToChatWindow(3, e.To, " is now a speaker.");
                        try
                        {
                            users.Insert(tempInt, new d2avatar(14, tempBool, e.To));
                            cw.insertUser(tempInt, new d2avatar(14, tempBool, e.To));    //Re-add user at old index with new name
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            users.Add(new d2avatar(6, tempBool, e.To));
                            cw.addUser(new d2avatar(6, tempBool, e.To));
                        }

                    }
                    
                }
                else if (e.Mode.Contains("b"))
                {
                    cw.AddToChatWindow(3, e.To, " was banned by " + e.From + ".");
                }
            }

            /*
             * REMOVING MODES
             */
            if (e.Mode.StartsWith("-"))
            {
                /*
                 * OP, Halfop, and Voice
                 */
                if (e.Mode.Contains("o") || e.Mode.Contains("h") || e.Mode.Contains("v"))
                {
                    if (tempInt >= 0)
                    {
                        cw.removeUser(tempInt);   //Remove user
                        users.RemoveAt(tempInt);
                        tempUser = getUserProfile(e.To);
                        users.Add(new d2avatar(tempUser.classId, tempUser.armorId, tempBool, e.To));
                        cw.addUser(new d2avatar(tempUser.classId, tempUser.armorId, tempBool, e.To));    //Re-add user at old index with new name
                        if (e.Mode.Contains("o") || e.Mode.Contains("h"))
                        {
                            cw.AddToChatWindow(3, e.To, " is no longer a channel operator.");
                        }
                        else
                        {
                            cw.AddToChatWindow(3, e.To, " is no longer a speaker.");
                        }
                    }
                }
                else if (e.Mode.Contains("b"))
                {
                    cw.AddToChatWindow(3, e.To, " was unbanned by " + e.From + ".");
                }
            }
        }


        void client_KickUser(object sender, KickUserEventArgs e)
        {
            if (!suppresskickmsg)
            {
                if (e.Message.Length > 1)
                {
                    cw.AddToChatWindow(3, e.To, " was kicked out of the channel by " + e.From + " (" + e.Message + ").");
                }
                else
                {
                    cw.AddToChatWindow(3, e.To, " was kicked out of the channel by " + e.From + ".");
                }
            }

            if (e.To.Equals(nick))
            {
                users.Clear();
                cw.clearList();
                cw.AddToChatWindow(3, e.From, " kicked you out of the channel!");
                joinTheVoid(true);
            }
            else
            {
                removeUser(e.To);
            }

        }

        void client_ChannelEmote(object sender, ChannelEmoteEventArgs e)    //DOES NOT WORK
        {
            /*
            tempInt = getUserByName(e.From);
            if (!(users.ElementAt(tempInt).squelched))
            {
                if (users.ElementAt(tempInt).PictureID.Equals("op"))
                {
                    tempInt = 10;
                }
                else
                {
                    tempInt = 1; ;
                }
                cw.AddToChatWindow(tempInt, e.From, e.Message);
            } */
        }

        void client_ExceptionThrown(object sender, SCommon.ExceptionEventArgs e)
        {
            cw.getPanel().Content = new D2Error(e.Exception.Message);
            
        }

        void client_ChannelMessage(object sender, ChannelMessageEventArgs e)
        {
            try
            {
                tempInt = getUserByName(e.From);
                if (!(userIsSquelched(e.From)))
                {
                    if (users.ElementAt(tempInt).isOperator())
                    {
                        tempInt = 9;
                    }
                    else
                    {
                        tempInt = 0; ;
                    }
                    printMessage(tempInt, e.From, e.Message);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                printMessage(0, e.From, e.Message);
            }

        }

        #endregion

        public void printMessage(int code, string from, string msg) {
            tempInt = code;
            //AddToChatWindow(tempInt, e.From, e.Message);
            if (msg.Length > 6)
            {
                tempString = msg.Substring(0, 7);
                //Console.WriteLine("\n" + tempString);
                if (tempString.Contains("ACTION")) //Emote
                {
                    tempString = msg.Substring(8);
                    cw.AddToChatWindow(1, from, tempString);
                }

                else //Regular message
                {
                    cw.AddToChatWindow(tempInt, from, msg);
                }
            }
            else //Regular message
            {
                cw.AddToChatWindow(tempInt, from, msg);
            }
        }

    }
}

