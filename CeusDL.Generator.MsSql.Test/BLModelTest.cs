using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;

namespace KDV.CeusDL.Model.BL.Test
{
    [TestClass]
    public class BLModelTest
    {
        [TestMethod]
        public void TestBLModel_BySimpleFile()
        {
            // Daten einlesen...
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);

            var blModel = new BLModel(model);
            var blIfa = blModel.Interfaces[0];

            // Properties des 0. Interfaces prüfen...
            Assert.IsFalse(blIfa.IsHistorized);
            Assert.IsNull(blIfa.HistoryAttribute);
            
            IBLAttribute attr = blIfa.Attributes[0];

            // Inhalt des 0. Attributs überprüfen
            Assert.AreEqual("Semester_ID", attr.Name);
            Assert.AreEqual("def_Semester.Semester_ID", attr.FullName);
            Assert.AreEqual(CoreDataType.INT, attr.DataType);
            Assert.AreEqual(0, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.IsFalse(attr.IsPartOfUniqueKey);
            Assert.IsTrue(attr.IsIdentity);
            Assert.IsTrue(attr.IsPrimaryKey);

            attr = blIfa.Attributes[1];

            // Inhalt des 1. Attributs überprüfen
            Assert.AreEqual("Semester_KNZ", attr.Name);
            Assert.AreEqual("def_Semester.Semester_KNZ", attr.FullName);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.AreEqual(50, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.IsTrue(attr.IsPartOfUniqueKey);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);

            attr = blIfa.Attributes[2];

            // Inhalt des 2. Attributs überprüfen
            Assert.AreEqual("Semester_KURZBEZ", attr.Name);
            Assert.AreEqual("def_Semester.Semester_KURZBEZ", attr.FullName);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.AreEqual(100, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.IsFalse(attr.IsPartOfUniqueKey);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);        
        }

        [TestMethod]
        public void TestBLModel_ByComplexFile() {
            // Daten einlesen...
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\file_demo2.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);

            var blModel = new BLModel(model);

            Assert.AreEqual(34, blModel.Interfaces.Count);
        }
    }
}