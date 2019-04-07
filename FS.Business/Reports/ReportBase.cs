using System.Collections.Generic;
using System.Data;

namespace FS.Business.Reports
{
    public abstract class ReportBase
    {
        /// <summary>
        /// Data table to hold the results of the report
        /// </summary>
        public DataTable Result;
        /// <summary>
        /// Method that will generate report results based on the data provided
        /// </summary>
        /// <param name="data">Transaction data</param>
        public abstract void Generate(List<TxnRecord> data);
    }
}
