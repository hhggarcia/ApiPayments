using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class Response
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Validation { get; set; } = string.Empty;
    }
}