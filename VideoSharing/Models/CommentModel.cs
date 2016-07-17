using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VideoSharing.DAL.Entity;

namespace VideoSharing.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int PostID { get; set; }
        public string UserID { get; set; }
        [Required]
        public string Text { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public ClientProfile User { get; set; }
        public Post Post { get; set; }
    }
}