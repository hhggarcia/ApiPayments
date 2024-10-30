using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class ReverseDebit
    {
        public string Reference { get; set; } = string.Empty;
        public string BranchID { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;

    }

    public class ReverseDebitResponse
    {
        public string Reference { get; set; } = string.Empty;
    }
}
