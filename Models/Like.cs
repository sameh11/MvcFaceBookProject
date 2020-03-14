using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class Like
    {
        [Required]
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
     
        public bool IsDeleted { get; set; }
        public React Type { get; set; }
        //Relation
        public virtual ApplicationUser Users { get; set; }//done
        public virtual Post Post { get; set; }//done

        public virtual Comment Comment { get; set; }//done


    }
}
