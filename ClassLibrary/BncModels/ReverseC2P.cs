using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class ReverseC2P
    {
        public decimal Amount { get; set; }
        public string BranchID { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;
        public int IdTransactionToReverse { get; set; }
        public string Terminal { get; set; } = string.Empty;
    }

    public class ReverseC2PResponse
    {
        public string Reference { get; set; } = string.Empty;
    }
}
