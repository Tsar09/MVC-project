using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL.Entity;

namespace VideoSharing.Business.DTO
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public ClientProfile User { get; set; }
        public string Video { get; set; }
        public IEnumerable<Comment> Comments;
    }
}
