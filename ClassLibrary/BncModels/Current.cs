using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class Current
    {
        public string ClientID { get; set; } = string.Empty;
        public string BranchID { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;
    }
    public class CurrentItem
    {
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
 