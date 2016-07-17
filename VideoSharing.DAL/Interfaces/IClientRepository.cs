using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL.Entity;

namespace VideoSharing.DAL
{
    public interface IClientRepository<T> where T :class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Func<T, Boolean> predicate);
        void Create(T item);
        void Update(T item);
        T Get(string id);
        void Delete(string id);
        void Attach(T user);
    }
}
