using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;
using KDV.CeusDL.Utilities.BL;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;

namespace KDV.CeusDL.Generator.BL {
    public class UpdateBLGenerator : IGenerator
    {
        private BLModel model;
        private ModificationAnalyzer analyzer;
        private CreateBLGenerator createGenerator;
        public List<IBLInterface> MissingTables { get; private set; }


        public UpdateBLGenerator(CoreModel model, string conStr) {
            this.model = new BLModel(model);
            this.createGenerator = new CreateBLGenerator(this.model);
            this.analyzer = new ModificationAnalyzer(this.model, GetConnection(conStr));            
            // Hier noch keinen Analysecode aufrufen, der gehört nach GenerateCode
        }
        ///
        /// @param model => BaseLayer Model
        /// @param conStr => Connectionstring zu einer BaseLayer-Datenbank
        ///
        public UpdateBLGenerator(BLModel model, string conStr) {
            this.model = model;
            this.createGenerator = new CreateBLGenerator(this.model);
            this.analyzer = new ModificationAnalyzer(this.model, GetConnection(conStr));
            // Hier noch keinen Analysecode aufrufen, der gehört nach GenerateCode            
        }

        public List<GeneratorResult> GenerateCode() {            
            this.MissingTables = GetMissingTables();
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Update.sql", GenerateUpdateTables()));            
            return result;
        }

        public string GenerateUpdateTables() {
            StringBuilder sb = new StringBuilder();
            // 1. Neue Tabellen hinzufügen
            sb.Append(GenerateCreateNewTables());
            // 2. Veränderte Tabellen anpassen
            //    TODO: select into -> drop -> create -> insert into select
            // 3. Alle BL-Views droppen 
            // 4. Alle BL-Views neu anlegen
            return sb.ToString();
        }

        private string GenerateCreateNewTables() {
            StringBuilder sb = new StringBuilder();
            sb.Append("/*\n * Generieren fehlender Tabellen\n */\n");
            foreach(var i in MissingTables) {
                sb.Append(createGenerator.GenerateBLTable(i));
            }
            return sb.ToString();
        }

        private SqlConnection GetConnection(string conStr) {
            return new SqlConnection(conStr);
        }

        private List<IBLInterface> GetMissingTables() {
            List<IBLInterface> temp = new List<IBLInterface>();
            foreach(var i in model.Interfaces.Where(i => i.InterfaceType != CoreInterfaceType.DIM_VIEW)) {
                if(!analyzer.TableExistsUnmodified(i) && !analyzer.InterfaceRenamed(i)) {                
                    // Tabelle existiert weder mit ihrem aktuellen noch dem früheren Namen in der Datenbank
                    // => sie fehlt.
                    temp.Add(i);
                }
            }
            return temp;
        }
    }
}