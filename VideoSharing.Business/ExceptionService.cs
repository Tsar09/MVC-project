using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.Business.DTO;
using VideoSharing.DAL;
using VideoSharing.DAL.Entity;

namespace VideoSharing.Business
{
    public class ExceptionService : IExceptionService
    {
         IUnitOfWork Database { get; set; }

         public ExceptionService(IUnitOfWork uow)
        {
            Database = uow;
        }

         public void AddException(ExceptionDTO exDTO)
         { 
             ExceptionDescription c = new ExceptionDescription
             {
                 ExceptionMessage = exDTO.ExceptionMessage,
                 StackTrace = exDTO.StackTrace,
                 ControllerName = exDTO.ControllerName,
                 ActionName = exDTO.ActionName,
                 Date = DateTime.Now
             };

             Database.ExceptionDescription.Create(c);
             Database.Save();
         }

         public void DeleteExeption(int id)
         {
             var d = GetException(id);
             Database.ExceptionDescription.Delete(id);
             Database.Save();
         }

         public IEnumerable<ExceptionDTO> GetExceptions()
         {
             Mapper.CreateMap<ExceptionDescription, ExceptionDTO>();
             return Mapper.Map<IEnumerable<ExceptionDescription>, List<ExceptionDTO>>(Database.ExceptionDescription.GetAll().ToList());
         }

         public ExceptionDTO GetException(int id)
         {
             if (id == null)
                 throw new ValidationException("Не установлено id комменария", "");
             var exception = Database.ExceptionDescription.Get(id);
             if (exception == null)
                 throw new ValidationException("Комментарий не найден", "");

             Mapper.CreateMap<ExceptionDescription, ExceptionDTO>();
             return Mapper.Map<ExceptionDescription, ExceptionDTO>(exception);
         }

    }
}
