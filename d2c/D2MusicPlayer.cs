using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace d2music
{
    public class D2MusicPlayer
    {
        private SoundPlayer player;
        private List<String> classicList;
        private List<String> lodList;
        public int mode { get; set; }
        public double volume { get; set; }

        public D2MusicPlayer(bool classic, bool exp, double vol)
        {
            if (classic && !exp)
            {
                mode = 1;
            }
            else if (!classic && exp)
            {
                mode = 2;
            }
            else if (classic & exp)
            {
                mode = 3;
            }
            else
            {
                mode = 0;
            }
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
            start();
        }

        private void start()
        {
            switch (mode)
            {
                case 1:
                    player = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "music\\" + classicList[0] + ".wav");
                    break;
                case 2:
                    player = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "music\\" + lodList[0] + ".wav");
                    break;
                default:
                    break;
            }
            player.Play();
        }


    }
}
