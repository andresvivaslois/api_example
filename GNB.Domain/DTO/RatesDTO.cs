using System;
using System.Collections.Generic;
using System.Text;

namespace GNB.Domain.DTO
{
    public class RatesDTO
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Rate { get; set; }
    }
}
