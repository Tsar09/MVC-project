using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSharing.Business;
using VideoSharing.Business.DTO;
using VideoSharing.Models;
using Microsoft.AspNet.Identity;
using VideoSharing.DAL.Entity;
using VideoSharing.DAL.Repository;
using VideoSharing.DAL;
using AutoMapper;
using VideoSharing.Filters;

namespace VideoSharing.Controllers
{
    [Authorize]
    [ExceptionFilterAttribute]
    public class PostController : Controller
    {
        PostService postService = new PostService(new IdentityUnitOfWork());
        CommentService commentService = new CommentService(new IdentityUnitOfWork());

        public ActionResult AddPost(string searchString)
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
            return View("AddPost");
        }

        [HttpPost]
        public ActionResult AddPost(PostModel post)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    post.UserID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    Mapper.CreateMap<PostModel, PostDTO>();
                    var postDto = Mapper.Map<PostModel, PostDTO>(post);
                    postService.AddPost(postDto);
                    ModelState.Clear(); // очищаем поля для ввода
                RedirectToAction("AddPost");    
            }
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
            }
            return View(post);
        }

        public ActionResult EditPost(int id)
        {
            Mapper.CreateMap<PostDTO, EditPostModel>();
            var post = Mapper.Map<PostDTO, EditPostModel>(postService.GetPost(id));
            if (post != null)
            {
                EditPostModel model = new EditPostModel { Description = post.Description, Id = post.Id, Video = post.Video };
                return View(model);
            }
            return View("AddPost");
        }

        [HttpPost]
        public ActionResult EditPost(EditPostModel post)
        {
            if(ModelState.IsValid)
                {
                    post.UserID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    Mapper.CreateMap<EditPostModel, PostDTO>();
                    var postDto = Mapper.Map<EditPostModel, PostDTO>(post);
                    postService.EditPost(postDto);
            }
            return RedirectToAction("AddComment", new { id = post.Id });    
        }

        public ActionResult EditComment(int id)
        {
            Mapper.CreateMap<CommentDTO, CommentModel>();
            var comment = Mapper.Map<CommentDTO, CommentModel>(commentService.GetComment(id));
            if (comment != null)
            {
                CommentModel model = new CommentModel { Text = comment.Text, Id = comment.Id, PostID = comment.PostID, UserID = comment.UserID };
                return View(model);
            }
            return View("AddComment", new { id = comment.PostID});
        }

        [HttpPost]
        public ActionResult EditComment(CommentModel comment)
        {
            if (ModelState.IsValid)
            {              
                Mapper.CreateMap<CommentModel, CommentDTO>();
                var commentDTO = Mapper.Map<CommentModel, CommentDTO>(comment);
                commentService.EditComment(commentDTO);               
            }
            return RedirectToAction("AddComment", new { id = comment.PostID });
        }

        public ActionResult Posts()
        {
            string userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Mapper.CreateMap<PostDTO, PostModel>();
            var posts = Mapper.Map<IEnumerable<PostDTO>, List<PostModel>>(
                postService.GetPosts(userId));
            ViewBag.Id = userId;
            return PartialView("_PostsPartial", posts);
        }

        public ActionResult Comments(int id)
        {
                
            Mapper.CreateMap<CommentDTO, CommentModel>();
            var comments = Mapper.Map<IEnumerable<CommentDTO>, List<CommentModel>>(
                commentService.GetComments(id));
            ViewBag.Id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            return PartialView("_CommentsPartial",comments);
        }

        public ActionResult AddComment(int id, string searchString)
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
  
            return View("AddComment",new CommentModel { PostID = id });
        }

        public PartialViewResult Video(int id)
        {
            Mapper.CreateMap<PostDTO, PostModel>();
            var postDto = Mapper.Map<PostDTO, PostModel>(postService.GetPost(id));
            ViewBag.Id = System.Web.HttpContext.Current.User.Identity.GetUserId();
            return PartialView("_VideoPartial",postDto);
        }

        [HttpPost]
        public ActionResult AddComment(CommentModel comment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    comment.UserID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    Mapper.CreateMap<CommentModel, CommentDTO>();
                    var commentDto = Mapper.Map<CommentModel, CommentDTO>(comment);
                    commentService.AddComment(commentDto);
                    
                    RedirectToAction("AddComment", new { id = comment.PostID });
                    ModelState.Clear(); 
                }
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
            }

            return View(comment);
         
        }

        public RedirectResult Delete(int id)
        {
            postService.DeletePost(id);
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        public RedirectResult DeleteComment(int id)
        {
            int idPost = commentService.GetComment(id).PostID;
            commentService.DeleteComment(id);
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
           // return RedirectToAction("AddComment", new { id = idPost });
        }

        public ActionResult PostsTable(string searchString, string sortOrder, string filterValue)
        {         
            if (searchString == null)
            {
                searchString = filterValue;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.FilterValue = searchString;
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.DescriptionSortParam = String.IsNullOrEmpty(sortOrder) ? "description_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.UserNameSortParam = sortOrder == "User" ? "user_desc" : "User";
                      
            var posts = postService.Search(searchString);
            posts = postService.Sort(sortOrder, posts);

            Mapper.CreateMap<PostDTO, PostModel>();
            var p = Mapper.Map<IEnumerable<PostDTO>, List<PostModel>>(posts);

            return View(p);
        
        }

        public ActionResult CommentsTable(string searchString, string sortOrder, string filterValue)
        {
            if (searchString == null)
            {
                searchString = filterValue;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.FilterValue = searchString;
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.DescriptionSortParam = String.IsNullOrEmpty(sortOrder) ? "text_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.UserNameSortParam = sortOrder == "User" ? "user_desc" : "User";

            var comments = commentService.Search(searchString);
            comments = commentService.Sort(sortOrder, comments);

            Mapper.CreateMap<CommentDTO, CommentModel>();
            var p = Mapper.Map<IEnumerable<CommentDTO>, List<CommentModel>>(comments);

            return View(p);

        }

        protected override void Dispose(bool disposing)
        {
            postService.Dispose();
            base.Dispose(disposing);
        }
    }
}