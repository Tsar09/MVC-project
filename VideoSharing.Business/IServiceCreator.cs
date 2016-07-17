using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoSharing.Business
{
    public interface IServiceCreator
    {
        IUserService CreateUserService();
    }
}
