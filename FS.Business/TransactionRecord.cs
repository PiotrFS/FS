using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FS.Business
{
    /// <summary>
    /// Represents data for a single transaction
    /// </summary>
    public class TransactionRecord
    {
        /// <summary>
        /// Reg ex used to split CSV transaction record
        /// </summary>
        private static readonly Regex CsvSplit = new Regex("(?:^|,)(\"(?:[^\"])*\"|[^,]*)", RegexOptions.Compiled);
        public enum Columns
        {
            Date = 0,
            Type = 1,
            Shares = 2,
            Price = 3,
            Fund = 4,
            Investor = 5,
            SalesRep = 6
        }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public double Shares { get; set; }
        public decimal Price { get; set; }
        public string Fund { get; set; }
        public string Investor { get; set; }
        public string SalesRep { get; set; }
        public decimal Cost
        {
            get
            {
                if (IsBuy)
                    return (decimal)(-1 * Shares * (double) Price);
                
                return (decimal)(Shares * (double)Price);
            }
        }

        public bool IsBuy => Type.Equals("BUY");

        public TransactionRecord(string data)
        {
            var cols = SplitCsv(data);
            Date = DateTime.Parse(cols[(int) Columns.Date]);

            Type = cols[(int) Columns.Type];
            if(Type != "BUY" && Type != "SELL") throw new Exception($"Transaction type is not BUY or SELL.  Its value is: {Type}.");

            Shares = double.Parse(cols[(int) Columns.Shares]);

            // remove dollar sign and trim whitespace
            Price = decimal.Parse(cols[(int) Columns.Price].Replace("$", "").Trim());

            Fund = cols[(int) Columns.Fund];

            // remove leading and trailing double quote
            Investor = cols[(int) Columns.Investor].TrimStart('"').TrimEnd('"');
            SalesRep = cols[(int) Columns.SalesRep].TrimStart('"').TrimEnd('"');
        }

        /// <summary>
        /// Splits CSV transaction record into individual data cells
        /// </summary>
        /// <param name="transactionRecord">Transaction record</param>
        /// <returns></returns>
        private static string[] SplitCsv(string transactionRecord)
        {
            // replace escaped double quotes with something very rare so they are preserved and reg ex doesn't mess it up
            transactionRecord = transactionRecord.Replace("\"\"", "||||");
            var list = new List<string>();
            string curr = null;
            foreach (Match match in CsvSplit.Matches(transactionRecord))
            {
                curr = match.Value;
                if (0 == curr.Length)
                {
                    list.Add("");
                }
                // restore the double quotes inside of a 'cell'
                list.Add(curr.TrimStart(',').Replace("||||", "\""));
            }

            return list.ToArray();
        }
    }
}