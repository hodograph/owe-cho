using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorApp.Shared
{
    public class Transaction
    {
        public Guid id { get; set; }

        public Guid TripId { get; set; }

        public string TransactionName { get; set; }

        public string Payer { get; set; }

        public double Total { get; set; }

        public List<Debt> Debts { get; set; } = new List<Debt>();

        public bool ProportionalSplit { get; set; }

        public List<Debt> CalculatePayments()
        {
            List<Debt> debts = new List<Debt>();

            foreach(Debt debt in Debts)
            {
                if(debts.Any(x => x.Debtor == debt.Debtor))
                {
                    debts.FirstOrDefault(x => x.Debtor == debt.Debtor).Amount += debt.Amount;
                }
                else
                {
                    debts.Add(new Debt() { Debtor = debt.Debtor, Amount = debt.Amount });
                }
            }

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

            debts.RemoveAll(x => Math.Round(x.Amount, 2) == 0);

            return debts;
        }
    }
}
