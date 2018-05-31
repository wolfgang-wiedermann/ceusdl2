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
        public List<IBLInterface> ModifiedTables { get; private set; }


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
            this.ModifiedTables = GetModifiedTables();            
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
            sb.Append(GenerateModifyTables());
            // 3. Alle BL-Views droppen
            sb.Append(GenerateDropViews());
            // 4. Alle BL-Views neu anlegen
            sb.Append(GenerateCreateViews());
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

        
        private string GenerateModifyTables()
        {
            StringBuilder sb = new StringBuilder();
            GenerateSelectInto(sb);
            GenerateDropModifiedTables(sb);
            GenerateCreateModifiedTables(sb);
            GenerateInsertIntoSelect(sb);
            return sb.ToString();
        }

        private void GenerateInsertIntoSelect(StringBuilder sb)
        {
            sb.Append("-- TODO: insert into NEUE_TABELLE select ... from ALTE_TABELLE\n");
        }

        private void GenerateDropModifiedTables(StringBuilder sb)
        {
            sb.Append("\n/*\n * Alte Versionen der veränderten Tabellen löschen\n */\n");
            foreach(var ifa in ModifiedTables) {
                if(analyzer.InterfaceRenamed(ifa)) {
                    sb.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{ifa.FormerName}]') AND type in (N'U'))\n");
                    sb.Append($"drop table {ifa.FormerName}\n");
                } else {
                    sb.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{ifa.Name}]') AND type in (N'U'))\n");
                    sb.Append($"drop table {ifa.FullName}\n");
                }
                sb.Append("go\n\n");
            }
        }

        private void GenerateCreateModifiedTables(StringBuilder sb)
        {
            sb.Append("\n/*\n * Veränderte Tabellen neu anlegen\n */\n");
            foreach(var i in ModifiedTables) {
                sb.Append(createGenerator.GenerateBLTable(i));
            }
        }

        private void GenerateSelectInto(StringBuilder sb)
        {
            sb.Append("\n/*\n * Veränderte Tabellen sichern\n */\n");
            foreach(var ifa in ModifiedTables) {
                if(analyzer.InterfaceRenamed(ifa)) {
                    sb.Append($"select * into {ifa.FullFormerName}_BAK from {ifa.FullFormerName};\n\n");
                } else {
                    sb.Append($"select * into {ifa.FullName}_BAK from {ifa.FullName};\n\n");
                }
            }
        }

        private string GenerateDropViews() {
            StringBuilder sb = new StringBuilder();
            sb.Append("/*\n * Löschen der bestehenden Views\n */\n");
            foreach(var ifa in model.Interfaces.Where(i => i.InterfaceType != CoreInterfaceType.DIM_VIEW && i.InterfaceType != CoreInterfaceType.DEF_TABLE)) {
                sb.Append($"-- View zu {ifa.ShortName} entfernen\n");
                // View löschen
                sb.Append($"IF OBJECT_ID(N'{ifa.ViewName}', N'V') IS NOT NULL\n");
                sb.Append($"DROP VIEW {ifa.ViewName}\n");
                sb.Append("go\n\n");
                if(ifa.FormerName != null) {
                    sb.Append($"IF OBJECT_ID(N'{ifa.FormerName}_VW', N'V') IS NOT NULL\n");
                    sb.Append($"DROP VIEW {ifa.FormerName}_VW\n");
                    sb.Append("go\n\n");
                }
            }
            return sb.ToString();
        }

        private string GenerateCreateViews() {
            StringBuilder sb = new StringBuilder();
            sb.Append("/*\n * Neu erstellen der Views\n */\n");
            foreach(var ifa in model.DimTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {
                // Diese Logik gibt es 2x, siehe auch CreateBLGenerator.GenerateCreateTables                               
                if(ifa is DerivedBLInterface) {
                    sb.Append(createGenerator.GenerateHistorizedDimTableView(ifa));                    
                } else {
                    sb.Append(createGenerator.GenerateDimTableView(ifa));                              
                }
            }
            //TODO: loop über model.FactTableInterfaces...
            //TODO: code += GenerateFactView(ifa);
            return sb.ToString();
        }

        private SqlConnection GetConnection(string conStr) {
            return new SqlConnection(conStr);
        }

        private List<IBLInterface> GetMissingTables() {
            List<IBLInterface> temp = new List<IBLInterface>();
            foreach(var i in model.Interfaces.Where(i => i.InterfaceType != CoreInterfaceType.DIM_VIEW)) {
                if(!analyzer.TableWithNameExists(i.Name) && !analyzer.InterfaceRenamed(i)) {                
                    // Tabelle existiert weder mit ihrem aktuellen noch dem früheren Namen in der Datenbank
                    // => sie fehlt.
                    temp.Add(i);
                }
            }
            return temp;
        }

        private List<IBLInterface> GetModifiedTables() {
            List<IBLInterface> temp = new List<IBLInterface>();
            foreach(var ifa in model.Interfaces.Where(i => i.InterfaceType != CoreInterfaceType.DIM_VIEW)) {
                if(analyzer.TableExistsModified(ifa)) {
                    temp.Add(ifa);
                } else if(analyzer.InterfaceRenamed(ifa)) {
                    temp.Add(ifa);
                }
            }            
            return temp;
        }
    }
}