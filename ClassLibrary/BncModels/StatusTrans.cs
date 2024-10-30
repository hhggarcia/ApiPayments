using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class StatusTrans
    {
        public string OperationType { get; set; } = string.Empty;
        public string BranchID { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }

    public class StatusTransResponse
    {
        public string Reference { get; set; } = string.Empty;
        public string TxSts { get; set; } = string.Empty;
        public string RsnDescription { get; set; } = string.Empty;
    }
}
