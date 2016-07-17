using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.Business.DTO;
using VideoSharing.DAL.Entity;
using Microsoft.AspNet.Identity;
using VideoSharing.DAL;
using AutoMapper;
using System.Collections.ObjectModel;

namespace VideoSharing.Business
{
    public class ClientProfileService
    {
        IUnitOfWork Database { get; set; }

        public ClientProfileService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public IEnumerable<ClientProfileDTO> GetUsers()
        {
            Mapper.CreateMap<ClientProfile, ClientProfileDTO>();
            return Mapper.Map<IEnumerable<ClientProfile>, List<ClientProfileDTO>>(Database.ClientProfile
                .GetAll().Select(x => { x.PostCount = Database.Post.GetAll().Count(s => s.User != null && s.User.Id == x.Id) ; return x; })); // вычисляем количетсво постов 
        }

        public ClientProfileDTO GetUser(string id)
        {
            try
            {
                if (id == null)
                    throw new ValidationException("Не установлено id пользователя", "");
                var u = GetUsers().First(x => x.Id == id);
                return u;
            } catch(Exception)
            {
                throw new Exception("User not found");
            }
        }

        public IEnumerable<ClientProfileDTO> Search(string searchString)
        {
            var u = GetUsers();
            if (!String.IsNullOrEmpty(searchString))
            {              
                if (u != null)
                    u = u.Where(p => (p.Name.ToLower().StartsWith(searchString.ToLower())
                            || p.LastName.ToLower().StartsWith(searchString.ToLower()) || searchString == null));
            }
            return u;
        }

        public IEnumerable<ClientProfileDTO> Sort(string sortOrder, IEnumerable<ClientProfileDTO> users)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    users = users.AsQueryable<ClientProfileDTO>().OrderByDescending(s => s.Name);
                    break;
                case "Post":
                    users = users.OrderBy(s => s.PostCount);
                    break;
                case "post_desc":
                    users = users.OrderByDescending(s => s.PostCount);
                    break;
                case "LastName":
                    users = users.OrderBy(s => s.LastName);
                    break;
                case "lastName_desc":
                    users = users.OrderByDescending(s => s.LastName).ToList();
                    break;
                default:
                    users = users.AsQueryable<ClientProfileDTO>().OrderBy(s => s.Name).ToList();
                    break;
            }
            return users;
        }

        public ICollection<ClientProfileDTO> GetFollowing(string id)
        {
            Mapper.CreateMap<ClientProfileDTO, ClientProfileDTO>();
            var u = GetUser(id).Following;
            return Mapper.Map<IEnumerable<ClientProfileDTO>, List<ClientProfileDTO>>(u);
        }

        public void StartFollowing(string userID, string followingID)
        {
            ClientProfile u = Database.ClientProfile.Get(userID);
            ClientProfile f = Database.ClientProfile.Get(followingID);

           (u.Following ?? (u.Following = new Collection<ClientProfile>())).Add(f);

           Database.ClientProfile.Attach(u);
           Database.ClientProfile.Update(u);
            Database.Save();
        }

        public void StopFollowing(string userID, string followingID)
        {
            ClientProfile u = Database.ClientProfile.Get(userID);
            ClientProfile f = Database.ClientProfile.Get(followingID);
            if (u != null)
                if (f != null)
                   u.Following.Remove(f);
            Database.Save();           
            
        }
        
        public bool isFollowed(string uId, string followingId)
        {
            var u = Database.ClientProfile.Get(uId).Following;
            if (u != null)
                return u.Any(f => f.Id == followingId);
            return false;
        }

      
    }
}
