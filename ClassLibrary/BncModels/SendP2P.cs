using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class SendP2P
    {
        public decimal Amount { get; set; }
        public int BeneficiaryBankCode { get; set; }
        public string BeneficiaryCellPhone { get; set; } = string.Empty;
        public string? BeneficiaryEmail { get; set; } = null!;
        public string BeneficiaryID { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        public string BranchID { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OperationRef { get; set; } = string.Empty;
    }

    public class SendP2PResponse
    {
        public string Reference { get; set; } = string.Empty;
        public string AuthorizationCode { get; set; } = string.Empty;
        public bool SwSwAlreadySent { get; set; }
    }
}
