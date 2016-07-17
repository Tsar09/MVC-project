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
    public class CommentRepository : IContentRepository<Comment>
    {
        private ApplicationContext db;

        public CommentRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Comment> GetAll()
        {
            return db.Comments.Include("Post").Include("User");
        }

        public Comment Get(int id)
        {
            return db.Comments.Include(p => p.Post).Include(x => x.User).First(x => x.ID == id);
        }

        public void Create(Comment profile)
        {
            db.Comments.Add(profile);
        }

        public void Update(Comment profile)
        {
            db.Entry(profile).State = EntityState.Modified;
        }

        public IEnumerable<Comment> Find(Func<Comment, Boolean> predicate)
        {
            return db.Comments.Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            Comment com = db.Comments.Find(id);
            if (com != null)
                db.Comments.Remove(com);
        }
    }
}
