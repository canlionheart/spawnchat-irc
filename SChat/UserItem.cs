using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SChat
{
    class UserItem
    {
        public User user { get; set; }
        public int position { get; set; }

        public UserItem(User user, int pos) {
            this.user = new User(user.PictureID, user.Name);
            position = pos;
    }

    }
}
