using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FS.Business.Reports
{
    public class AssetsSummaryReport : ReportBase
    {
        private const string COL_SALES_REP = "Sales Rep";
        private const string COL_NET_AMOUNT = "Net Amount";
        /// <inheritdoc />
        /// <summary>
        /// Generates Asset Under Management Summary Report based on provided data
        ///
        /// For each Sales Rep, generate a summary of the net amount held by investors across all funds.
        /// </summary>
        /// <param name="data">Transaction data</param>
        public override void Generate(List<TxnRecord> data)
        {
            Result = new DataTable(this.GetType().Name);
            Result.Columns.Add(new DataColumn(COL_SALES_REP));
            Result.Columns.Add(new DataColumn(COL_NET_AMOUNT));

            var salesReps = data.Select(d => d.SalesRep).Distinct();

            foreach (var salesRep in salesReps)
            {
                var row = Result.NewRow();
                row[COL_SALES_REP] = salesRep;
                row[COL_NET_AMOUNT] = GetNetAmount(data, salesRep).ToString("C");
                Result.Rows.Add(row);
            }
        }
        /// <summary>
        /// Gets net amount of held by given sales rep using data provided
        /// </summary>
        /// <param name="data">Transaction data</param>
        /// <param name="salesRep">Sales rep</param>
        /// <returns></returns>
        private static decimal GetNetAmount(List<TxnRecord> data, string salesRep)
        {
            var transactions = data.FindAll(d => d.SalesRep.Equals(salesRep));
            double total = 0;
            foreach (var transaction in transactions)
            {
                if (transaction.IsBuy)
                    total += transaction.Shares * (double)transaction.Price;
                else
                    total -= transaction.Shares * (double) transaction.Price;
            }

            return (decimal)total;
        }
    }
}