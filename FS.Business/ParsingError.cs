using System;

namespace FS.Business
{
    /// <summary>
    /// Holds information about any parsing errors that might have occurred during report generation
    /// </summary>
    public class ParsingError
    {
        public string TransactionLine { get; set; }
        public Exception ExceptionThrown { get; set; }
    }
}