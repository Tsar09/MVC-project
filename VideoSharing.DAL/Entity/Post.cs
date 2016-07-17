using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoSharing.DAL.Entity
{
    public class Post
    {     
        
      //  [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Id { get; set; }      
        public string UserID { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

         [ForeignKey("UserID")]
        public virtual ClientProfile User { get; set; }
        public string Video { get; set; }

      //  public virtual VideoItem Video { get; set; }
        public virtual IEnumerable<Comment> Comments { get; set; }
       public Post()
       {
           Comments = new List<Comment>();
       } 
       
       // public User User { get; set; }
    }
}
