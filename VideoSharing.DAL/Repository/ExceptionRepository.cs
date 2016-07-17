using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL.Entity;
using VideoSharing.DAL.Interfaces;

namespace VideoSharing.DAL.Repository
{
    public class ExceptionRepository : IExceptionRepository<ExceptionDescription>
    {
          private ApplicationContext db;

          public ExceptionRepository(ApplicationContext context)
        {
            this.db = context;
        }

          public IEnumerable<ExceptionDescription> GetAll()
        {
            return db.ExceptionDescription;
        }

          public ExceptionDescription Get(int id)
        {
            return db.ExceptionDescription.First(x => x.Id == id);
        }

          public void Create(ExceptionDescription exc)
        {
            db.ExceptionDescription.Add(exc);
        }
 
        public void Delete(int id)
        {
            ExceptionDescription exc = db.ExceptionDescription.Find(id);
            if (exc != null)
                db.ExceptionDescription.Remove(exc);
        }

    }
}
