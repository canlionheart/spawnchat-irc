using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SChat
{
    public static class ChatGem
    {
        public static SpChBtn gem { get; set; }
        public static D2ChatWindow cw { get; set; }
        public static bool activated { get; set;}

        public static void toggle()
        {
            switch (activated)
            {
                case true:
                    gem.setImages("d2dv/chatgem/GemDeactivated", "d2dv/chatgem/GemDeactivatedPressed");
                    activated = false;
                    cw.AddToChatWindow(10, null, "Gem Deactivated.");
                    break;
                default:
                    gem.setImages("d2dv/chatgem/GemActivated", "d2dv/chatgem/GemActivatedPressed");
                    activated = true;
                    cw.AddToChatWindow(10, null, "Gem Activated.");
                    break;
            }
            gem.dePress();
        }
       
    }
}
