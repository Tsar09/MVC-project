using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using VideoSharing.Business.Infrastructure;
using VideoSharing.Business.DTO;
using VideoSharing.DAL.Identity;
using VideoSharing.DAL;
using VideoSharing.DAL.Entity;
using Microsoft.AspNet.Identity;

using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

namespace VideoSharing.Business
{
    public class UserAccountService : IUserService
    {
        IUnitOfWork Database { get; set; }

        public UserAccountService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<OperationDetails> Create(AccountDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.Email };
                await Database.UserManager.CreateAsync(user, userDto.Password);
                // add role
                await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);

                // add clients' profile
                ClientProfile clientProfile = new ClientProfile { Id = user.Id, LastName = userDto.LastName, Name = userDto.Name };
                Database.ClientManager.Create(clientProfile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Регистрация успешно пройдена", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователь с таким логином уже существует", "Email");
            }
        }

        public async Task<OperationDetails> Delete(string id)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                var u = Database.ClientProfile.Get(id);
                Database.Comment.GetAll().ToList().RemoveAll(x => x.UserID == id);
                Database.Post.GetAll().ToList().RemoveAll(x => x.UserID == id);
                Database.ClientProfile.Delete(user.Id);              
                await Database.UserManager.DeleteAsync(user);                           
                await Database.SaveAsync();
                Database.Save();
                return new OperationDetails(true, "Удаление выполнено", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователь не найден", "id");
            }
        }

        public async Task<OperationDetails> Edit(ClientProfileDTO model)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                 var u = Database.ClientProfile.Get(model.Id);
                 u.Name = model.Name;
                 u.LastName = model.LastName;

                 Database.ClientProfile.Update(u);
                 Database.Save();
                 await Database.UserManager.UpdateAsync(user);

                    return new OperationDetails(true, "Редактирование выполнено", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователь не найден", "id");
            }
        }

        public async Task<ClaimsIdentity> Authenticate(AccountDTO userDto)
        {
            ClaimsIdentity claim = null;
            // находим пользователя
            ApplicationUser user = await Database.UserManager.FindAsync(userDto.Email, userDto.Password);
            // авторизуем пользователя и возвращаем объект ClaimsIdentity
            if (user != null)
                if (user.EmailConfirmed || user.Email == "admin@mail.ru")
                claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
            return claim;
        }

        // начальная инициализация бд
        public async Task SetInitialData(AccountDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await Database.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }
            }
            await Create(adminDto);
        }

        public async Task ConfirmEmail(string Token, string Email)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(Email);
            if (user != null)
            {
                if (user.Email == Email)
                {
                    user.EmailConfirmed = true;
                    await Database.UserManager.UpdateAsync(user);
               
                }
            }
        }

        public async Task<UserInfoDTO> ForgotPassword(ForgotPasswordDTO model)
        {

            var user = await Database.UserManager.FindByNameAsync(model.Email);
            if (user != null || (user.EmailConfirmed))
            {
                var provider = new DpapiDataProtectionProvider("VideoSharing");

                Database.UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                    provider.Create("ResetPassword"));
            }
                string code = await Database.UserManager.GeneratePasswordResetTokenAsync(user.Id);
            

            return new UserInfoDTO { code = code, id = user.Id };
         
        }

        public async Task<OperationDetails> ResetPassword(PasswordResertDTO model)
        {
            var provider = new DpapiDataProtectionProvider("VideoSharing");

            Database.UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                provider.Create("ResetPassword"));

            var user = await Database.UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return new OperationDetails(false, "Пользователь не найден", "id");
            }
            var result = await Database.UserManager.ResetPasswordAsync(user.Id,model.Code, model.Password);
            return new OperationDetails(true, "Сброс пароля выполнен", "");
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
