using System;
using System.Collections.Generic;

namespace Gemini.Models
{
    public partial class PosPaymentQr
    {
        public System.Guid Guid { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Template { get; set; }
        public bool Active { get; set; }
        public string Note { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}
