using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class Send
    {
        public int AccountType { get; set; }
        public int AffiliationNumber { get; set; }
        public decimal Amount { get; set; }
        public int CardHolderID { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public int CardNumber { get; set; }
        public int CardPIN { get; set; }
        public string BranchID { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;
        public int CVV { get; set; }
        public int dtExpiration { get; set; }
        public int idCardType { get; set; }
        public string TransactionIdentifier { get; set; } = string.Empty;
    }

    public class SendResponse
    {
        public int Reference { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
