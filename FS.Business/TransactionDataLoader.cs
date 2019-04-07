using System;
using System.Collections.Generic;
using System.IO;

namespace FS.Business
{
    /// <summary>
    /// Responsible for loading transaction data and parsing to ensure proper format and valid data
    /// </summary>
    public class TransactionDataLoader
    {
        public List<TxnRecord> Data { get; set; }
        public List<ParsingError> LoadData(Stream data)
        {
            var errors = new List<ParsingError>();
            using (var sr = new StreamReader(data))
            {
                var content = sr.ReadToEnd();
                var stringSeparators = new[] { "\r\n" };
                var lines = content.Split(stringSeparators, StringSplitOptions.None);

                if (lines.Length <= 1) return errors;

                Data = new List<TxnRecord>();
                // first line contains header so skip it and start with line 2
                for (var i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        Data.Add(new TxnRecord(lines[i]));
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new ParsingError
                        {
                            ExceptionThrown = ex,
                            TransactionLine = lines[i]
                        });
                    }
                }
            }

            return errors;
        }
    }
}
