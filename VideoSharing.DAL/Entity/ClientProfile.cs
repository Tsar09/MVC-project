using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoSharing.DAL.Entity
{
    public class ClientProfile
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }

        public string Name { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public ICollection<ClientProfile> Followers { get; set; }
        
        public ICollection<ClientProfile> Following { get; set; }

        public string FullName
        {
            get
            {
                return Name + " " + LastName;
            }
        }

        [NotMapped]
        public int PostCount { get; set; }
    }
}
