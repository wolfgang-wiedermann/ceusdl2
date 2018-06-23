using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;

namespace KDV.CeusDL.Model.BT.Test
{
    [TestClass]
    public class BTAttributeTest
    {
        [TestMethod]
        public void TestBTAttribute_RefAttributesNaming()
        {
            // Daten einlesen aus Datei ohne Config-Section
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.2.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);
            var blModel = new BL.BLModel(model);
            var btModel = new BTModel(blModel);

            Assert.AreEqual(3, btModel.Interfaces.Count);
            Assert.AreEqual("AP", btModel.Config.Prefix);            
            Assert.AreEqual("FH_AP_BaseLayer", btModel.Config.BTDatabase);

            var btIfa = btModel.Interfaces[0];            
            Assert.AreEqual("AP_BT_D_Land", btIfa.Name);

            var btAttr1 = btIfa.Attributes[6];
            Assert.IsTrue(btAttr1 is RefBTAttribute);
            var refAttr = (RefBTAttribute)btAttr1;
            Assert.AreEqual("Kontinent_KNZ", refAttr.KnzAttribute.Name);
            Assert.AreEqual("Kontinent_ID", refAttr.IdAttribute.Name);
            Assert.AreEqual(CoreDataType.VARCHAR, refAttr.KnzAttribute.DataType);
            Assert.AreEqual(CoreDataType.INT, refAttr.IdAttribute.DataType);
            Assert.AreEqual("varchar(50)", refAttr.KnzAttribute.SqlDataType);
            Assert.AreEqual("int", refAttr.IdAttribute.SqlDataType);

            var btAttr2 = btIfa.Attributes[7];
            Assert.IsTrue(btAttr2 is RefBTAttribute);
            refAttr = (RefBTAttribute)btAttr2;
            Assert.AreEqual("ExistiertSeit_Semester_KNZ", refAttr.KnzAttribute.Name);
            Assert.AreEqual("ExistiertSeit_Semester_ID", refAttr.IdAttribute.Name);
            Assert.AreEqual("Semester_KNZ", refAttr.KnzAttribute.ShortName);            
            Assert.AreEqual("Semester_ID", refAttr.IdAttribute.ShortName);
            Assert.AreEqual("ExistiertSeit", refAttr.KnzAttribute.Alias);            
            Assert.AreEqual("ExistiertSeit", refAttr.IdAttribute.Alias);
        }
    }
}