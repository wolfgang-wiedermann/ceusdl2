using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BL;
using KDV.CeusDL.Utilities.MySql.BL;
using System.Text;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace KDV.CeusDL.Generator.MySql.BL {
    public class UpdateBLGenerator : IGenerator
    {
        private BLModel model;
        private MySqlModificationAnalyzer analyzer;
        private CreateBLGenerator createGenerator;        
        public List<IBLInterface> MissingTables { get; private set; }
        public List<IBLInterface> ModifiedTables { get; private set; }
        public List<string> DeletedTables { get; private set; }

        ///
        /// @param model => CoreModel
        /// @param conStr => Connectionstring zu einer BaseLayer-Datenbank
        ///
        public UpdateBLGenerator(CoreModel model, string conStr, bool generateConstraints) {
            this.model = new BLModel(model);
            this.createGenerator = new CreateBLGenerator(this.model, generateConstraints);            
            this.analyzer = new MySqlModificationAnalyzer(this.model, GetConnection(conStr));            
            // Hier noch keinen Analysecode aufrufen, der gehört nach GenerateCode
        }

        ///
        /// @param model => BaseLayer Model
        /// @param conStr => Connectionstring zu einer BaseLayer-Datenbank
        ///
        public UpdateBLGenerator(BLModel model, string conStr, bool generateConstraints) {
            this.model = model;
            this.createGenerator = new CreateBLGenerator(this.model, generateConstraints);
            this.analyzer = new MySqlModificationAnalyzer(this.model, GetConnection(conStr));
            // Hier noch keinen Analysecode aufrufen, der gehört nach GenerateCode            
        }

        public List<GeneratorResult> GenerateCode() {            
            this.MissingTables = GetMissingTables();
            this.ModifiedTables = GetModifiedTables();
            this.DeletedTables = analyzer.ListDeletedInterfaceNames(this.model).ToList();    
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Update.sql", GenerateUpdateTables()));            
            return result;
        }

        public string GenerateUpdateTables() {
            StringBuilder sb = new StringBuilder();
            // 0. Use-Direktive generieren
            sb.Append(GenerateUse());
            sb.Append(GetBeginTransaction());
            // 1. Tabellen, die nicht mehr als ceusdl Interface definiert sind löschen
            sb.Append(GenerateDeleteObsoleteTables());
            // 2. Neue Tabellen hinzufügen
            sb.Append(GenerateCreateNewTables());
            // 3. Veränderte Tabellen anpassen: select into -> drop -> create -> insert into select
            sb.Append(GenerateModifyTables());
            // 4. Constraints anlegen (FKs sind derzeit auskommentiert!!!)            
            // TODO: FKs müssten vor den Änderungen an den Tabellen für die ganze BL gedroppt werden!
            sb.Append(GenerateConstraints());
            sb.Append(GetCommitTransaction());
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

        private string GetCommitTransaction()
        {
            return "COMMIT;\n\n"; //"COMMIT TRANSACTION bl_modification_transaction;\n\n";
        }

        private string GetBeginTransaction()
        {
            return "START TRANSACTION;\n\n"; //"BEGIN TRANSACTION bl_modification_transaction;\n\n";            
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

        private string GenerateConstraints() {
            StringBuilder sb = new StringBuilder();

            sb.Append("/*\n * Generieren fehlender Unique-Key und Foreign-Key-Constraints\n */\n");

            // Unique Keys generieren
            foreach(var newIfa in MissingTables) {                
                sb.Append(createGenerator.GenerateUniqueKeyConstraint(newIfa));                
            }
            foreach(var modIfa in ModifiedTables) {
                sb.Append(createGenerator.GenerateUniqueKeyConstraint(modIfa));
            }

            // Foreign Keys generieren
            sb.Append("-- ANMERKUNG: Generierung der FKs funktioniert auch einwandfrei, die Frage ist blos ob ich die FKs wirklich will ;-) \n\n");
            /*
            foreach(var newIfa in MissingTables) {
                sb.Append(createGenerator.GenerateForeignKeyConstraints(newIfa));                
            }
            foreach(var modIfa in ModifiedTables) {
                sb.Append(createGenerator.GenerateForeignKeyConstraints(modIfa));
            }
            */

            return sb.ToString();
        }

        private void GenerateInsertIntoSelect(StringBuilder sb)
        {
            sb.Append("/*\n * Aktualisierte Tabelle wieder mit den gesicherten Daten befüllen\n */\n");
            foreach(var ifa in ModifiedTables) {
                sb.Append($"insert into {ifa.Name} (\n");
                foreach(var attr in ifa.Attributes.Where(a => a.RealFormerName != null)) {
                    sb.Append(attr.Name.Indent("    "));
                    if(ifa.Attributes.Last() != attr) {
                        sb.Append(",");
                    }
                    sb.Append("\n");
                }
                sb.Append(")\nselect \n");
                foreach(var attr in ifa.Attributes.Where(a => a.RealFormerName != null)) {
                    sb.Append(WrapWithCast(attr, attr.RealFormerName).Indent("    "));
                    if(ifa.Attributes.Last() != attr) {
                        sb.Append(",");
                    }
                    sb.Append("\n");
                }
                sb.Append($"from {ifa.RealFormerName}_BAK;\n\n");
            }
        }

        private void GenerateDropModifiedTables(StringBuilder sb)
        {
            sb.Append("\n/*\n * Alte Versionen der veränderten Tabellen löschen\n */\n");
            foreach(var ifa in ModifiedTables) {
                sb.Append($"drop table if exists {ifa.RealFormerName};\n");                
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
                // Select into table wird in MySQL nicht unterstützt ...           
                sb.Append($"create table {ifa.RealFormerName}_BAK select * from {ifa.RealFormerName};\n\n");              
            }
        }

        private string GenerateDropViews() {
            StringBuilder sb = new StringBuilder();
            sb.Append("/*\n * Löschen der bestehenden Views\n */\n");
            foreach(var ifa in model.Interfaces.Where(i => i.InterfaceType != CoreInterfaceType.DIM_VIEW && i.InterfaceType != CoreInterfaceType.DEF_TABLE)) {
                sb.Append($"-- View zu {ifa.ShortName} entfernen\n");
                // View löschen
                sb.Append($"drop view if exists {ifa.ViewName};\n");
                if(ifa.FormerName != null) {
                    sb.Append($"drop view if exists {ifa.FormerName}_VW;\n");
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
                sb.Append($"drop table if exists {table};\n");                
            }
            return sb.ToString();
        }

        private MySqlConnection GetConnection(string conStr) {
            return new MySqlConnection(conStr);
        }

        private List<IBLInterface> GetMissingTables() {
            List<IBLInterface> temp = new List<IBLInterface>();
            foreach(var i in model.Interfaces.Where(i => i.InterfaceType != CoreInterfaceType.DIM_VIEW)) {
                if(!analyzer.TableWithNameExists(i.Name, i.ParentModel.Config.BLDatabase) && !analyzer.InterfaceRenamed(i)) {                
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

        private string WrapWithCast(IBLAttribute attr, string name) {
            switch(attr.DataType) {
                case CoreDataType.VARCHAR:
                    return $"cast({name} as varchar({attr.Length})) as {name}";
                case CoreDataType.DECIMAL:
                    return $"cast({name} as decimal({attr.Length}, {attr.Decimals})) as {name}";
                default:
                    // TODO: Prüfen ob das für die anderen Typen auch relevant ist und wie ich 
                    //       das dann am besten umsetze.
                    // => Im Test hat es mit date, datetime und time auch ohne cast funktioniert,
                    //    ich hab aber nie von date nach varchar oder umgekehrt ausprobiert!
                    return name;
            }
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
                } else if(attr is RefBLAttribute) {                    
                    var refAttr = (RefBLAttribute)attr;                    
                    if(refAttr?.Core?.FormerName != null && refAttr?.Core?.Alias != null) {
                        // Felder FormerName und Alias des Attributs sind gefüllt
                        if(analyzer.ColumnExists(ifa.RealFormerName, $"{refAttr.Core.FormerName}_{refAttr.Core.ReferencedAttribute.FormerName}")) {
                            // Die Namensänderung des ReferencedAttribute und des Alias wurden noch nicht in der DB aktualisiert
                            attr.RealFormerName = $"{refAttr.Core.FormerName}_{refAttr.Core.ReferencedAttribute.FormerName}";
                        } else if(analyzer.ColumnExists(ifa.RealFormerName, $"{refAttr.Core.Alias}_{refAttr.Core.ReferencedAttribute.FormerName}")) {
                            // Alias wurde bereits in der Datenbank aktualisiert, die Namensänderung des ReferencedAttribute aber nicht
                            attr.RealFormerName = $"{refAttr.Core.Alias}_{refAttr.Core.ReferencedAttribute.FormerName}";
                        } else if(analyzer.ColumnExists(ifa.RealFormerName, $"{refAttr.Core.FormerName}_{refAttr.Core.ReferencedAttribute.Name}")) {
                            // Die Namensänderung des ReferencedAttribute wurde bereits in der DB aktualisiert, das Alias aber noch nicht
                            attr.RealFormerName = $"{refAttr.Core.FormerName}_{refAttr.Core.ReferencedAttribute.Name}";
                        } else if(analyzer.ColumnExists(ifa.RealFormerName, $"{refAttr.Core.Alias}_{refAttr.Core.ReferencedAttribute.Name}")) {
                            // Alias und ReferencedAttribute wurden bereits aktualisiert
                            attr.RealFormerName = $"{refAttr.Core.Alias}_{refAttr.Core.ReferencedAttribute.Name}";
                        } else {
                            // Das Attribut hat es entweder vorher nicht gegeben oder es ist aus anderen Gründen nicht in der DB
                            attr.RealFormerName = null;
                        }
                    } else if(refAttr?.Core?.FormerName == null && refAttr?.Core?.Alias != null) {
                        // Nur ein Alias ist definiert, kein FormerName
                        if(analyzer.ColumnExists(ifa.RealFormerName, $"{refAttr.Core.Alias}_{refAttr.ReferencedAttribute.FormerName}")) {
                            // Alias wurde bereits in der Datenbank aktualisiert, die Namensänderung des ReferencedAttribute aber nicht
                            attr.RealFormerName = $"{refAttr.Core.Alias}_{refAttr.ReferencedAttribute.FormerName}";
                        } else if(analyzer.ColumnExists(ifa.RealFormerName, $"{refAttr.Core.Alias}_{refAttr.ReferencedAttribute.Name}")) {
                            // Alias und ReferencedAttribute wurden bereits aktualisiert
                            attr.RealFormerName = $"{refAttr.Core.Alias}_{refAttr.ReferencedAttribute.Name}";                        
                        } else {
                            // Das Attribut hat es entweder vorher nicht gegeben oder es ist aus anderen Gründen nicht in der DB
                            attr.RealFormerName = null;
                        }
                    } else if(refAttr?.Core?.FormerName != null && refAttr?.Core?.Alias == null) {
                        // Nur ein FormerName ist definiert aber kein Alias
                        if(analyzer.ColumnExists(ifa.RealFormerName, $"{refAttr.Core.FormerName}_{refAttr.Core.ReferencedAttribute.FormerName}")) {
                            // Die Namensänderung des ReferencedAttribute und des Alias wurden noch nicht in der DB aktualisiert
                            attr.RealFormerName = $"{refAttr.Core.FormerName}_{refAttr.Core.ReferencedAttribute.FormerName}";
                        } else if(analyzer.ColumnExists(ifa.RealFormerName, $"{refAttr.Core.FormerName}_{refAttr.Core.ReferencedAttribute.Name}")) {
                            // Die Namensänderung des ReferencedAttribute wurde bereits in der DB aktualisiert, das Alias aber noch nicht
                            attr.RealFormerName = $"{refAttr.Core.FormerName}_{refAttr.Core.ReferencedAttribute.Name}";
                        } else if(analyzer.ColumnExists(ifa.RealFormerName, refAttr.Core.ReferencedAttribute.Name)) {
                            // Die Spalte hatte früher ein Alias, jetzt aber nichtmehr, Änderung bereits in der DB umgesetzt.
                            // Achtung: darf nur in diesem Sonderfall so gezogen werden!
                            attr.RealFormerName = refAttr.Core.ReferencedAttribute.Name;
                        } else {
                            // Das Attribut hat es entweder vorher nicht gegeben oder es ist aus anderen Gründen nicht in der DB
                            attr.RealFormerName = null;
                        }
                    } else {
                        // Weder FormerName noch Alias sind definiert
                        if(analyzer.ColumnExists(ifa.RealFormerName, refAttr.Core.ReferencedAttribute.Name)) {                            
                            // Achtung: darf nur in diesem Sonderfall so gezogen werden!
                            attr.RealFormerName = refAttr.Core.ReferencedAttribute.Name;
                        } else {
                            // Das Attribut hat es entweder vorher nicht gegeben oder es ist aus anderen Gründen nicht in der DB
                            attr.RealFormerName = null;
                        }
                    }                                     
                } else {
                    // Neues Attribut, das keinen bisherigen Namen hat...
                    attr.RealFormerName = null;
                }
            }
        }
    }
}