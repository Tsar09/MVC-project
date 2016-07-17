using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL.Entity;
using VideoSharing.DAL.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using VideoSharing.DAL.Repository;
using System.Data.Entity.Validation;
using System.Diagnostics;
using VideoSharing.DAL.Interfaces;

namespace VideoSharing.DAL
{
    public class IdentityUnitOfWork : IUnitOfWork
    {
        private ApplicationContext db;

        private ApplicationUserManager userManager;
        private ApplicationRoleManager roleManager;
        private IClientManager clientManager;
        private IContentRepository<Post> posts;
        private IContentRepository<Comment> comments;
        private IClientRepository<ClientProfile> clientProfile;
        private IExceptionRepository<ExceptionDescription> exception;

        public IdentityUnitOfWork()
        {
            db = new ApplicationContext();
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(db));
            clientManager = new ClientManager(db);
        }
        public IContentRepository<Comment> Comment
        {
            get
            {
                if (comments == null)
                    comments = new CommentRepository(db);
                return comments;
            }
        }

        public IContentRepository<Post> Post
        {
            get
            {
                if (posts == null)
                    posts = new PostRepository(db);
                return posts;
            }
        }

        public IClientRepository<ClientProfile> ClientProfile
        {
            get
            {
                if (clientProfile == null)
                    clientProfile = new ClientProfileRepository(db);
                return clientProfile;
            }
        }

        public IExceptionRepository<ExceptionDescription> ExceptionDescription
        {
            get
            {
                if (exception == null)
                    exception = new ExceptionRepository(db);
                return exception;
            }
        }

        public ApplicationUserManager UserManager
        {
            get { return userManager; }
        }

        public IClientManager ClientManager
        {
            get { return clientManager; }
        }

        public ApplicationRoleManager RoleManager
        {
            get { return roleManager; }
        }

        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }

        public void Save()
        {
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    userManager.Dispose();
                    roleManager.Dispose();
                    clientManager.Dispose();
                    
                }
                this.disposed = true;
            }
        }
        
    }
}
