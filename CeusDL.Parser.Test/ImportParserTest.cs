using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KDV.CeusDL.Parser.Test
{
    [TestClass]
    public class ImportParserTest {
        [TestMethod]
        public void TestParseSimpleImport() {
            var data = new ParsableData("import \"../../../../Test/Data/interface_demo.ceusdl\"");
            var p = new ImportParser(data);
            var result = p.Parse();
            Console.WriteLine($"Result : {result.Path}");

            Assert.AreEqual("..\\..\\..\\..\\Test\\Data\\interface_demo.ceusdl", result.Path);
            Assert.AreEqual(1, result?.Content?.Interfaces?.Count);
            Assert.AreEqual("Semester", result.Content.Interfaces[0].Name);
        }
    }
}