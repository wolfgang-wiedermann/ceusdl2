using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;
using KDV.CeusDL.Model.Exceptions;

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

        [TestMethod]
        public void TestBTModel_WithMandantFile()
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
            Assert.AreEqual("FH_AP_BaseLayer", btModel.Config.BLDatabase);
            Assert.AreEqual("FH_AP_BaseLayer", btModel.Config.BTDatabase);

            var btIfa = btModel.Interfaces[0];
            Assert.AreEqual("Land", btIfa.ShortName);            
            Assert.AreEqual("AP_BT_D_Land", btIfa.Name);            
            Assert.AreEqual("FH_AP_BaseLayer.dbo.AP_BT_D_Land", btIfa.FullName);            
        }

        [TestMethod]
        [ExpectedException(typeof(MissingFinestTimeAttributeException))]
        public void TestBTModel_WithMandantAndHistoryFile()
        {
            // Daten einlesen aus Datei ohne Config-Section
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.5.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);
            var blModel = new BL.BLModel(model);
            var btModel = new BTModel(blModel);

            Assert.AreEqual(3, btModel.Interfaces.Count);
            Assert.AreEqual("AP", btModel.Config.Prefix);
            Assert.AreEqual("FH_AP_BaseLayer", btModel.Config.BLDatabase);
            Assert.AreEqual("FH_AP_BaseLayer", btModel.Config.BTDatabase);

            var btIfa = btModel.Interfaces[0];
            Assert.AreEqual("Land", btIfa.ShortName);            
            Assert.AreEqual("AP_BT_D_Land", btIfa.Name);            
            Assert.AreEqual("FH_AP_BaseLayer.dbo.AP_BT_D_Land", btIfa.FullName);            
        }

        [TestMethod]        
        public void TestBTModel_WithMandantAndHistoryFile2()
        {
            // Daten einlesen aus Datei ohne Config-Section
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\interface_demo.6.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);
            var blModel = new BL.BLModel(model);
            var btModel = new BTModel(blModel);

            Assert.AreEqual(4, btModel.Interfaces.Count);
            Assert.AreEqual("AP", btModel.Config.Prefix);
            Assert.AreEqual("FH_AP_BaseLayer", btModel.Config.BLDatabase);
            Assert.AreEqual("FH_AP_BaseLayer", btModel.Config.BTDatabase);

            var btIfa = btModel.Interfaces[0];
            Assert.AreEqual("Land", btIfa.ShortName);            
            Assert.AreEqual("AP_BT_D_Land", btIfa.Name);            
            Assert.AreEqual("FH_AP_BaseLayer.dbo.AP_BT_D_Land", btIfa.FullName);

            var btIfaV = btModel.Interfaces[1];
            Assert.AreEqual("AP_BT_D_Land_VERSION", btIfaV.Name);

            var identity = (BaseBTAttribute)btIfaV.Attributes[0];
            Assert.AreEqual("Land_VERSION_ID", identity.Name);
            Assert.IsTrue(identity.IsIdentity);

            var reference = (RefBTAttribute)btIfaV.Attributes[2];
            Assert.AreEqual("Land_ID", reference.IdAttribute.Name);
            Assert.AreEqual("Land_KNZ", reference.KnzAttribute.Name);
            Assert.AreEqual("AP_BL_D_Land", reference.ReferencedBLInterface.Name);
            Assert.AreEqual("AP_BT_D_Land", reference.ReferencedBTInterface.Name);
            Assert.AreEqual("Land_ID", ((BaseBTAttribute)(reference.ReferencedBTAttribute)).Name);
        }
    }
}