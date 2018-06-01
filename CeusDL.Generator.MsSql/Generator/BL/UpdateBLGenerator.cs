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
        public List<string> DeletedTables { get; private set; }

        ///
        /// @param model => CoreModel
        /// @param conStr => Connectionstring zu einer BaseLayer-Datenbank
        ///
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
            this.DeletedTables = analyzer.ListDeletedInterfaceNames(this.model);    
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Update.sql", GenerateUpdateTables()));            
            return result;
        }

        public string GenerateUpdateTables() {
            StringBuilder sb = new StringBuilder();
            // 0. Use-Direktive generieren
            sb.Append(GenerateUse());
            // 1. Tabellen, die nicht mehr als ceusdl Interface definiert sind löschen
            sb.Append(GenerateDeleteObsoleteTables());
            // 2. Neue Tabellen hinzufügen
            sb.Append(GenerateCreateNewTables());
            // 3. Veränderte Tabellen anpassen: select into -> drop -> create -> insert into select
            sb.Append(GenerateModifyTables());
            // 4. Alle BL-Views droppen
            sb.Append(GenerateDropViews());
            // 5. Alle BL-Views neu anlegen
            sb.Append(GenerateCreateViews());            
            return sb.ToString();
        }

        private string GenerateUse() {
            string code = "";
            if(!string.IsNullOrEmpty(model.Config.BLDatabase)) {
                code += $"\nuse {model.Config.BLDatabase};\n\n";
            }
            return code;
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
            sb.Append("-- Aktualisierte Tabelle wieder mit den gesicherten Daten befüllen\n");
            foreach(var ifa in ModifiedTables) {
                sb.Append($"set identity_insert {ifa.FullName} on;\n");
                sb.Append($"insert into {ifa.FullName} (\n");
                foreach(var attr in ifa.Attributes.Where(a => a.RealFormerName != null)) {
                    sb.Append(attr.Name.Indent("    "));
                    if(ifa.Attributes.Last() != attr) {
                        sb.Append(",");
                    }
                    sb.Append("\n");
                }
                sb.Append(")\nselect \n");
                foreach(var attr in ifa.Attributes.Where(a => a.RealFormerName != null)) {
                    sb.Append(attr.RealFormerName.Indent("    "));
                    if(ifa.Attributes.Last() != attr) {
                        sb.Append(",");
                    }
                    sb.Append("\n");
                }
                sb.Append($"from {ifa.RealFormerName}_BAK;\n");
                sb.Append($"set identity_insert {ifa.FullName} off;\n\n");
            }
        }

        private void GenerateDropModifiedTables(StringBuilder sb)
        {
            sb.Append("\n/*\n * Alte Versionen der veränderten Tabellen löschen\n */\n");
            foreach(var ifa in ModifiedTables) {
                sb.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{ifa.RealFormerName}]') AND type in (N'U'))\n");
                sb.Append($"drop table {ifa.RealFormerName}\n");                
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
                sb.Append($"select * into {ifa.DatabaseName}.dbo.{ifa.RealFormerName}_BAK from {ifa.DatabaseName}.dbo.{ifa.RealFormerName};\n\n");              
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

        private string GenerateDeleteObsoleteTables()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/*\n * Löschen der Tabellen, deren Interfaces nicht mehr im ceusdl-Code enthalten sind.\n */\n");
            foreach(var table in DeletedTables) {
                sb.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{table}]') AND type in (N'U'))\n");
                sb.Append($"drop table {table}\n");                
                sb.Append("go\n\n");
            }
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
                if(analyzer.TableExistsModified(ifa) || analyzer.InterfaceRenamed(ifa)) {                   
                    SetRealFromerNames(ifa); 
                    temp.Add(ifa);
                }
            }            
            return temp;
        }

        private void SetRealFromerNames(IBLInterface ifa)
        {
            // Realen bisherigen Tabellennamen setzen
            if(analyzer.TableRenamed(ifa.Name, ifa.FormerName)) {
                ifa.RealFormerName = ifa.FormerName;
            } else {
                ifa.RealFormerName = ifa.Name;
            }

            // Reale bisherige Spaltennamen setzen
            foreach(var attr in ifa.Attributes) {
                if(analyzer.ColumnExists(ifa.RealFormerName, attr.Name)) {
                    attr.RealFormerName = attr.Name;
                } else if(analyzer.ColumnExists(ifa.RealFormerName, attr.FormerName)) {
                    attr.RealFormerName = attr.FormerName;
                } else {
                    // Neues Attribut, das keinen bisherigen Namen hat...
                    attr.RealFormerName = null;
                }
            }
        }
    }
}