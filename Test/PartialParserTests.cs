using System;
using KDV.CeusDL.Parser;

namespace KDV.CeusDL.Test {
    public class PartialParserTest {
        public static void RunTests() {
            Console.WriteLine("Tests");            

            var test = new PartialParserTest();

            test.TestNextNonWhitespaceIs();
            test.TestNamedParameterParser();
            test.TestCommentParser();
            test.TestAttributeParser();  
            test.TestConfigParser();   
            test.TestInterfaceParser();              
        }

        public void TestNamedParameterParser() {
            var data = new ParsableData("text=\"Hallo Welt\\n----------\"");
            var p = new NamedParameterParser(data);
            var result = p.Parse();
            Console.WriteLine($"Result : {result.Name} = {result.Value}");

            data = new ParsableData("text = \"Hallo Welt\\n----------\"");
            p = new NamedParameterParser(data);
            result = p.Parse();
            Console.WriteLine($"Result : {result.Name} = {result.Value}");
        }

        public void TestCommentParser() {
            var data2 = new ParsableData("//LineComment\n\n   ");
            var p2 = new CommentParser(data2);
            var result2 = p2.Parse();
            Console.WriteLine($"Result2: {result2}");

            var data3 = new ParsableData("/*\n * ein einfacher Block Comment\n *\n*/\n   ");
            var p3 = new CommentParser(data3);
            var result3 = p3.Parse();
            Console.WriteLine($"Result3: {result3}");
        }

        public void TestAttributeParser() {
             var data4 = new ParsableData("base KNZ:varchar(len=\"50\", primary_key=\"true\");");
            var p4 = new AttributeParser(data4);
            var result4 = p4.Parse();
            Console.WriteLine($"Result4: {result4.AttributeType} {result4.Name} {result4.Parameters.Count}");

            data4 = new ParsableData("base KURZBEZ:varchar ( len = \"100\"  ) ;          ");
            p4 = new AttributeParser(data4);
            result4 = p4.Parse();
            Console.WriteLine($"Result4: {result4.AttributeType} {result4.Name} {result4.Parameters.Count}");

            data4 = new ParsableData("fact Anzahl_F:decimal(len=\"1,0\",\n\t default=\"1\"); // ein default=\"1\" wäre hier noch nett");
            p4 = new AttributeParser(data4);
            result4 = p4.Parse();
            Console.WriteLine($"Result4: {result4.AttributeType} {result4.Name} {result4.Parameters.Count}");

            data4 = new ParsableData("ref JaNein.KNZ as Zulassung; // Zulassung_JaNein");
            p4 = new AttributeParser(data4);
            result4 = p4.Parse();
            Console.WriteLine($"Result4: {result4.AttributeType} {result4.InterfaceName}.{result4.FieldName} {result4.Alias}");
        }

        public void TestConfigParser() {
            var data = new ParsableData("  { name=\"Wiedermann\";\n vorname=\"Wolfgang\";\n\n}");
            var p = new ConfigParser(data);
            var result = p.Parse();
            
            
            foreach(var v in result.Parameters) {
                Console.WriteLine($"Result5: {v.Name} -> {v.Value}");
            }
            Console.WriteLine("------");
            

            data = new ParsableData("  { // Beispieldatei\n name=\"Wiedermann\";\n vorname=\"Wolfgang\";\n\n}");
            p = new ConfigParser(data);
            result = p.Parse();
            
            foreach(var v in result.Parameters) {
                Console.WriteLine($"Result5: {v.Name} -> {v.Value}");
            }

            Console.WriteLine("------");
            

            data = new ParsableData("  { /* Beispieldatei */ name=\"Wiedermann\";\n \n /* Beispieldatei */\n vorname=\"Wolfgang\";\n\n}\n // Und noch ein Kommentar, der eigentlich ignoriert werden sollte...");
            p = new ConfigParser(data);
            result = p.Parse();
            
            foreach(var v in result.Parameters) {
                Console.WriteLine($"Result5: {v.Name} -> {v.Value}");
            }
        }

        public void TestNextNonWhitespaceIs() {
            var data = new ParsableData("     // Hallo Welt\n so gehts doch nicht");
            if(ParserUtil.NextNonWhitespaceIs(data, '/')) {
                Console.WriteLine("Passt");
            } else {
                Console.WriteLine("Passt NICHT !!!");
            }
        }

        public void TestInterfaceParser() {
            var data = new ParsableData(System.IO.File.ReadAllText(@"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.ceusdl"));
            var p = new InterfaceParser(data);
            var result = p.Parse();

            // TODO: Ausgabe

            data = new ParsableData(System.IO.File.ReadAllText(@"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo2.ceusdl"));
            p = new InterfaceParser(data);
            result = p.Parse();
        }
    }
}