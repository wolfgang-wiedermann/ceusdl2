using System;
using System.Linq;
using KDV.CeusDL.Model;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Test {
    public class InterpreterTest {
        public static void RunTests() {
            var test = new InterpreterTest();

            var data = new ParsableData(System.IO.File.ReadAllText(@"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\file_demo.ceusdl"));
            var p = new FileParser(data);
            var result = p.Parse();

            test.Test1(result);
            test.TestWithBigCodeFile();
        }

        public void Test1(TmpParserResult result) {
            CoreModel m = new CoreModel(result);

            if(!(m.Interfaces[1].Attributes[3] is CoreRefAttribute)) {
                Console.WriteLine("Fehler: Das 4. Attribut im Interface StudiengangHISinOne muss ein RefAttribut sein.");
            }
            
            Console.WriteLine("Und jetzt im Debugger testen!");
        }

        public void TestWithBigCodeFile() {
            var data = new ParsableData(System.IO.File.ReadAllText(@"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\file_demo2.ceusdl"));
            var p = new FileParser(data);
            var result = p.Parse();
            CoreModel m = new CoreModel(result);            

            if(m.Interfaces.Count != result.Interfaces.Count) {
                throw new Exception("ERROR: Anzahl der Interfaces in Tmp und Core verschieden");
            }

            var b = m.Interfaces.Where(i => i.Name == "Bewerber").First();
            if(!b.IsMandant) {
                throw new Exception("ERROR: Das Interfaces Bewerber ist definitiv Mandanten-abhängig.");
            }
            if(!b.IsHistorized) {
                throw new Exception("ERROR: Das Interfaces Bewerber ist definitiv nach Tag.KNZ historisiert.");
            }
            if(!b.HistoryBy.ParentInterface.Name.Equals("Tag")) {
                throw new Exception("ERROR: Das Interfaces Bewerber ist definitiv nach Tag.KNZ historisiert.");
            }
            
            Console.WriteLine("Und jetzt im Debugger testen!");
        }
    }
}