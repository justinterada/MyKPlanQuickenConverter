using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyKPlanQuickenConverter.Model;
using System.IO;

namespace MyKPlanQuickenConverterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var transactionNameReplacements = new Dictionary<string, string>();
            transactionNameReplacements["JPMorgan Core Bond Fund - Class A"] = "JPMorgan Core Bond Fund";
            transactionNameReplacements["Oppenheimer International Bond Fund - Class A"] = "Oppenheimer International Bonds Fund";
            transactionNameReplacements["JPMorgan SmartRetirement 2040 Fund - Class A"] = "JPMorgan SmartRetirement 2040 Fund";
            transactionNameReplacements["Pioneer Cullen Value Fund - Class A"] = "Pioneer Cullen Value Fund";
            transactionNameReplacements["JPMorgan Mid Cap Value Fund - Class A"] = "JPMorgan Mid Cap Value Fund - Class A";
            transactionNameReplacements["SSgA Russell Small Cap Index Fund"] = "SSgA Small Cap Fund";
            transactionNameReplacements["Invesco International Growth Fund - Class A"] = "Invesco International Growth Fund";
            transactionNameReplacements["Oppenheimer Developing Markets Fund - Class A"] = "Oppenheimer Developing Markets Fund";

            transactionNameReplacements["VAN TOT ST"] = "Vanguard Total Stock Market Index Inv";
            transactionNameReplacements["VG MidCap"] = "Vanguard Mid-Cap Index Inv";
            transactionNameReplacements["VGSmCpRet"] = "Vanguard Small-Cap Value Index Inv";
            transactionNameReplacements["VGValueIn"] = "Vanguard Value Index Inv";
            transactionNameReplacements["Van EmgStk"] = "Vanguard Emerging Mkts Stock Index Inv";
            transactionNameReplacements["VanSmCpGrw"] = "Vanguard Small-Cap Growth Index Inv";
            transactionNameReplacements["Vangrd500"] = "Vanguard 500 Index Inv";
            transactionNameReplacements["VgTgRt2040"] = "Vanguard Target Retirement 2040 Inv";

            transactionNameReplacements["VanG ShTrm"] = "Vanguard Short-Term Bond Index Fund Investor Shares";

            string outputFile = "Output.qif";
            if (args.Length == 2)
            {
                outputFile = args[1];
            }

            ParseForAccount(args[0], outputFile, "kCura Vanguard 401(k)", new string[] { "EMPLOYEE 401(K)", "EMPLOYER MATCH" }, transactionNameReplacements);
            ParseForAccount(args[0], "roth-" + outputFile, "kCura Vanguard Roth 401(k)", new string[] { "EMPLOYEE ROTH 401(K) DEFERRAL" }, transactionNameReplacements);
        }

        private static void ParseForAccount(string inputFileName, string outputFileName, string accountName, string[] accountNamesToParse, Dictionary<string, string> transactionNameReplacements)
        {
            Transaction[] transactions = null;

            using (FileStream readStream = new FileStream(inputFileName, FileMode.Open))
            {
                var parser = new MyVanguardParser(accountNamesToParse);

                transactions = parser.GetTransactionsFromAccountHistory(readStream);
            }

            using (FileStream writeStream = new FileStream(outputFileName, FileMode.Create))
            {
                var writer = new QifWriter(accountName, "Invst", transactionNameReplacements);

                writer.WriteToFile(transactions, writeStream);
            }
        }
    }
}
