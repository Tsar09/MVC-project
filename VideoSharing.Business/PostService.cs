using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL;
using VideoSharing.Business.DTO;
using VideoSharing.DAL.Entity;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;


namespace VideoSharing.Business
{
    public class PostService
    {
       IUnitOfWork Database { get; set; }
       private static string MATCHER = @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)";

       public PostService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public void AddPost(PostDTO postDto)
        {
            ClientProfile user = Database.ClientProfile.Get(postDto.UserID);
            // валидация
            if (user == null)
                throw new ValidationException("User not found","");

            Match youtubeMatch = new Regex(MATCHER).Match(postDto.Video);
            Post p = new Post
            {
                Description = postDto.Description,
                Video = youtubeMatch.Success ? youtubeMatch.Groups[1].Value : string.Empty,
                Date = DateTime.Now,
                UserID = postDto.UserID,
                User = user 
            };
           
            Database.Post.Create(p);
            Database.Save();
          }

        public void DeletePost(int id)
        {
            var d = GetPost(id);
            Database.Post.Delete(id);
            Database.Save();
        }

        public PostDTO EditPost(PostDTO post)
        {
            Mapper.CreateMap<PostDTO, Post>();
            var d = Mapper.Map<PostDTO, Post>(post);
            d.Date = DateTime.Now;
            Database.Post.Update(d);
            Database.Save();
            return post;
        }

        public IEnumerable<PostDTO> GetPosts()
        {
            Mapper.CreateMap<Post, PostDTO>();
            return Mapper.Map<IEnumerable<Post>, List<PostDTO>>(Database.Post.GetAll().ToList());
        }

        public IEnumerable<PostDTO> GetPosts(string uId)
        {
            if (uId == null)
                throw new ValidationException("Не установлено id поста", "");
            var u = Database.ClientProfile.Get(uId).Following;
           List<Post> posts = Database.Post.GetAll().Where(x => x.User != null && x.User.Id == uId)
               .ToList();

           if (posts == null)
               throw new ValidationException("Посты не найдены", "");

           foreach (Post p in Database.Post.GetAll())
           {
               foreach (ClientProfile c in u)
               {
                   if (p.UserID == c.Id)
                   {
                       posts.Add(p);
                   }
               }
           }
             
            
            // применяем автомаппер для проекции Post на PostDTO
            Mapper.CreateMap<Post, PostDTO>();
            return Mapper.Map<IEnumerable<Post>, List<PostDTO>>(posts);
        }

        public PostDTO GetPost(int id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id поста", "");
            var post = Database.Post.Get(id);
            if (post == null)
                throw new ValidationException("Пост не найден", "");
            // применяем автомаппер для проекции Post на PostDTO
            Mapper.CreateMap<Post, PostDTO>();
            return Mapper.Map<Post, PostDTO>(post);
        }

        public IEnumerable<PostDTO> Search(string searchString)
        {
            var p = GetPosts();
            if (!String.IsNullOrEmpty(searchString))
            {
                if (p != null)
                    p = p.Where(x => (x.Description.ToLower().StartsWith(searchString.ToLower()) || searchString == null));
            }
            return p;
        }

        public IEnumerable<PostDTO> Sort(string sortOrder, IEnumerable<PostDTO> posts)
        {
            switch (sortOrder)
            {
                case "description_desc":
                    posts = posts.OrderByDescending(s => s.Description);
                    break;
                case "Date":
                    posts = posts.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    posts = posts.OrderByDescending(s => s.Date);
                    break;
                case "User":
                    posts = posts.OrderBy(s => s.User.FullName);
                    break;
                case "user_desc":
                    posts = posts.OrderByDescending(s => s.User.FullName);
                    break;
                default:
                    posts = posts.OrderBy(s => s.Description);
                    break;
            }
            return posts;
        }

        public void Dispose()
        {
            Database.Dispose();
        }
     
    }


}
