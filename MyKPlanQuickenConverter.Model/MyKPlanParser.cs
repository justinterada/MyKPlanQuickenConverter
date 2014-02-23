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
    public class MyKPlanParser
    {
        public Transaction[] GetTransactionsFromAccountHistory(Stream stream)
        {
            List<Transaction> resultTransactions = new List<Transaction>();
            var root = XElement.Load(stream);

            // Pullled from http://stackoverflow.com/questions/2524804/how-do-i-use-xpath-with-a-default-namespace-with-no-prefix
            var nameTable = new NameTable();
            var nsMgr = new XmlNamespaceManager(nameTable);
            nsMgr.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");

            var transactionTable = root.XPathSelectElement("xhtml:body/xhtml:table/xhtml:tbody/xhtml:tr/xhtml:td/xhtml:table[4]", nsMgr);

            var transactions = transactionTable.XPathSelectElements("//xhtml:tr[@valign='bottom']", nsMgr);

            string currentFundName = null;

            foreach (XElement t in transactions)
            {
                var cells = t.XPathSelectElements("xhtml:td", nsMgr);

                if (cells.Count() > 1)
                {
                    if (cells.ElementAt(1).Value == "Investment Fund")
                    {
                        continue;
                    }
                    else if (cells.ElementAt(7).Value == "Beginning Balance")
                    {
                        currentFundName = cells.ElementAt(1).Value;
                    }
                    else
                    {
                        TransactionType tType = TransactionType.Buy;

                        if (cells.ElementAt(7).Value.Contains("Dividends and Earnings"))
                        {
                            tType = TransactionType.ReinvestDividend;
                        }
                        else if (cells.ElementAt(7).Value.Contains("Contribution"))
                        {
                            tType = TransactionType.Buy;
                        }
                        else
                        {
                            continue;
                        }

                        resultTransactions.Add(new Transaction()
                        {
                            FundName = currentFundName,
                            Price = double.Parse(cells.ElementAt(9).Value, System.Globalization.NumberStyles.Any),
                            Amount = double.Parse(cells.ElementAt(8).Value),
                            Total = double.Parse(cells.ElementAt(10).Value, System.Globalization.NumberStyles.Any),
                            Date = DateTime.Parse(cells.ElementAt(3).Value),
                            Type = tType
                        });
                    }
                }
            }

            return resultTransactions.ToArray();
        }
    }
}
