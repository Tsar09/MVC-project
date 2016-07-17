using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSharing.Business;
using VideoSharing.Models;
using VideoSharing.DAL;
using VideoSharing.DAL.Entity;
using Microsoft.AspNet.Identity;
using VideoSharing.Business.DTO;
using AutoMapper;
using VideoSharing.Filters;

namespace VideoSharing.Controllers
{
    [Authorize]
    [ExceptionFilterAttribute]
    public class UserController : Controller
    {
        
          ClientProfileService user = new ClientProfileService(new IdentityUnitOfWork());
      
        // GET: UserProfile
          public ActionResult Friends(string searchString)
          {
              if (!String.IsNullOrEmpty(searchString))
              {
                  if (!User.IsInRole("admin"))
                      return RedirectToAction("People", "User", new { searchString = searchString });
                  else
                  {
                      return RedirectToAction("PeopleTable", "User", new { searchString = searchString, sortOrder = "", filterValue = "" });
                  }
              }

            string id = System.Web.HttpContext.Current.User.Identity.GetUserId();         
             IEnumerable<ClientProfileDTO> following = user.GetFollowing(id);
             if (following != null)
             {
                 return View(following);
             }
             else return View("Error");            
        }

        [ExceptionFilterAttribute]
        public ActionResult Account(string id,string searchString)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                if (!User.IsInRole("admin"))
                    return RedirectToAction("People", "User", new { searchString = searchString });
                else
                {
                    return RedirectToAction("PeopleTable", "User", new { searchString = searchString, sortOrder = "", filterValue = "" });
                }
            }
                Mapper.CreateMap<ClientProfileDTO, UserModel>();
                var u = Mapper.Map<ClientProfileDTO, UserModel>(user.GetUser(id));

                string currentClientID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                if (id == currentClientID)
                {
                    return View(u);
                }
                else
                {
                    ViewBag.IsFollow = user.isFollowed(currentClientID, id);
                    return View("FAccount", u);
                }
          
            } 
               
        public RedirectToRouteResult AccountRedirect()
        {
            return RedirectToAction("Account", new { id = System.Web.HttpContext.Current.User.Identity.GetUserId() });
        }


        public ActionResult ChangeFollow(bool startFollow, string id)
        {
            string clientId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (!startFollow)
            {              
                user.StartFollowing(clientId, id);
                return RedirectToAction("Account", new { id = id });
            } else
            {
                user.StopFollowing(clientId, id);
                return RedirectToAction("Account", new { id = id });
            }
        }

        public ActionResult Unfollow(string id)
        {
            string clientId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                user.StopFollowing(clientId, id);
                return RedirectToAction("Friends", new { id = clientId });
        }

        public ActionResult People(string searchString)
        {
            var users = user.Search(searchString);
            Mapper.CreateMap<ClientProfileDTO, UserModel>();
            var u = Mapper.Map<IEnumerable<ClientProfileDTO>, List<UserModel>>(users);

            return View(u);

        }

        public ActionResult PeopleTable(string searchString, string sortOrder, string filterValue)
        {
            if (searchString == null)
            {
                searchString = filterValue;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.FilterValue = searchString;
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PostCountSortParm = sortOrder == "Post" ? "post_desc" : "Post";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "lastName_desc" : "LastName";
                      
            var users = user.Search(searchString);            
            users = user.Sort(sortOrder, users);

            Mapper.CreateMap<ClientProfileDTO, UserModel>();
            var u = Mapper.Map<IEnumerable<ClientProfileDTO>, List<UserModel>>(users);

            return View(u);
        }

        }
    
    }
