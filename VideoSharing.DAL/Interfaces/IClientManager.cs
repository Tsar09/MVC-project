using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL.Entity;

namespace VideoSharing.DAL
{
    public interface IClientManager : IDisposable
    {
        void Create(ClientProfile item);
    }
}
