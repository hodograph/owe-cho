using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }

        public string TransactionName { get; set; }

        public string Payer { get; set; }

        public double Total { get; set; }

        public TransactionType TransactionType { get; set; }

        public List<Debt> Debts { get; set; } = new List<Debt>();
    }
}
