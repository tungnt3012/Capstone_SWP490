using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.ExceptionHandler.Interfaces
{
    abstract class IException : Exception
    {
        private IException() { }
        public IException(string code, string message, Dictionary<string, object> param)
        {
            this.code = code;
            this.message = message;
            this.param = param;
        }
        public string code { get; set; }
        public string message { get; set; }
        public Dictionary<string, object> param {get;set;}
    }
}
