using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MyKPlanQuickenConverter.Model;

namespace MyKPlanQuickenConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Transaction[] transactions = null;

            using (FileStream readStream = new FileStream(@"C:\Users\Justin Terada\Desktop\AccountHistory.htm", FileMode.Open))
            {
                var parser = new MyKPlanParser();

                transactions = parser.GetTransactionsFromAccountHistory(readStream);
            }

            using (FileStream writeStream = new FileStream(@"C:\Users\Justin Terada\Desktop\Output.qif", FileMode.Create))
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

                var writer = new QifWriter("kCura 401(k)", "Invst", transactionNameReplacements);

                writer.WriteToFile(transactions, writeStream);
            }
        }
    }
}
