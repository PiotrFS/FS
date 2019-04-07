using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FS.Business.Reports
{
    public class SalesSummaryReport : ReportBase
    {
        private readonly DateTime _reportToDate;
        private const string COL_SALES_REP = "Sales Rep";
        private const string COL_YEAR_TO_DATE = "Year to date";
        private const string COL_MONTH_TO_DATE = "Month to date";
        private const string COL_QUARTER_TO_DATE = "Quarter to date";
        private const string COL_INCEPTION_TO_DATE = "Inception to date";

        public SalesSummaryReport(DateTime reportToDate)
        {
            _reportToDate = reportToDate;
        }
        /// <inheritdoc />
        /// <summary>
        /// Generates Break Report based on the data provided.
        /// For each Sales Rep, generate Year to Date, Month to Date, Quarter to
        /// Date, and Inception to Date summary of cash amounts sold across all funds.
        /// </summary>
        /// <param name="data">Transaction data</param>
        public override void Generate(List<TxnRecord> data)
        {
            Result = new DataTable(this.GetType().Name);
            Result.Columns.Add(new DataColumn(COL_SALES_REP));
            Result.Columns.Add(new DataColumn(COL_YEAR_TO_DATE));
            Result.Columns.Add(new DataColumn(COL_MONTH_TO_DATE));
            Result.Columns.Add(new DataColumn(COL_QUARTER_TO_DATE));
            Result.Columns.Add(new DataColumn(COL_INCEPTION_TO_DATE));

            var startYearDate = new DateTime(_reportToDate.Year, 1, 1);
            var startMonthDate = new DateTime(_reportToDate.Year, _reportToDate.Month, 1);
            var startQuarterDate = GetQuarterStartDate(_reportToDate);

            var salesReps = data.Select(d => d.SalesRep).Distinct();
            
            foreach (var salesRep in salesReps)
            {
                var row = Result.NewRow();
                row[COL_SALES_REP] = salesRep;
                row[COL_YEAR_TO_DATE] = GetTotalSales(data, salesRep, startYearDate).ToString("C");
                row[COL_MONTH_TO_DATE] = GetTotalSales(data, salesRep, startMonthDate).ToString("C"); ;
                row[COL_QUARTER_TO_DATE] = GetTotalSales(data, salesRep, startQuarterDate).ToString("C"); ;
                row[COL_INCEPTION_TO_DATE] = GetTotalSales(data, salesRep, new DateTime()).ToString("C"); ;
                Result.Rows.Add(row);
            }
        }
        /// <summary>
        /// Gets total sale amount for a given sales rep and start date
        /// </summary>
        /// <param name="data">Transaction data</param>
        /// <param name="salesRep">Sales rep</param>
        /// <param name="startDate">Start date for transactions</param>
        /// <returns></returns>
        private decimal GetTotalSales(List<TxnRecord> data, string salesRep, DateTime startDate)
        {
            var toDateSales = data.FindAll(d =>
                d.SalesRep.Equals(salesRep)
                && d.Date >= startDate
                && d.Date <= _reportToDate
                && d.Type.Equals("SELL"));
            double total = 0;
            foreach (var sale in toDateSales)
            {
                total += sale.Shares * (double) sale.Price;
            }

            return (decimal) total;
        }
        /// <summary>
        /// Finds quarter start date based on date provided
        /// </summary>
        /// <param name="date">Date to figure out start of the quarter for</param>
        /// <returns></returns>
        private static DateTime GetQuarterStartDate(DateTime date)
        {
            var month = date.Month;
            if (month >= 1 && month <= 3) return new DateTime(date.Year, 1, 1);
            if (month >= 4 && month <= 6) return new DateTime(date.Year, 4, 1);
            if (month >= 7 && month <= 9) return new DateTime(date.Year, 7, 1);

            return new DateTime(date.Year, 10, 1);
        }
    }
}