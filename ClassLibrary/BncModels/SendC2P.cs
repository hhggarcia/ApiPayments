﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.BncModels
{
    public class SendC2P
    {
        public decimal Amount { get; set; }
        public string BranchID { get; set; } = string.Empty;
        public string ChildClientID { get; set; } = string.Empty;
        public int DebtorBankCode { get; set; }
        public string DebtorID { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

    }

    public class SendC2PResponse
    {
        public int IdTransation { get; set; }
        public string Reference { get; set; } = string.Empty;
    }
}