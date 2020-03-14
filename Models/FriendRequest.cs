using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class FriendRequest
    {
     
       
        public string IdSend { get; set; }
        public string IdRecive { get; set; }
        public ApplicationUser Recive { get; set; }//done
        public ApplicationUser Send { get; set; }//done
        public RequestState requestState { get; set; }

    }
}
