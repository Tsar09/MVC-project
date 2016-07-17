using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using VideoSharing.DAL.Entity;
using VideoSharing.DAL.Interfaces;

namespace VideoSharing.DAL.Repository
{
    public class PostRepository : IContentRepository<Post>, IDisposable
    {
         public ApplicationContext Database { get; set; }
         public PostRepository(ApplicationContext db)
        {
            Database = db;
        }

         public IEnumerable<Post> GetAll()
        {         
            return Database.Posts.Include(p => p.User).Include(x => x.User.Following);
        }

         public Post Get(int id)
         {
             return Database.Posts.Include(p => p.User).Include(x => x.User.Following).First(x => x.Id == id);
        }

         public void Create(Post post)
        {
            Database.Posts.Add(post);
        }

         public void Update(Post post)
        {

            Database.Entry(post).State = EntityState.Modified;
        }

         public IEnumerable<Post> Find(Func<Post, Boolean> predicate)
        {
            return Database.Posts.Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            Post post = Database.Posts.Find(id);
            if (post != null)
                Database.Posts.Remove(post);
        }

        public void Delete(string id)
        {
            Post post = Database.Posts.Find(id);
            if (post != null)
                Database.Posts.Remove(post);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
