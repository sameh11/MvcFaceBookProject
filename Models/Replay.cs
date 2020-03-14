using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class Replay
    {
        [Required]
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        //[Required]
        //public int CommentId { get; set; }
        public string Text { get; set; }
      //  [Required]
      //  public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public string Photo { get; set; }
        //Relations
        public virtual Comment Comment { get; set; }//done
        public ApplicationUser user { get; set; }//done

       // public virtual IEnumerable<Like> Likeslist { get; set; }

    }
}
