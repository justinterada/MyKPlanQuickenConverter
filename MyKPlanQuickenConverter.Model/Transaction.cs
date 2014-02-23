using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyKPlanQuickenConverter.Model
{
    public class Transaction
    {
        public string FundName { get; set; }
        public TransactionType Type { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        public DateTime Date { get; set; }
    }
}
