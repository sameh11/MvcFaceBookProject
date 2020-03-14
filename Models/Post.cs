using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class Post
    {
        [Required]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public int Likes { get; set; }
        public string Photo { get; set; }
        public string Text { get; set; }
       
        public bool IsDeleted { get; set; }
        public Feeling feel { get; set; }
        //Relation
        public  ApplicationUser user { get; set; } //done
        public virtual IEnumerable<Comment> Comments { get; set; }//done
        public virtual IEnumerable<Like> Likeslist { get; set; }//done
    }
}
