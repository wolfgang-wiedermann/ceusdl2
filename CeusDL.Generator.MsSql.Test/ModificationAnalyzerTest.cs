using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data.Common;
using System.Data.SqlClient;

using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;

namespace KDV.CeusDL.Utilities.BL.Test
{
    [TestClass]
    public class ModificationAnalyzerTest
    {
        [TestMethod]
        public void TestTableExists() {            
            using(var con = new SqlConnection("Data Source=localhost;Initial Catalog=FH_AP_BaseLayer;Integrated Security=True;Application Name=\"CEUSDL Tests\"")) {
                con.Open();
                var ana = new ModificationAnalyzer(null, con);
                var result = ana.TableExists("AP_def_Semester");
                Assert.IsTrue(result);

                result = ana.TableExists("ABC_depp_Semesta");
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void TestColumnExists_Simple() {            
            using(var con = new SqlConnection("Data Source=localhost;Initial Catalog=FH_AP_BaseLayer;Integrated Security=True;Application Name=\"CEUSDL Tests\"")) {
                con.Open();
                var ana = new ModificationAnalyzer(null, con);
                var result = ana.ColumnExists("AP_def_Semester", "Semester_KNZ");
                Assert.IsTrue(result);

                result = ana.ColumnExists("AP_def_Semester", "Sem_KNZ");
                Assert.IsFalse(result);
            }
        }
    }
}