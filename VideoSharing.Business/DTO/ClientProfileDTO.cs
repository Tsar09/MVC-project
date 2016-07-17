using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL.Entity;

namespace VideoSharing.Business.DTO
{
    public class ClientProfileDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int PostCount { get; set; }
        public IEnumerable<ClientProfileDTO> Following { get; set; }
        public string FullName { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public bool IsFollow { get; set; }
    }
}
