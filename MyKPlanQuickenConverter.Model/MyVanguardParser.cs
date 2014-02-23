using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

namespace MyKPlanQuickenConverter.Model
{
    public class MyVanguardParser
    {
        public Transaction[] GetTransactionsFromAccountHistory(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);

            // From here: http://stackoverflow.com/questions/12822680/xmldocument-failed-to-load-xhtml-string-because-of-error-reference-to-undeclare
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(reader.ReadToEnd());
            doc.OptionOutputAsXml = true;

            StringWriter writer = new StringWriter();
            doc.Save(writer);

            XDocument root = XDocument.Load(new StringReader(writer.ToString()));
            XPathNavigator navigator = root.CreateNavigator();

            List<Transaction> resultTransactions = new List<Transaction>();

            // Pullled from http://stackoverflow.com/questions/2524804/how-do-i-use-xpath-with-a-default-namespace-with-no-prefix
            var nameTable = new NameTable();
            var nsMgr = new XmlNamespaceManager(nameTable);
            nsMgr.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");

            var transactionTable = navigator.SelectSingleNode("span/body/table[1]/tr[4]/td/table[2]");

            var transactions = transactionTable.Select("tr");

            bool first = true;

            foreach (XPathNavigator t in transactions)
            {
                if (first)
                {
                    first = false;
                    continue;
                }

                var cellSelect = t.Select("td");
                var cells = new List<string>();

                cells.Add(cellSelect.Current.Value.Trim());

                while (cellSelect.MoveNext())
                {
                    cells.Add(cellSelect.Current.Value.Trim());
                }

                if (cells.Count == 8)
                {
                    TransactionType tType = TransactionType.Buy;

                    if (cells[3].Contains("Short-Term Earnings") || cells[3] == "Dividend")
                    {
                        tType = TransactionType.ReinvestDividend;
                    }
                    else if (cells[3] == "Contributions")
                    {
                        tType = TransactionType.Buy;
                    }
                    else
                    {
                        continue;
                    }

                    resultTransactions.Add(new Transaction()
                    {
                        FundName = cells[2],
                        Price = double.Parse(cells[5], System.Globalization.NumberStyles.Any),
                        Amount = double.Parse(cells[6]),
                        Total = double.Parse(cells[7], System.Globalization.NumberStyles.Any),
                        Date = DateTime.Parse(cells[1]),
                        Type = tType
                    });
                }
            }

            return resultTransactions.ToArray();
        }
    }
}
