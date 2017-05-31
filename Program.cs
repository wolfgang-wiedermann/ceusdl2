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
            Console.WriteLine($"Result : {result.Name} = {result.Value}");

            var data2 = new ParsableData("//LineComment\n\n   ");
            data2.Position = 2;
            var p2 = new CommentParser(data2);
            var result2 = p2.Parse();
            Console.WriteLine($"Result2: {result2}");
        }
    }
}
