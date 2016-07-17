using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VideoSharing.DAL.Entity;

namespace VideoSharing.Models
{
    public class UserModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public bool IsFollowed { get; set; }
        public int PostCount { get; set; }
        public IEnumerable<UserModel> Following { get; set; }
        public string FullName { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public bool IsFollow { get; set; }
    }
}