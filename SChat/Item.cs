using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SChat
{
    public class Item
    {
        public string PictureID { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string PictureString
        {
            get { return "/img/avatars/" + PictureID.ToString() + ".png"; }
        }

        public Item()
        {

        }

        public Item(string pic)
        {
            PictureID = pic;
            Name = "";
        }

        public Item(string pic, string text)
        {
            PictureID = pic;
            Name = text;
            Color = "Yellow";
        }

        public Item(string pic, string text, string color)
        {
            PictureID = pic;
            Name = text;
            Color = color;
        }
    }
}
