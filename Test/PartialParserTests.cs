using System;
using KDV.CeusDL.Parser;

namespace KDV.CeusDL.Test {
    public class PartialParserTest {
        public static void RunTests() {
            Console.WriteLine("Tests");            
            var data = new ParsableData("text=\"Hallo Welt\\n----------\"");
            var p = new NamedParameterParser(data);
            var result = p.Parse();
            Console.WriteLine($"Result : {result.Name} = {result.Value}");

            data = new ParsableData("text = \"Hallo Welt\\n----------\"");
            p = new NamedParameterParser(data);
            result = p.Parse();
            Console.WriteLine($"Result : {result.Name} = {result.Value}");

            var data2 = new ParsableData("//LineComment\n\n   ");
            data2.Next();
            data2.Next();
            var p2 = new CommentParser(data2);
            var result2 = p2.Parse();
            Console.WriteLine($"Result2: {result2}");

            var data3 = new ParsableData("/*\n * ein einfacher Block Comment\n *\n*/\n   ");
            data3.Next();
            data3.Next();
            var p3 = new CommentParser(data3);
            var result3 = p3.Parse();
            Console.WriteLine($"Result3: {result3}");

            var data4 = new ParsableData("base KNZ:varchar(len=\"50\", primary_key=\"true\");");
            var p4 = new AttributeParser(data4);
            var result4 = p4.Parse();
            Console.WriteLine($"Result4: {result4.AttributeType} {result4.Name} {result4.Parameters.Count}");

            data4 = new ParsableData("base KURZBEZ:varchar ( len = \"100\"  ) ;          ");
            p4 = new AttributeParser(data4);
            result4 = p4.Parse();
            Console.WriteLine($"Result4: {result4.AttributeType} {result4.Name} {result4.Parameters.Count}");
        }
    }
}