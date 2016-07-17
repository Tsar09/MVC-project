using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSharing.Business;
using VideoSharing.Business.Infrastructure;
using VideoSharing.DAL.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Security.Claims;
using VideoSharing.Models;
using VideoSharing.Business.DTO;
using VideoSharing.DAL;
using System.Net.Mail;
using System.Net;
using AutoMapper;
using VideoSharing.DAL.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using VideoSharing.Filters;

namespace VideoSharing.Controllers
{
    [Authorize]
    [ExceptionFilterAttribute]
    public class AccountController : Controller
    {
        ClientProfileService client = new ClientProfileService(new IdentityUnitOfWork());

        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

         [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]       
        public async Task<ActionResult> Login(LoginModel model)
        {
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                AccountDTO userDto = new AccountDTO { Email = model.Email, Password = model.Password };
                ClaimsIdentity claim = await UserService.Authenticate(userDto);
                if (claim == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous] 
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> Register(RegisterModel model)
        {
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                AccountDTO userDto = new AccountDTO
                {
                    Email = model.Email,
                    Password = model.Password,
                    LastName = model.LastName,
                    Name = model.Name,
                    Role = "user"
                };
                OperationDetails operationDetails = await UserService.Create(userDto);
                if (operationDetails.Succedeed)
                {
        
                    string toaddr = userDto.Email; 
                    string msgBody = string.Format("Для завершения регистрации перейдите по ссылке:" +
                                    "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                        Url.Action("ConfirmEmail", "Account", new { Token = userDto.Id, Email = userDto.Email }, Request.Url.Scheme));

                    SentMail(toaddr, msgBody);                

                    return RedirectToAction("Confirm", "Account", new { Email = userDto.Email });
                }
                else
                    ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult Confirm(string Email)
        {
            ViewBag.Message = Email;
            return View("SuccessRegister");
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string Token, string Email)
        {
                await UserService.ConfirmEmail(Token, Email);
                return RedirectToAction("Index", "Home", new { EmailConfirmed = Email });
            }

        public ActionResult Delete(UserModel model)
        {
            return View("Delete", model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            if (System.Web.HttpContext.Current.User.Identity.GetUserId() == id)
            {
                LogOff();
            }
            await UserService.Delete(id);    
            return RedirectToAction("PeopleTable", "User");
        }


        public ActionResult Edit(string id)
        {
            Mapper.CreateMap<ClientProfileDTO, EditModel>();
            var user = Mapper.Map<ClientProfileDTO, EditModel>(client.GetUser(id));
            if (user != null)
            {
                EditModel model = new EditModel { Name = user.Name, LastName = user.LastName, Id = user.Id };
                return View(model);
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditModel model)
        {
            Mapper.CreateMap<EditModel, ClientProfileDTO>();
            var user = Mapper.Map<EditModel, ClientProfileDTO>(model);
            OperationDetails operationDetails = await UserService.Edit(user);
                if (operationDetails.Succedeed)
                {
                    if(model.Id == System.Web.HttpContext.Current.User.Identity.GetUserId())
                         return RedirectToAction("Account", "User", new {id = model.Id });
                    else return RedirectToAction("Account", "User", new { id = model.Id });
                }
            
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
           // AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<ForgotPasswordModel, ForgotPasswordDTO>();
                var mod = Mapper.Map<ForgotPasswordModel, ForgotPasswordDTO>(model);

                UserInfoDTO helper = await UserService.ForgotPassword(mod);

                string to = model.Email;
                  var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = helper.id, code = helper.code }, protocol: Request.Url.Scheme);

                string body =  "Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>";

                SentMail(to,body);

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(PasswordResertModel model)
        {
            Mapper.CreateMap<PasswordResertModel, PasswordResertDTO>();
            var mod = Mapper.Map<PasswordResertModel, PasswordResertDTO>(model);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            OperationDetails operationDetails = await UserService.ResetPassword(mod);
            if (operationDetails.Succedeed)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            return View();
        }

        private async Task SetInitialDataAsync()
        {
            await UserService.SetInitialData(new AccountDTO
            {
                Email = "admin@mail.ru",
                UserName = "admin@mail.ru",
                Password = "qwerty",
                Name = "Admin",
                LastName = "VideoSharing",
                Role = "admin",
                EmailConfirmed= true
            }, new List<string> { "user", "admin" });
        }

        private static void SentMail(string to, string body)
        {
            string from = "costa2305rica@gmail.com";
            string password = "videosharing";

            MailMessage msg = new MailMessage();
            msg.Subject = "Email confirmation";
            msg.From = new MailAddress("costa2305rica@gmail.com", "VideoSharing");
            msg.From = new MailAddress(from);

            msg.To.Add(new MailAddress(to));
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential(from, password);
            smtp.Credentials = nc;

            msg.Body = body;              
            smtp.Send(msg);
        }
      
    }
}