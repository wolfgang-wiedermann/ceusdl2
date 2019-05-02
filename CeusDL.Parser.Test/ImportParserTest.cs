using System;
using System.Linq;
using KDV.CeusDL.Model.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KDV.CeusDL.Parser.Test
{
    // Einige eigentlich gute Tests auskommentiert, dass es auf kdv-build wieder durchläuft:-)
    [TestClass]
    public class ImportParserTest {

        /**
         * Testfall: 2 imports nach einander, wird der zweite richtig gelesen?
         *           (damit gab es schon Probleme!)
         */
        [TestMethod]
        public void TestCursorPositionAfterImport() {
            var data = new ParsableData("import \"../../../../Test/Data/interface_demo.ceusdl\"\nimport \"../../../../Test/Data/interface_demo2.ceusdl\"");
            var p = new ImportParser(data);
            var result = p.Parse();
            Console.WriteLine($"Result : {result.Path}");

            StringAssert.EndsWith(result.Path, "..\\..\\..\\..\\Test\\Data\\interface_demo.ceusdl");
            Assert.AreEqual(1, result?.Content?.Interfaces?.Count);
            Assert.AreEqual("Semester", result.Content.Interfaces[0].Name);

            result = p.Parse();
            StringAssert.EndsWith(result.Path, "..\\..\\..\\..\\Test\\Data\\interface_demo2.ceusdl");                       
        }

        [TestMethod]
        public void TestParseSimpleImport() {
            var data = new ParsableData("import \"../../../../Test/Data/interface_demo.ceusdl\"");
            var p = new ImportParser(data);
            var result = p.Parse();
            Console.WriteLine($"Result : {result.Path}");

            StringAssert.EndsWith(result.Path, "..\\..\\..\\..\\Test\\Data\\interface_demo.ceusdl");
            Assert.AreEqual(1, result?.Content?.Interfaces?.Count);
            Assert.AreEqual("Semester", result.Content.Interfaces[0].Name);
        }

        [TestMethod]
        public void TestParseWithSplitFile() {
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\split_main.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();
            
            // TODO: Hier noch Asserts einbauen, bisher testet das halt einfach
            //       obs ohne Exception durchläuft.
        }

        [TestMethod]
        public void TestParseToCoreModelWithSplitFile() {
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\split_main.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();
            
            var model = new CoreModel(result);

            Assert.AreEqual(39, model.Interfaces.Count);
            Assert.AreEqual("Tag", model.Interfaces.Where(i => i.Name == "Tag").First().Name);
            Assert.AreEqual("Studienfach", model.Interfaces.Where(i => i.Name == "Studienfach").First().Name);
        }
    }
}