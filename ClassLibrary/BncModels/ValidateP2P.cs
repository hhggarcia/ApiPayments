using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class ValidateP2P
    {
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int BankCode { get; set; }
        public string BranchID { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;
        public string ClientID { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int Reference { get; set; }
        public DateTime RequestDate { get; set; }

    }

    public class ValidateP2PResponse
    {
        public decimal Amount { get; set; }
        public string BalanceDelta { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string ControlNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool MovementExists { get; set; }
        public string ReferenceA { get; set; } = string.Empty;
        public string ReferenceB { get; set; } = string.Empty;
        public string ReferenceC { get; set; } = string.Empty;
        public string ReferenceD { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
