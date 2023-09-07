using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorApp.Shared
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }

        public string TransactionName { get; set; }

        public string Payer { get; set; }

        public double Total { get; set; }

        public List<Debt> Debts { get; set; } = new List<Debt>();

        public List<Debt> CalculatePayments()
        {
            List<Debt> debts = new List<Debt>();

            Debts.ForEach(x => debts.Add(new Debt() { Amount = x.Amount, Debtor = x.Debtor }));

            double debtsTotal = 0;
            debts.ForEach(x => debtsTotal += x.Amount);
            double remainder = Total - debtsTotal;

            if (!debts.Any(x => x.Debtor == Payer))
            {
                debts.Add(new Debt()
                {
                    Debtor = Payer,
                    Amount = 0
                });
            }

            double remainingSplit = remainder / debts.Count;
            debts.ForEach(x => x.Amount += remainingSplit);

            return debts;
        }
    }
}
