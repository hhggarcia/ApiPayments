using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class Request
    {
        public string ClientGuid { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Validation { get; set; } = string.Empty;
        public bool SwTestOperation { get; set; }
    }
}
