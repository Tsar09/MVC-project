using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VideoSharing.Models
{
    public class ExceptionModel
    {
        public int Id { get; set; }
        public string ExceptionMessage { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string StackTrace { get; set; }
    }
}