using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data.Common;
using System.Data.SqlClient;

using KDV.CeusDL.Generator;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;
using KDV.CeusDL.Model.BL;

namespace KDV.CeusDL.Utilities.BL.Test
{
    [TestClass]
    public class ModificationAnalyzerTest
    {
        [TestMethod]
        public void TestInterfaceRenamed_Complex() {
            var fileName = @"..\..\..\..\Test\Data\interface_demo.3.ceusdl";
            var data = new ParsableData(System.IO.File.ReadAllText(fileName), fileName);
            var p = new FileParser(data);
            var result = p.Parse();            
            var model = new CoreModel(result);
            var blModel = new BLModel(model);

            // Auswählen
            var ifa = blModel.Interfaces[0];

            using(var con = new SqlConnection("Data Source=localhost;Initial Catalog=FH_AP_BaseLayer;Integrated Security=True;Application Name=\"CEUSDL Tests\"")) {
                con.Open();
                var ana = new ModificationAnalyzer(blModel, con);
                bool tex = ana.InterfaceRenamed(ifa);
                Assert.IsTrue(tex);
            }
        }

        [TestMethod]
        public void TestTableWithNameExists_Simple() {            
            using(var con = new SqlConnection("Data Source=localhost;Initial Catalog=FH_AP_BaseLayer;Integrated Security=True;Application Name=\"CEUSDL Tests\"")) {
                con.Open();
                var ana = new ModificationAnalyzer(null, con);
                var result = ana.TableWithNameExists("AP_def_Semester");
                Assert.IsTrue(result);

                result = ana.TableWithNameExists("ABC_depp_Semesta");
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

        [TestMethod]
        public void TestTableRenamed_Simple() {
            using(var con = new SqlConnection("Data Source=localhost;Initial Catalog=FH_AP_BaseLayer;Integrated Security=True;Application Name=\"CEUSDL Tests\"")) {
                con.Open();
                
                var ana = new ModificationAnalyzer(null, con);
                var result = ana.TableRenamed("AP_def_Notenstufe", "AP_def_Notenstuf");
                Assert.IsTrue(result);
            }
        }
    }
}