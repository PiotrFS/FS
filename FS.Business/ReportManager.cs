using System;
using System.Collections.Generic;
using System.IO;
using FS.Business.Reports;

namespace FS.Business
{
    /// <summary>
    /// Responsible for generation of reports
    /// </summary>
    public class ReportManager
    {
        private readonly TransactionDataLoader _transactionDataLoader = new TransactionDataLoader();
        public List<ReportBase> Reports { get; set; }
        public DateTime ReportToDate { get; set; }
        public List<ParsingError> Errors { get; set; }
        public ReportManager()
        {
            
        }
        public ReportManager(DateTime reportToDate, Stream data)
        {
            ReportToDate = reportToDate;
            Reports = new List<ReportBase>
            {
                new AssetsSummaryReport(),
                new BreakReport(),
                new InvestorProfitReport(),
                new SalesSummaryReport(ReportToDate)
            };

            Errors = _transactionDataLoader.LoadData(data);
        }

        public void GenerateReports()
        {
            foreach (var report in Reports)
            {
                report.Generate(_transactionDataLoader.Data);
            }
        }
    }
}