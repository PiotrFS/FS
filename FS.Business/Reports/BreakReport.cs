using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FS.Business.Reports
{
    public class BreakReport : ReportBase
    {
        private const string COL_INVESTOR = "Investor";
        private const string COL_CASH_BALANCE = "Cash balance";
        private const string COL_SHARE_BALANCE = "Share balance";
        /// <inheritdoc />
        /// <summary>
        /// Generates Break Report based on the data provided.
        /// 
        /// Assuming the information in the data provided is complete and accurate,
        /// generate a report that shows any errors(negative cash balances,
        /// negative share balance) by investor.
        /// </summary>
        /// <param name="data">Transaction data</param>
        public override void Generate(List<TransactionRecord> data)
        {
            Result = new DataTable(this.GetType().Name);
            Result.Columns.Add(new DataColumn(COL_INVESTOR));
            Result.Columns.Add(new DataColumn(COL_CASH_BALANCE));
            Result.Columns.Add(new DataColumn(COL_SHARE_BALANCE));

            var balances = new Dictionary<string, Balances>();

            foreach (var transaction in data)
            {
                var investor = transaction.Investor;

                if (balances.ContainsKey(investor))
                {
                    balances[investor].Cash += transaction.Cost;
                    balances[investor].Shares += transaction.IsBuy ? transaction.Shares : -1 * transaction.Shares;
                }
                else
                {
                    balances.Add(investor, new Balances
                    {
                        Cash = transaction.Cost,
                        Shares = transaction.IsBuy ? transaction.Shares : -1 * transaction.Shares
                    });
                }
            }
            
            // figure out which investor had negative cash or shares balance and include them in the report result
            var negativeBalances = balances.Where(b => b.Value.Cash < 0 || b.Value.Shares < 0);

            foreach (var negativeBalance in negativeBalances)
            {
                var row = Result.NewRow();
                row[COL_INVESTOR] = negativeBalance.Key;
                row[COL_CASH_BALANCE] = $"{negativeBalance.Value.Cash:C}";
                row[COL_SHARE_BALANCE] = negativeBalance.Value.Shares;
                Result.Rows.Add(row);
            }
        }
    }

    public class Balances
    {
        public decimal Cash { get; set; }
        public double Shares { get; set; }
    }
}