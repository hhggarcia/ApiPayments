using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class CreditBeginner
    {
        public string Amount { get; set; } = string.Empty;
        public string BranchID { get; set; } = string.Empty;
        public string CreditorBank { get; set; } = string.Empty;
        public string CreditorID { get; set; } = string.Empty;
        public string CreditorAccType { get; set; } = string.Empty;
        public string CreditorAccount { get; set; } = string.Empty;
        public string CreditorName { get; set; } = string.Empty;
        public string Concept { get; set; } = string.Empty;
    }

    public class CreditBeginnerResponse
    {
        public string Reference { get; set; } = string.Empty;
    }
}
