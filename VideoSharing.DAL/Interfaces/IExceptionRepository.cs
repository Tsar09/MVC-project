using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoSharing.DAL.Interfaces
{
    public interface IExceptionRepository<T> where T : class
    {
        void Delete(int id);
        T Get(int id);
        IEnumerable<T> GetAll();
        void Create(T item);
    }
}
