using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VideoSharing.DAL.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public bool EmailConfirmed { get; set; }
        public virtual ClientProfile ClientProfile { get; set; }
    }
}
