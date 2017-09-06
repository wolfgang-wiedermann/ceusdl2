using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;

namespace KDV.CeusDL.Model.BL.Test
{
    [TestClass]
    public class BaseBLAttributeTest
    {
        [TestMethod]
        public void TestBaseBLAttribute_ByCoreBaseAttribute()
        {
            // Daten einlesen...
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);

            // Auswählen
            var coreIfa = model.Interfaces[0];
            var coreAttr = coreIfa.Attributes[0];
            var blIfa = new DefaultBLInterface() {
                Name = $"AP_def_{coreIfa.Name}"
            };

            // In BaseBLAttribute konvertieren
            BaseBLAttribute attr = new BaseBLAttribute((CoreBaseAttribute)coreAttr, blIfa);

            // Inhalt überprüfen
            Assert.AreEqual("Semester_KNZ", attr.Name);
            Assert.AreEqual("AP_def_Semester.Semester_KNZ", attr.FullName);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.AreEqual(50, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.IsTrue(attr.IsPartOfUniqueKey);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);            
        }
    }
}