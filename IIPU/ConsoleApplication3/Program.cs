using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WifiSH;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            var wifies = new WifiService();

            foreach (var network in wifies.GetWifiList())
            {
                Console.WriteLine("Name {0}", network.Name);
                Console.WriteLine("ConnectPercent {0}", network.ConnectPercent);
                Console.WriteLine("CypherType {0}", network.CypherType);
                Console.WriteLine("MacAddress {0}", network.MacAddress);
                Console.WriteLine("-----------------------------------");
            }

            Console.ReadKey();
        }
    }
}
