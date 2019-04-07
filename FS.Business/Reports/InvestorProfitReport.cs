using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FS.Business.Reports
{
    public class InvestorProfitReport: ReportBase
    {
        private const string COL_INVESTOR = "Investor";
        private const string COL_FUND = "Fund";
        private const string COL_PROFIT_LOSS = "Net Profit or Loss";
        /// <inheritdoc />
        /// <summary>
        /// Generates Break Report based on the data provided.
        /// 
        /// For each Investor and Fund, return net profit or loss on investment.
        /// </summary>
        /// <param name="data">Transaction data</param>
        public override void Generate(List<TxnRecord> data)
        {
            Result = new DataTable(this.GetType().Name);
            Result.Columns.Add(new DataColumn(COL_INVESTOR));
            Result.Columns.Add(new DataColumn(COL_FUND));
            Result.Columns.Add(new DataColumn(COL_PROFIT_LOSS));

            var results = data.GroupBy(g => new {g.Investor, g.Fund})
                                .Select(d => (Investor: d.Key.Investor, Fund: d.Key.Fund, ProfitLoss: d.Sum(x => x.Cost)));

            foreach (var (investor, fund, profitLoss) in results)
            {
                var row = Result.NewRow();
                row[COL_INVESTOR] = investor;
                row[COL_FUND] = fund;
                row[COL_PROFIT_LOSS] = profitLoss.ToString("C");
                Result.Rows.Add(row);
            }
        }
    }
}