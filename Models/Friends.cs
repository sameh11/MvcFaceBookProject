using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class Friends
    {

        public string User1Id { get; set; }
        public string User2Id { get; set; }
        public DateTime FriendshipStart { get; set; }

        public virtual ApplicationUser User1 { get; set; }
        public virtual ApplicationUser User2 { get; set; }
    }
}
