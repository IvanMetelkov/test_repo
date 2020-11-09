using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIP4_database1.Models
{
    public class ErrorMessage
    {
        public Error error = new Error();
    }
    public class Error
    {
        public int code = 1;
        public string Message { get; set; }
    }
}
