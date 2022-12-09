using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ini;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using WpfAnimatedGif;

namespace SCommon
{
    public class SpawnMaster
    {
        public String[] clientMsg;
        public String nick { get; set; }
        public String chan { get; set; }
        public String server { get; set; }
        public String persistentIgnoring = "disabled";
        public List<string> ads;
        public List<string> squelchedUsers;
        public Image banner;
        public bool showAds { get; set; }
        IniFile ini;
        private int tempInt;
        private string fileLine;
        private bool ignorepersist;
        Random rnd;

        public SpawnMaster(string nick, string server, string chan, Image ad)
        {
            rnd = new Random();
            ini = new IniFile(System.IO.Path.Combine(Environment.CurrentDirectory, @"config.ini"));
            try
            {
                ignorepersist = (Boolean.Parse(ini.IniReadValue("Settings", "ignorepersist")));
            }
            catch (FormatException e)
            {
                ignorepersist = false;
            }
            try
            {
                showAds = (Boolean.Parse(ini.IniReadValue("Settings", "ads")));
            }
            catch (FormatException e)
            {
                showAds = false;
            }
            banner = ad;
            squelchedUsers = new List<string>();
            this.nick = nick;
            this.server = server;
            this.chan = chan;
            defineMsgs();
            defineAds();
            setBanner();
            if (ignorepersist)
            {
                loadIgnoreList();
            }

        }

        /// <summary>
        /// Setup the timer for changing the banner ad.
        /// </summary>
        public void setBanner()
        {
            if (showAds)
            {
                ImageBehavior.SetAnimatedSource(banner, ResourceHelper.LoadBitmapFromResource("img/ads/" + ads[rnd.Next(ads.Count)] + ".gif"));
            }
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (showAds)
            {
                ImageBehavior.SetAnimatedSource(banner, ResourceHelper.LoadBitmapFromResource("img/ads/" + ads[rnd.Next(ads.Count)] + ".gif"));
            }
        }

        /// <summary>
        /// Check if a user is in the squelchedUsers List.
        /// </summary>
        /// <param name="user">The user to check</param>
        /// <returns>True if the user is found in the list.</returns>
        public bool userIsSquelched(string user)
        {
            return (squelchedUsers.Contains(user, StringComparer.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Toggles the persistent ignoring feature.
        /// </summary>
        public void toggleIgnore()
        {
            if (ignorepersist)
            {
                ignorepersist = false;
                persistentIgnoring = "disabled";
            }
            else
            {
                ignorepersist = true;
                persistentIgnoring = "enabled";
            }
            ini.IniWriteValue("Settings", "ignorepersist", ignorepersist.ToString());
            clientMsg[13] = "Persistent ignoring is now " + persistentIgnoring + ".";
        }

         ///<summary>
        ///Perform the actions the SpawnMaster should do when the connection is closing.
        ///</summary>
        public void close()
        {
            if (ignorepersist)
            {
                try
                {
                    //Pass the filepath and filename to the StreamWriter Constructor
                    StreamWriter sw = new StreamWriter(System.IO.Path.Combine(Environment.CurrentDirectory, @"ignorelist.txt"));
                    foreach (string user in squelchedUsers)
                    {
                        sw.WriteLine(user);
                    }

                    //Close the file
                    sw.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }

        }

        /// <summary>
        /// Loads the lines from ignorelist.txt into the squelchedUsers list.
        /// </summary>
        private void loadIgnoreList()
        {
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(System.IO.Path.Combine(Environment.CurrentDirectory, @"ignorelist.txt"));

                //Read the first line of text
                fileLine = sr.ReadLine();

                //Continue to read until you reach end of file
                while (fileLine != null)
                {
                    //write the lie to console window
                    squelchedUsers.Add(fileLine);
                    //Read the next line
                    fileLine = sr.ReadLine();
                }

                //close the file
                sr.Close();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

        }

        /// <summary>
        /// Updates the Banner ad if need be. 
        /// </summary>
        public void update()
        {
            try
            {
                if (showAds != (Boolean.Parse(ini.IniReadValue("Settings", "ads"))))
                {
                    showAds = (Boolean.Parse(ini.IniReadValue("Settings", "ads")));
                    if (showAds)
                    {
                        ImageBehavior.SetAnimatedSource(banner, ResourceHelper.LoadBitmapFromResource("img/ads/" + ads[rnd.Next(ads.Count)] + ".gif"));
                    }
                    else
                    {
                        ImageBehavior.SetAnimatedSource(banner, ResourceHelper.LoadBitmapFromResource("img/blank.gif"));
                    }
                }
            }
            catch (FormatException e)
            {
                showAds = false;
                ImageBehavior.SetAnimatedSource(banner, ResourceHelper.LoadBitmapFromResource("img/blank.gif"));
            }

        }

        /// <summary>
        /// Defines some common messages the client can display to the user.
        /// </summary>
        public void defineMsgs()
        {
            clientMsg = new String[14];
            clientMsg[0] = "Welcome to SpawnChat Beta, built off of a modified version of KoBE's IrcClient library\nfrom https://github.com/cshivers/IrcClient-csharp";
            clientMsg[1] = "Welcome back to SpawnChat!";
            clientMsg[2] = "You can't kick a channel operator";
            clientMsg[3] = "Invalid user.";
            clientMsg[4] = "You are not a channel operator.";
            clientMsg[5] = "No one hears you.";
            clientMsg[6] = "Enter/Leave Notifications: OFF";
            clientMsg[7] = "Enter/Leave Notifications: ON";
            clientMsg[8] = " was kicked out of the channel by " + nick + ".";
            clientMsg[9] = "That user is not logged on.";
            clientMsg[10] = "This channel does not have chat privileges.";
            clientMsg[11] = "You can't squlech yourself.";
            clientMsg[12] = "That is not a valid command.";
            clientMsg[13] = "Persistent ignoring is now " + persistentIgnoring + ".";
        }

        /// <summary>
        /// Define the list of Ads the program can choose from.
        /// </summary>
        public void defineAds()
        {
            ads = new List<string>();
            ads.Add("468_warcraft_instores");
            ads.Add("AF_banner_final");
            ads.Add("Boardgame_Banner_V4");
            ads.Add("CE-v4big-revised");
            ads.Add("d2tales");
            ads.Add("dbanner");
            ads.Add("DVD-collection9");
            ads.Add("Night-Elf_Now_big");
            ads.Add("Rev_Promo_Banner");
            ads.Add("rr_468_contest_2");
            ads.Add("sbanner");
            ads.Add("wcg_468_60_east");
            ads.Add("toycomactionfigures468");
            ads.Add("vikings_468");
            ads.Add("war2bne");
            ads.Add("War3XP");
            ads.Add("WC III EXP banner");
            ads.Add("wowtcg");
            ads.Add("13930-WarCraft-banner");
            ads.Add("468x60_warcraft_final_3days");
            ads.Add("Blizzard-INS-Bannerv31");
            ads.Add("F_LVbanner_99k");
            ads.Add("lordoftheclans");
            ads.Add("mysterytournament3");
            ads.Add("Orc_Now_big");
            ads.Add("wbanner");
            ads.Add("rnrr_468x60_battlenet");
            ads.Add("SC-banner-801-2");
            ads.Add("wc_banner_728x90");
            ads.Add("StarCraft_DVDBanner41");
            ads.Add("Undead_Now_big");
            ads.Add("V1_w3shirtbanner");
            ads.Add("war3instores");
            ads.Add("war3ladder");
            ads.Add("warrpg4682");
        }

    }
}
