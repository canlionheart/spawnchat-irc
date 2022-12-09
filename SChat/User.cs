using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SChat
{
    public class User 
    {
        public string PictureID { get; set; }
        public string Name { get; set; }
        public bool squelched { get; set; }
        public string PictureString
        {
            get { return "/img/avatars/" + PictureID.ToString() + ".png"; }
        }

        public User()
        {

        }

        public User(string pic, string text)
        {
            PictureID = pic;
            Name = text;
            squelched = false;
        }

        public User(string pic, string text, bool squel)
        {
            PictureID = pic;
            Name = text;
            squelched = squel;
        }
    }
}
