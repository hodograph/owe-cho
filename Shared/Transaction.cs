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

        public bool ProportionalSplit { get; set; }

        public List<Debt> CalculatePayments()
        {
            List<Debt> debts = new List<Debt>();

            Debts.ForEach(x => debts.Add(new Debt() { Amount = x.Amount, Debtor = x.Debtor }));

            double debtsTotal = debts.Select(x => x.Amount).Sum();
            double remainder = Total - debtsTotal;

            if (!debts.Any(x => x.Debtor == Payer))
            {
                debts.Add(new Debt()
                {
                    Debtor = Payer,
                    Amount = 0
                });
            }

            if (ProportionalSplit)
            {
                foreach(Debt debt in debts)
                {
                    double proportionPercent = debt.Amount / debtsTotal;
                    double proportion = remainder * proportionPercent;
                    debt.Amount += proportion;
                }
            }
            else
            {
                double remainingSplit = remainder / debts.Count;
                debts.ForEach(x => x.Amount += remainingSplit);
            }

            return debts;
        }
    }
}
