using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL.Entity;

namespace VideoSharing.Business.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public int PostID { get; set; }
        public string UserID { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public ClientProfile User { get; set; }
        public Post Post { get; set; }
    }
}
