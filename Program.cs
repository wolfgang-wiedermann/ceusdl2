using System;
using KDV.CeusDL.Parser;

namespace CeusDL2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tests");            
            var data = new ParsableData("text=\"Hallo Welt\\n----------\"");
            var p = new NamedParameterParser(data);
            var result = p.Parse();
            Console.WriteLine($"Result: {result.Name} = {result.Value}");
        }
    }
}
