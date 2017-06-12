using System;
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
        }

        public void Test1(TmpParserResult result) {
            CoreModel m = new CoreModel(result);

            if(!(m.Interfaces[1].Attributes[3] is CoreRefAttribute)) {
                Console.WriteLine("Fehler: Das 4. Attribut im Interface StudiengangHISinOne muss ein RefAttribut sein.");
            }
            
            Console.WriteLine("Und jetzt im Debugger testen!");
        }
    }
}