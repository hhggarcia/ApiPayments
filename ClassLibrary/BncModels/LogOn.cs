using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class LogOn
    {
        public string ClientGuid { get; set; } = string.Empty;
    }

    public class LogOnResponse
    {
        public string WorkingKey { get; set; } = string.Empty;
    }
}
