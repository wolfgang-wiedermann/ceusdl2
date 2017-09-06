using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL.Test
{
    [TestClass]
    public class CustomBLAttributeTest
    {
        ///
        /// Ziel im Create-Statement:
        /// Mandant_KNZ varchar(10) not null
        ///
        [TestMethod]
        public void TestCustomBLAttribute_Mandant_Sample1()
        {
            IBLAttribute attr = new CustomBLAttribute() {
                Name = "Mandant_KNZ",
                FullName = "Unknown.Mandant_KNZ",
                DataType = CoreDataType.VARCHAR,
                Length = 10,
                IsNotNull = true,
                ParentInterface = null
            };

            Assert.AreEqual("varchar(10) not null", attr.GetSqlDataTypeDefinition());
            Assert.AreEqual("Mandant_KNZ", attr.Name);
            Assert.AreEqual(10, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.IsNull(attr.ParentInterface);
        }

        ///
        /// Ziel im Create-Statement:
        /// Mandant_KNZ varchar(10) not null
        ///
        [TestMethod]
        public void TestCustomBLAttribute_Mandant_Sample2()
        {
            IBLAttribute attr = CustomBLAttribute.GetNewMandantAttribute(null);

            Assert.AreEqual("varchar(10) not null", attr.GetSqlDataTypeDefinition());
            Assert.AreEqual("Mandant_KNZ", attr.Name);
            Assert.AreEqual(10, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.IsNull(attr.ParentInterface);
            Assert.IsTrue(attr.IsPartOfUniqueKey);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);            
        }

        ///
        /// Ziel im Create-Statement:
        /// Semester_ID int primary key identity not null
        ///
        [TestMethod]
        public void TestCustomBLAttribute_TableID_Sample1()
        {
            IBLInterface parent = new DefaultBLInterface() {
                ShortName = "Semester"
            };

            IBLAttribute attr = CustomBLAttribute.GetNewIDAttribute(parent);

            Assert.AreEqual("Semester_ID", attr.Name);
            Assert.AreEqual(0, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.INT, attr.DataType);
            Assert.AreEqual(parent, attr.ParentInterface);
            Assert.IsTrue(attr.IsIdentity);
            Assert.IsTrue(attr.IsPrimaryKey);
            Assert.AreEqual("int primary key identity not null", attr.GetSqlDataTypeDefinition());
        }

        ///
        /// Ziel im Create-Statement:
        /// T_Modifikation varchar(10) not null
        ///
        [TestMethod]
        public void TestCustomBLAttribute_TModifikation()
        {
            IBLInterface parent = new DefaultBLInterface() {
                ShortName = "Semester"
            };

            IBLAttribute attr = CustomBLAttribute.GetNewTModifikationAttribute(parent);

            Assert.AreEqual("T_Modifikation", attr.Name);
            StringAssert.EndsWith(attr.FullName, attr.Name);
            Assert.AreEqual(10, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.AreEqual(parent, attr.ParentInterface);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);
            Assert.IsFalse(attr.IsPartOfUniqueKey);
            Assert.AreEqual("varchar(10) not null", attr.GetSqlDataTypeDefinition());
        }

        ///
        /// Ziel im Create-Statement:
        /// T_Bemerkung varchar(10)
        ///
        [TestMethod]
        public void TestCustomBLAttribute_TBemerkung()
        {
            IBLInterface parent = new DefaultBLInterface() {
                ShortName = "Semester"
            };

            IBLAttribute attr = CustomBLAttribute.GetNewTBemerkungAttribute(parent);

            Assert.AreEqual("T_Bemerkung", attr.Name);
            StringAssert.EndsWith(attr.FullName, attr.Name);
            Assert.AreEqual(100, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.AreEqual(parent, attr.ParentInterface);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);
            Assert.IsFalse(attr.IsPartOfUniqueKey);
            Assert.AreEqual("varchar(100)", attr.GetSqlDataTypeDefinition());
        }

        ///
        /// Ziel im Create-Statement:
        /// T_Benutzer varchar(100) not null
        ///
        [TestMethod]
        public void TestCustomBLAttribute_TBenutzer()
        {
            IBLInterface parent = new DefaultBLInterface() {
                ShortName = "Semester"
            };

            IBLAttribute attr = CustomBLAttribute.GetNewTBenutzerAttribute(parent);

            Assert.AreEqual("T_Benutzer", attr.Name);
            StringAssert.EndsWith(attr.FullName, attr.Name);
            Assert.AreEqual(100, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.AreEqual(parent, attr.ParentInterface);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);
            Assert.IsFalse(attr.IsPartOfUniqueKey);
            Assert.AreEqual("varchar(100) not null", attr.GetSqlDataTypeDefinition());
        }

        ///
        /// Ziel im Create-Statement:
        /// T_System varchar(10) not null
        ///
        [TestMethod]
        public void TestCustomBLAttribute_TSystem()
        {
            IBLInterface parent = new DefaultBLInterface() {
                ShortName = "Semester"
            };

            IBLAttribute attr = CustomBLAttribute.GetNewTSystemAttribute(parent);

            Assert.AreEqual("T_System", attr.Name);
            StringAssert.EndsWith(attr.FullName, attr.Name);
            Assert.AreEqual(10, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.VARCHAR, attr.DataType);
            Assert.AreEqual(parent, attr.ParentInterface);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);
            Assert.IsFalse(attr.IsPartOfUniqueKey);
            Assert.AreEqual("varchar(10) not null", attr.GetSqlDataTypeDefinition());
        }        

        ///
        /// Ziel im Create-Statement:
        /// T_Erst_DAT datetime not null
        ///
        [TestMethod]
        public void TestCustomBLAttribute_TErstDat()
        {
            IBLInterface parent = new DefaultBLInterface() {
                ShortName = "Semester"
            };

            IBLAttribute attr = CustomBLAttribute.GetNewTErstDatAttribute(parent);

            Assert.AreEqual("T_Erst_Dat", attr.Name);
            StringAssert.EndsWith(attr.FullName, attr.Name);
            Assert.AreEqual(0, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.DATETIME, attr.DataType);
            Assert.AreEqual(parent, attr.ParentInterface);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);
            Assert.AreEqual("datetime not null", attr.GetSqlDataTypeDefinition());
        }

        ///
        /// Ziel im Create-Statement:
        /// T_Aend_DAT datetime not null
        ///
        [TestMethod]
        public void TestCustomBLAttribute_TAendDat()
        {
            IBLInterface parent = new DefaultBLInterface() {
                ShortName = "Semester"
            };

            IBLAttribute attr = CustomBLAttribute.GetNewTAendDatAttribute(parent);

            Assert.AreEqual("T_Aend_Dat", attr.Name);
            StringAssert.EndsWith(attr.FullName, attr.Name);
            Assert.AreEqual(0, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.DATETIME, attr.DataType);
            Assert.AreEqual(parent, attr.ParentInterface);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);
            Assert.AreEqual("datetime not null", attr.GetSqlDataTypeDefinition());
        }

        ///
        /// Ziel im Create-Statement:
        /// T_Ladelauf_NR int not null
        ///
        [TestMethod]
        public void TestCustomBLAttribute_TLadelaufNR()
        {
            IBLInterface parent = new DefaultBLInterface() {
                ShortName = "Semester"
            };

            IBLAttribute attr = CustomBLAttribute.GetNewTLadelaufNRAttribute(parent);

            Assert.AreEqual("T_Ladelauf_NR", attr.Name);
            StringAssert.EndsWith(attr.FullName, attr.Name);
            Assert.AreEqual(0, attr.Length);
            Assert.AreEqual(0, attr.Decimals);
            Assert.AreEqual(CoreDataType.INT, attr.DataType);
            Assert.AreEqual(parent, attr.ParentInterface);
            Assert.IsFalse(attr.IsIdentity);
            Assert.IsFalse(attr.IsPrimaryKey);
            Assert.IsFalse(attr.IsPartOfUniqueKey);
            Assert.AreEqual("int not null", attr.GetSqlDataTypeDefinition());
        }                      
    }
}