using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.DAL;
using VideoSharing.Business.DTO;
using VideoSharing.DAL.Entity;
using AutoMapper;

namespace VideoSharing.Business
{
    public class CommentService
    {
        IUnitOfWork Database { get; set; }

        public CommentService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public void AddComment(CommentDTO comDTO)
        {
            ClientProfile user = Database.ClientProfile.Get(comDTO.UserID);
            Post post = Database.Post.Get(comDTO.PostID);

            if (post == null)
                throw new ValidationException("Comment not found","");

            Comment c = new Comment
            {
                Text = comDTO.Text,
                Date = DateTime.Now,
                PostID = comDTO.PostID,
                UserID = comDTO.UserID,
                User = user
            };
            c.User = user;
            c.Post = post;
           
            Database.Comment.Create(c);
            Database.Save();
          }

        public void DeleteComment(int id)
        {
            var d = GetComment(id);
            Database.Comment.Delete(id);
            Database.Save();
        }

        public CommentDTO EditComment(CommentDTO comment)
        {
            Mapper.CreateMap<CommentDTO, Comment>();
            var d = Mapper.Map<CommentDTO, Comment>(comment);
            d.Date = DateTime.Now;
            Database.Comment.Update(d);
            Database.Save();
            return comment;
        }

        public IEnumerable<CommentDTO> GetComments()
        {
            Mapper.CreateMap<Comment, CommentDTO>();
            return Mapper.Map<IEnumerable<Comment>, List<CommentDTO>>(Database.Comment.GetAll().ToList());
        }

        public IEnumerable<CommentDTO> GetComments(int pId)
        {
            var comments = Database.Comment.GetAll().Where(x => x.Post != null && x.Post.Id == pId);
            if (comments == null)
                throw new ValidationException("Комментарий не найден", "");

            Mapper.CreateMap<Comment, CommentDTO>();
            return Mapper.Map<IEnumerable<Comment>, List<CommentDTO>>(comments);
        }
 
        public CommentDTO GetComment(int id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id комменария", "");
            var comment = Database.Comment.Get(id);
            if (comment == null)
                throw new ValidationException("Комментарий не найден", "");

            Mapper.CreateMap<Comment, CommentDTO>();
            return Mapper.Map<Comment, CommentDTO>(comment);
        }

        public IEnumerable<CommentDTO> Search(string searchString)
        {
            var p = GetComments();
            if (!String.IsNullOrEmpty(searchString))
            {
                if (p != null)
                    p = p.Where(x => (x.Text.ToLower().StartsWith(searchString.ToLower()) 
                        || x.User.FullName.ToLower().StartsWith(searchString.ToLower())
                        || searchString == null));
            }
            return p;
        }

        public IEnumerable<CommentDTO> Sort(string sortOrder, IEnumerable<CommentDTO> comments)
        {
            switch (sortOrder)
            {
                case "text_desc":
                    comments = comments.OrderByDescending(s => s.Text);
                    break;
                case "Date":
                    comments = comments.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    comments = comments.OrderByDescending(s => s.Date);
                    break;
                case "User":
                    comments = comments.OrderBy(s => s.User.FullName);
                    break;
                case "user_desc":
                    comments = comments.OrderByDescending(s => s.User.FullName);
                    break;
                default:
                    comments = comments.OrderBy(s => s.Text);
                    break;
            }
            return comments;
        }
    }
}
