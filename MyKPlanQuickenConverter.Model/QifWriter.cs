using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MyKPlanQuickenConverter.Model
{
    public class QifWriter
    {
        private string _accountName;
        private string _accountType;
        private IDictionary<string, string> _fundNameReplacements;

        public QifWriter(string accountName, string accountType, IDictionary<string, string> fundNameReplacements)
        {
            _accountName = accountName;
            _accountType = accountType;
            _fundNameReplacements = fundNameReplacements ?? new Dictionary<string, string>();
        }

        public void WriteToFile(Transaction[] transactions, Stream outputStream)
        {
            using(StreamWriter writer = new StreamWriter(outputStream))
            {
                writer.WriteLine("!Account");
                writer.WriteLine(string.Format("N{0}", _accountName));
                writer.WriteLine(string.Format("T{0}", _accountType));
                writer.WriteLine("^");
                writer.WriteLine(string.Format("!Type:{0}", _accountType));
                writer.WriteLine("^");

                foreach (Transaction t in transactions)
                {
                    writer.WriteLine(string.Format("D{0:M/d\\'yy}", t.Date));

                    if(t.Type == TransactionType.Buy)
                    {
                        writer.WriteLine("NBuy");
                    }
                    else if (t.Type == TransactionType.ReinvestDividend)
                    {
                        writer.WriteLine("NReinvDiv");
                    }

                    writer.WriteLine(string.Format("Y{0}", _fundNameReplacements.ContainsKey(t.FundName) ? _fundNameReplacements[t.FundName] : t.FundName));
                    writer.WriteLine(string.Format("I{0}", t.Price));
                    writer.WriteLine(string.Format("Q{0}", t.Amount));
                    writer.WriteLine(string.Format("U{0}", t.Total));
                    writer.WriteLine(string.Format("T{0}", t.Total));
                    writer.WriteLine("^");
                }
            }
        }
    }
}
