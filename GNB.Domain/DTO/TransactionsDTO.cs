using System;
using System.Collections.Generic;
using System.Text;

namespace GNB.Domain.DTO
{
    public class TransactionsDTO
    {
        public string SKU { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
