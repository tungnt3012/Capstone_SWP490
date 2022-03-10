using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models
{
    public class PagingOutput <T>
    {
        public int Index { get; set; }
        public int PageSize { get; set; }
        public int TotalItem { get; set; }
        public int TotalPage { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}