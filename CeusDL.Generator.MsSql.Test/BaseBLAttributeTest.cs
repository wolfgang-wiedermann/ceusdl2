using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

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
            var blIfa = new DefaultBLInterface(coreIfa, null);

            // In BaseBLAttribute konvertieren
            BaseBLAttribute attr = new BaseBLAttribute((CoreBaseAttribute)coreAttr, blIfa);

            // Inhalt überprüfen
            Assert.AreEqual("Semester_KNZ", attr.Name);
            Assert.AreEqual("def_Semester.Semester_KNZ", attr.FullName);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.AreEqual(50, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.IsTrue(attr.IsPartOfUniqueKey);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);            
        }

        [TestMethod]
        public void TestBaseBLAttribute_FormerName() 
        {
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.1.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var coreModel = new CoreModel(result);
            var blModel = new BLModel(coreModel);

            // Auswählen            
            var blIfa = (DefaultBLInterface)blModel.Interfaces[0];
            var blAttr = (BaseBLAttribute)blIfa.Attributes.Where(a => a.ShortName == "KURZBEZ").First();

            Assert.AreEqual("AP_def_Semester", blIfa.Name);
            Assert.AreEqual("Semester_KURZBEZ", blAttr.Name);
            Assert.AreEqual("Sem_KurzBezeichnung", blAttr.FormerName);
            Assert.AreEqual("AP_def_Sem.Sem_KurzBezeichnung", blAttr.FullFormerName);
        }

        [TestMethod]
        public void TestRefBLAttribute_FormerName()
        {
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.2.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var coreModel = new CoreModel(result);
            var blModel = new BLModel(coreModel);

            var ifaLand = (DefaultBLInterface)blModel.Interfaces[0];
            var attrKont = (RefBLAttribute)ifaLand.Attributes.Where(a => a.Name.Contains("Kontinent")).First();

            Assert.IsNotNull(attrKont);
            Assert.AreEqual("Kontinent_KNZ", attrKont.Name);
            Assert.AreEqual("Kont_Kontinent_KNZ", attrKont.FormerName);
            Assert.AreEqual("AP_BL_D_Kontinent.Kont_Kontinent_KNZ", attrKont.FullFormerName);

            // Das wird noch interessant, denn was ist, wenn Attributbezeichnung und Tabellenbezeichnung
            // zu unterschiedlichen Zeitpunkten geändert werden?
        }
    }
}