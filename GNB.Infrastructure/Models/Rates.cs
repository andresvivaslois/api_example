using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GNB.Infrastructure.Models
{
    public class Rates
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Rate { get; set; }
    }
}
