using Microsoft.VisualStudio.TestTools.UnitTesting;
using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL.Test
{
    [TestClass]
    public class IBLAttributeTest
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
    }
}