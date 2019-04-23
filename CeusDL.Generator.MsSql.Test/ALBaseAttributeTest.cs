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
        [TestMethod]
        public void TestALBaseAttribute_GetName()
        {
            // Daten einlesen...
            var fileName = @"..\..\..\..\Test\Data\al_tests.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);

            var bt = new BT.BTModel(model);
            var alModel = new AL.ALModel(bt);

            var baseAttr = alModel.FactInterfaces[0].Attributes.Where(a => a is BaseALAttribute).FirstOrDefault();
            Assert.IsNotNull(baseAttr);            
            Assert.AreEqual("Students_ID", baseAttr.Name);

            baseAttr = alModel.DimensionInterfaces[0].Attributes.Where(a => a is BaseALAttribute).FirstOrDefault();
            Assert.IsNotNull(baseAttr);            
            Assert.AreEqual("Term_Primary_TermGroup_ID", baseAttr.Name);

            baseAttr = alModel.DimensionInterfaces[2].Attributes.Where(a => a is BaseALAttribute).FirstOrDefault();
            Assert.IsNotNull(baseAttr);            
            Assert.AreEqual("Term_Term_ID", baseAttr.Name);
        }        

        [TestMethod]
        public void TestDimensionALInterface_GetName()
        {
            // Daten einlesen...
            var fileName = @"..\..\..\..\Test\Data\al_tests.ceusdl";
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

        [TestMethod]
        public void TestDimensionALInterface_CheckReferences()
        {
            // Daten einlesen...
            var fileName = @"..\..\..\..\Test\Data\al_tests.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);

            var bt = new BT.BTModel(model);
            var alModel = new AL.ALModel(bt);
            var refAttr = alModel.FactInterfaces[0].Attributes.Where(a => a is RefALAttribute).Select(a => (RefALAttribute)a).FirstOrDefault();
            Assert.IsNotNull(refAttr);
            Assert.AreEqual("Term_Term_ID", refAttr.Name);
            
            var dim = refAttr.ReferencedDimension;
            Assert.IsNotNull(dim);
            Assert.AreEqual("ARC_D_Term_1_Term", dim.Name);
            Assert.AreEqual("ARC_D_Term_1_Term", dim.RootDimension.Name);
            Assert.AreEqual(5, dim.Attributes.Count);

            var childRef = dim.Attributes.Where(a => a is RefALAttribute).Select(a => (RefALAttribute)a).FirstOrDefault();
            Assert.IsNotNull(childRef);
            Assert.AreEqual("Term_Primary_TermGroup_ID", childRef.Name);
            Assert.IsNotNull(childRef.ReferencedDimension);
            var childDim = childRef.ReferencedDimension;
            Assert.AreEqual("ARC_D_Term_2_Primary_TermGroup", childDim.Name);
            Assert.AreEqual("ARC_D_Term_1_Term", childDim.RootDimension.Name);

            refAttr = alModel.FactInterfaces[0].Attributes.Where(a => a is RefALAttribute).Select(a => (RefALAttribute)a).LastOrDefault();
            Assert.IsNotNull(refAttr);
            Assert.AreEqual("Former_Gender_Former_Gender_ID", refAttr.Name);
        }  
    }
}