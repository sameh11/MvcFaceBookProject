using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Facebook.Models
{
    public class ApplicationUser :IdentityUser
    {
      
        public string FName { get; set; }
        public string LName { get; set; }
    

        public DateTime? BrithDate { get; set; }

        public string Photo { get; set; }

        public string Address { get; set; }
        public string Bio { get; set; }
        public bool IsDeleted { get; set; }

        //Relations
        public virtual IEnumerable<Like> Likes { get; set; }//done
        public virtual IEnumerable<Comment> Comments { get; set; } //done
        public virtual IEnumerable<Post> Posts { get; set; } //Done
        public virtual IEnumerable<Replay> Replays { get; set; }//done
        public virtual IEnumerable<FriendRequest> RequestFriendRecives { get; set; }//done
        public virtual IEnumerable<FriendRequest> FriendRequests { get; set; }//done

        public virtual IEnumerable<Friends> MyFriends { get; set; }//done
    }
}
