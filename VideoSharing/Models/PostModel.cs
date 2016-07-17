using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VideoSharing.DAL.Entity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VideoSharing.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public string Description { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public ClientProfile User { get; set; }
        [Required]
        public string Video { get; set; }
        public IEnumerable<Comment> Comments;
    }

    public class EditPostModel
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }
        public string Video { get; set; }
    }
}