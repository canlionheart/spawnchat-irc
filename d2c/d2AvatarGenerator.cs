using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d2c
{
    public static class d2AvatarGenerator
    {
        static Random rnd = new Random();
        static d2avatar avatar;
        static int tempInt = 0;

        /// <summary>
        /// Picks a random class and equips random armor.
        /// </summary>
        /// <returns></returns>
        public static d2avatar generate()
        {
            avatar = new d2avatar();
            tempInt = rnd.Next(13);
            avatar.setClass(tempInt);
            if (tempInt < 5)
            {
                avatar.setPreset(rnd.Next(7));
            }
            return avatar;
        }

        /// <summary>
        /// Get a randomly generated avatar with an assigned name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static d2avatar generate(string name)
        {
            avatar = generate();
            avatar.Name = name;
            return avatar;
        }
    }
}
