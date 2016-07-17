using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL.Identity;
using VideoSharing.DAL.Entity;
using VideoSharing.DAL.Interfaces;

namespace VideoSharing.DAL
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationUserManager UserManager { get; }
        IClientManager ClientManager { get; }
        ApplicationRoleManager RoleManager { get; }
        IExceptionRepository<ExceptionDescription> ExceptionDescription { get; }
        IClientRepository<ClientProfile> ClientProfile { get; }
        IContentRepository<Post> Post { get; }
        IContentRepository<Comment> Comment { get; }
        Task SaveAsync();
        void Save();
    }
}
