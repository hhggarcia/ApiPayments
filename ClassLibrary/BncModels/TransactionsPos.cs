using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class TransactionsPos
    {
        public string Terminal { get; set; } = null!;
        public DateTime DtTransaction { get; set; }
        public string BranchID { get; set; } = null!;
        public string ChildClientID { get; set; } = null!;
    }

    public class TransactionPosResponse
    {
        public DateTime DtPayment { get; set; }
        public DateTime DtTransaction { get; set; }
        public string IdPOS { get; set; } = null!;
        public decimal Amount { get; set; }
        public string AuthorizationNumber { get; set; } = null!;
        public string IdLot { get; set; } = null!;
        public string CardType { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public string Reference { get; set; } = null!;
        public string HourTransaction { get; set; } = null!;
    }
}
