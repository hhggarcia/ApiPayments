using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class ReverseC2Palt
    {
        public decimal Amount { get; set; }
        public int BanckCode { get; set; }
        public string BranchID { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;
        public string DebtorCellPhone { get; set; } = string.Empty;
        public string DebtorID { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
    }

    public class ReverseC2PaltResponse
    {
        public string Reference { get; set; } = string.Empty;
    }
}
