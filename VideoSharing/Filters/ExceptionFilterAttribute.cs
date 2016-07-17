using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VideoSharing.Business;
using VideoSharing.Business.DTO;
using VideoSharing.DAL;
using VideoSharing.Models;

namespace VideoSharing.Filters
{
    public class ExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {

        public void OnException(ExceptionContext filterContext)
        {
            IExceptionService commentService = new ExceptionService(new IdentityUnitOfWork());
            ExceptionModel exceptionDetail = new ExceptionModel()
            {
                ExceptionMessage = filterContext.Exception.Message,
                StackTrace = filterContext.Exception.StackTrace,
                ControllerName = filterContext.RouteData.Values["controller"].ToString(),
                ActionName = filterContext.RouteData.Values["action"].ToString(),
            };

            Mapper.CreateMap<ExceptionModel, ExceptionDTO>();
            var exception = Mapper.Map<ExceptionModel, ExceptionDTO>(exceptionDetail);
            commentService.AddException(exception);
       
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Error/Index.cshtml",
                 
            };
         //   filterContext.Result = new RedirectResult("/User/Error.html");
            filterContext.ExceptionHandled = true;
        }
    }
}