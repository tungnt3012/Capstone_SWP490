using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models
{
    public class AjaxResponseViewModel<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}