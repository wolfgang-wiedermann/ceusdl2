using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KDV.CeusDL.Parser;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Parser.Test {

    [TestClass]
    public class FileParserTest {

        [TestMethod]
        public void TestAttributeRecognition_Case1()
        {
            var data = new ParsableData(System.IO.File.ReadAllText(@"..\..\..\..\Test\Data\file_demo.ceusdl".Replace('\\', System.IO.Path.DirectorySeparatorChar)));
            var p = new FileParser(data);
            var result = p.Parse();

            CoreModel m = new CoreModel(result);

            if(!(m.Interfaces[1].Attributes[3] is CoreRefAttribute)) {
                Console.WriteLine("Fehler: Das 4. Attribut im Interface StudiengangHISinOne muss ein RefAttribut sein.");
            }
            
            Console.WriteLine("Und jetzt im Debugger testen!");
        }
    }
}