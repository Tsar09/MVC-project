using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL.Entity;

namespace VideoSharing.DAL.Repository
{
    public class ClientProfileRepository : IClientRepository<ClientProfile>
    {
        private ApplicationContext db;

        public ClientProfileRepository(ApplicationContext context)
        {
            this.db = context;
            db.Configuration.ProxyCreationEnabled = false;
        }

        public IEnumerable<ClientProfile> GetAll()
        {
            return db.ClientProfiles.Include("Following").Include("Posts");
        }

        public ClientProfile Get(string id)
        {
            return db.ClientProfiles.Include("Following").Include(p => p.Posts).First(x => x.Id == id);
        }

        public void Create(ClientProfile profile)
        {
            db.ClientProfiles.Add(profile);
           
        }

        public void Update(ClientProfile profile)
        {           
            db.Entry(profile).State = EntityState.Modified;
        }

        public void Attach(ClientProfile profile)
        {
            db.ClientProfiles.Attach(profile);
        }

        public IEnumerable<ClientProfile> Find(Func<ClientProfile, Boolean> predicate)
        {
            return db.ClientProfiles.Where(predicate).ToList();
        }
 
        public void Delete(string id)
        {
            ClientProfile profile = db.ClientProfiles.Find(id);
            if (profile != null)
                db.ClientProfiles.Remove(profile);
        }

        public void Dispose()
        {
            db.Dispose();
        }
       
    }
}
