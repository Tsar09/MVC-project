using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoSharing.DAL.Entity
{
    public class Comment
    {
        [Key]
        public int ID { get; set; }
        public int PostID { get; set; }
        public string UserID { get; set; }
        public string Text { get; set; }

        // [DataType(DataType.Date)]
        // [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [ForeignKey("PostID")]
        public virtual Post Post { get; set; }
        [ForeignKey("UserID")]
        public virtual ClientProfile User { get; set; }
    }
}
