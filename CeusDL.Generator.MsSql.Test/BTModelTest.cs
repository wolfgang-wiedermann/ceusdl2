using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;

namespace KDV.CeusDL.Model.BT.Test
{
    [TestClass]
    public class BTModelTest
    {
        [TestMethod]
        public void TestBTModel_BySimpleFileWithConfig()
        {
            // Daten einlesen aus Datei mit Config-Section
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.4.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);
            var blModel = new BL.BLModel(model);
            var btModel = new BTModel(blModel);

            Assert.AreEqual(1, btModel.Interfaces.Count);
            Assert.AreEqual("AP", btModel.Config.Prefix);
            Assert.AreEqual("FH_AP_BaseLayer", btModel.Config.BLDatabase);
            Assert.AreEqual("FH_AP_BaseLayer", btModel.Config.BTDatabase);

            var btIfa = btModel.Interfaces[0];
            Assert.AreEqual("Semester", btIfa.ShortName);            
            Assert.AreEqual("AP_BT_D_Semester", btIfa.Name);            
            Assert.AreEqual("FH_AP_BaseLayer.dbo.AP_BT_D_Semester", btIfa.FullName);
        }

        [TestMethod]
        public void TestBTModel_BySimpleFile()
        {
            // Daten einlesen aus Datei ohne Config-Section
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);
            var blModel = new BL.BLModel(model);
            var btModel = new BTModel(blModel);

            Assert.AreEqual(1, btModel.Interfaces.Count);
            Assert.AreEqual(null, btModel.Config.Prefix);
            Assert.AreEqual(null, btModel.Config.BLDatabase);
            Assert.AreEqual(null, btModel.Config.BTDatabase);

            var btIfa = btModel.Interfaces[0];
            Assert.AreEqual("Semester", btIfa.ShortName);            
            Assert.AreEqual("BT_D_Semester", btIfa.Name);            
            Assert.AreEqual("dbo.BT_D_Semester", btIfa.FullName);
        }        
    }
}