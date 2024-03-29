using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;

namespace KDV.CeusDL.Model.BL.Test
{
    [TestClass]
    public class DefaultBLInterfaceTest
    {
        [TestMethod]
        public void TestDefaultBLInterface_FormerName() {
            var fileName = @"..\..\..\..\Test\Data\interface_demo.1.ceusdl".Replace('\\', System.IO.Path.DirectorySeparatorChar);
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);
            var blModel = new BLModel(model);

            // Auswählen
            var coreIfa = model.Interfaces[0];
            var coreAttr = coreIfa.Attributes[0];

            // in DefaultBLInterface konvertieren
            var blIfa = new DefaultBLInterface(coreIfa, blModel);                       
            Assert.AreEqual("AP_def_Semester", blIfa.Name);
            Assert.AreEqual("AP_def_Sem", blIfa.FormerName);
            Assert.AreEqual("FH_AP_BaseLayer.dbo.AP_def_Sem", blIfa.FullFormerName);

            var blIfa2 = new DefaultBLInterface(coreIfa, null);
            Assert.AreEqual("def_Semester", blIfa2.Name);
            Assert.AreEqual("def_Sem", blIfa2.FormerName);
            Assert.AreEqual("dbo.def_Sem", blIfa2.FullFormerName);
        }

        [TestMethod]
        public void TestDerivedBLInterface_FormerName() {
            var fileName = @"..\..\..\..\Test\Data\interface_demo2.1.ceusdl".Replace('\\', System.IO.Path.DirectorySeparatorChar);
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);
            var blModel = new BLModel(model);

            // Auswählen
            var coreIfa = model.Interfaces[0];
            var coreAttr = coreIfa.Attributes[0];

            // in DefaultBLInterface konvertieren
            var blIfa = new DefaultBLInterface(coreIfa, blModel);                       
            Assert.AreEqual("AP_BL_D_StudiengangHisInOne", blIfa.Name);
            Assert.AreEqual("AP_BL_D_Studiengang", blIfa.FormerName);
            Assert.AreEqual("FH_AP_BaseLayer.dbo.AP_BL_D_Studiengang", blIfa.FullFormerName);

            var blIfa2 = new DerivedBLInterface(blIfa, blModel);
            Assert.AreEqual("AP_BL_D_StudiengangHisInOne_VERSION", blIfa2.Name);
            Assert.AreEqual("AP_BL_D_Studiengang_VERSION", blIfa2.FormerName);
            Assert.AreEqual("FH_AP_BaseLayer.dbo.AP_BL_D_Studiengang_VERSION", blIfa2.FullFormerName);
        }


        [TestMethod]
        public void TestDefaultBLInterface_BySimpleFile()
        {
            // Daten einlesen...
            var fileName = @"..\..\..\..\Test\Data\interface_demo.ceusdl".Replace('\\', System.IO.Path.DirectorySeparatorChar);
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);

            // Auswählen
            var coreIfa = model.Interfaces[0];
            var coreAttr = coreIfa.Attributes[0];

            // in DefaultBLInterface konvertieren
            var blIfa = new DefaultBLInterface(coreIfa, null);            
            
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
        public void TestDefaultBLInterface_ILInterface()
        {
            // Daten einlesen...
            var fileName = @"..\..\..\..\Test\Data\interface_demo2.1.ceusdl".Replace('\\', System.IO.Path.DirectorySeparatorChar);
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);

            // Auswählen
            var coreIfa = model.Interfaces[0];
            var coreAttr = coreIfa.Attributes[0];

            // in DefaultBLInterface konvertieren
            var blIfa = new DefaultBLInterface(coreIfa, null);
            Assert.IsNotNull(blIfa.GetILInterface());
            Assert.AreEqual("AP_IL_StudiengangHisInOne", blIfa.GetILInterface().Name);
            
            IBLAttribute attr = blIfa.Attributes[0];

            // Inhalt des 0. Attributs überprüfen
            Assert.AreEqual("StudiengangHisInOne_ID", attr.Name);
            Assert.IsNull(attr.GetILAttribute());

            attr = blIfa.Attributes[1];

            // Inhalt des 1. Attributs überprüfen
            Assert.AreEqual("StudiengangHisInOne_KNZ", attr.Name);
            Assert.IsNotNull(attr.GetILAttribute());
            Assert.AreEqual("StudiengangHisInOne_KNZ", attr.GetILAttribute().Name);
            
            attr = blIfa.Attributes[2];

            // Inhalt des 2. Attributs überprüfen
            Assert.AreEqual("StudiengangHisInOne_KURZBEZ", attr.Name);
            Assert.IsNotNull(attr.GetILAttribute());            
            Assert.AreEqual("StudiengangHisInOne_KURZBEZ", attr.GetILAttribute().Name);
        }
    }
}