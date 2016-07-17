using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoSharing.Business.DTO;

namespace VideoSharing.Business
{
    public interface IExceptionService
    {
        void AddException(ExceptionDTO exDTO);
        void DeleteExeption(int id);
        IEnumerable<ExceptionDTO> GetExceptions();
        ExceptionDTO GetException(int id);
    }
}
