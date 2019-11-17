using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GNB.Infrastructure.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
