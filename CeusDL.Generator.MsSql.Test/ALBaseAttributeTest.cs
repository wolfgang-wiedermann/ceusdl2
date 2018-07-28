using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;

namespace KDV.CeusDL.Model.AL.Test
{
    [TestClass]
    public class ALBaseAttributeTest
    {
        /*
        [TestMethod]
        public void TestALBaseAttribute_GetName()
        {
            // Daten einlesen...
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\arc2018.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);

            var bt = new BT.BTModel(model);
            var al = new BaseALAttribute(null, (BT.BaseBTAttribute)bt.Interfaces[2].Attributes.Where(a => a is BT.BaseBTAttribute).First());
            Assert.IsNull(al.Core);
            Assert.AreEqual("Students_ID", al.Name);
        }
        */

        [TestMethod]
        public void TestDimensionALInterface_GetName()
        {
            // Daten einlesen...
            var fileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\al_tests.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);

            var bt = new BT.BTModel(model);
            var alModel = new AL.ALModel(bt);
            var refBt = (BT.RefBTAttribute)bt.Interfaces[3].Attributes.Where(a => a is BT.RefBTAttribute).FirstOrDefault();
            var dim = new DimensionALInterface(alModel, refBt);
            Assert.IsNotNull(refBt);            
            Assert.AreEqual("ARC_D_Term_1_Term", dim.Name);
        }        
    }
}