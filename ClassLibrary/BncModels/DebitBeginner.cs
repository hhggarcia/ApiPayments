using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class DebitBeginner
    {
        public string Amount { get; set; } = string.Empty;
        public string BranchID { get; set; } = string.Empty;
        public string DebtorBank { get; set; } = string.Empty;
        public string DebtorID { get; set; } = string.Empty;
        public string DebtorAccType { get; set; } = string.Empty;
        public string DebtorAccount { get; set; } = string.Empty;
        public string DebtorName { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;
        public string Concept { get; set; } = string.Empty;
        public int AddtlInf { get; set; }
    }

    public class DebitBeginnerResonse
    {
        public string Reference { get; set; } = string.Empty;
    }
}
