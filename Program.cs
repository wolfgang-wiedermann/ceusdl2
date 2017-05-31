using System;
using KDV.CeusDL.Parser;

namespace CeusDL2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tests");            
            var data = new ParsableData("\"Hallo Welt\\n----------\"");
            var p = new StringElementParser(data);
            Console.WriteLine($"Result: {p.Parse()}");
        }
    }
}
