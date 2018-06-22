using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;
using System.Text;

namespace KDV.CeusDL.Generator.BL {
    public class LoadBLGenerator : IGenerator
    {
        private BLModel model;

        public LoadBLGenerator(CoreModel model) {
            this.model = new BLModel(model);
        }

        public LoadBLGenerator(BLModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Load.sql", GenerateLoadTables()));            
            return result;
        }

        private string GenerateLoadTables()
        {
            var sb = new StringBuilder();

            // Dimensionen
            foreach(var ifa in model.DimTableInterfaces) {
                sb.Append(GenerateDimTableUpdate(ifa));
                sb.Append(GenerateDimTableInsert(ifa));                
            }            

            // Fakten
            foreach(var ifa in model.FactTableInterfaces.OrderByDescending(i => i.MaxReferenceDepth)) {
                sb.Append(GenerateFactTableDelete(ifa));             
            }
            foreach(var ifa in model.FactTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {                
                sb.Append(GenerateFactTableInsert(ifa));
            }

            return sb.ToString();
        }

        internal string GenerateFactTableInsert(IBLInterface ifa)
        {
            // Guard-Condition
            if(ifa.InterfaceType != CoreInterfaceType.FACT_TABLE) {
                throw new InvalidInterfaceTypeException("GenerateFactTableInsert darf nur mit einem FactTableInterface aufgerufen werden");
            }

            string code = $"-- Laden der Tabelle {ifa.FullName}\n";
            code += $"insert into {ifa.FullName} (\n";
            var relevantAttributes = ifa.Attributes.Where(a => !a.IsIdentity).OrderBy(a => a.SortId);
            foreach(var attr in relevantAttributes) {
                // Alle Attribute, aber ohne ID
                code += $"{attr.Name}".Indent("    ");
                if(attr != relevantAttributes.Last()) {
                    code += ", \n";
                }
            }
            code += ")\nselect\n";
            foreach(var attr in relevantAttributes) {
                if(attr.IsTechnicalAttribute) {                    
                    switch(attr.Name) {
                        case "T_Modifikation":
                            code += "'I'".Indent("    ");
                            break;
                        case "T_Bemerkung":
                            code += "'Insert bei Ladelauf'".Indent("    ");
                            break;
                        case "T_Benutzer":
                            code += "SYSTEM_USER".Indent("    ");
                            break;
                        case "T_System":
                            code += "'SRC'".Indent("    ");
                            break;
                        case "T_Erst_Dat":
                            code += "GETDATE()".Indent("    ");
                            break;
                        case "T_Aend_Dat":
                            code += "GETDATE()".Indent("    ");
                            break;
                        case "T_Ladelauf_NR":
                            // TODO: Echte Ermittlung einer LadelaufNr einbauen:
                            code += "0".Indent("    ");
                            break;                        
                    }
                } else if(attr.Name == "Mandant_KNZ") {
                    code += "Mandant_KNZ".Indent("    ");
                } else {
                    code += $"{attr.GetILAttribute().Name} as {attr.Name}".Indent("    ");
                }
                if(attr != relevantAttributes.Last()) {
                    code += ", \n";
                }
            }
            code += $"\nfrom {ifa.GetILInterface().FullName}\n\n";
            return code;
        }

        internal string GenerateFactTableDelete(IBLInterface ifa) {
            string code = $"-- Löschen der neu zu ladenden Inhalte von {ifa.FullName}\n";
            code += $"delete from {ifa.FullName} \n";                        
            code += GenerateFactRelevantTimeSelector(ifa);
            code += "\n";
            return code;
        }

        private string GenerateFactRelevantTimeSelector(IBLInterface ifa) {
            string code = "";
            if(ifa.IsHistorized) {
                var histAttr = ifa.HistoryAttribute;
                code += $"where {histAttr.Name} in (\n";
                code += $"select distinct tmp1.{histAttr.GetILAttribute().Name}\n".Indent("    ");
                code += $"from {ifa.GetILInterface().FullName} as tmp1\n".Indent("    ");
                if(ifa.IsMandant) {
                    code += "where tmp1.Mandant_KNZ = Mandant_KNZ\n".Indent("    ");
                }
                code += ")\n";
            }
            return code;
        }

        private string GenerateDimTableUpdate(IBLInterface ifa)
        {
            if(ifa.IsHistorized && ifa is DerivedBLInterface) {
                return GenerateUpdatesForDimTableWithHistory(ifa);
            } else {
                return GenerateDimTableUpdateNoHist(ifa);
            }            
        }

        private string GenerateDimTableUpdateNoHist(IBLInterface ifa)
        {
            var idAttribute = ifa.Attributes.Where(a => a.IsIdentity).First();

            StringBuilder sb = new StringBuilder();
            sb.Append($"-- Update for non historized table: {ifa.FullName}\n");
            sb.Append("update t set\n");            
            foreach(var attr in ifa.UpdateCheckAttributes) {
                sb.Append($"t.{attr.Name} = v.{attr.Name},\n".Indent("    "));
            }
            sb.Append("t.T_Modifikation = 'U',\nt.T_Aend_Dat = GETDATE(),\nt.T_Benutzer = SYSTEM_USER\n".Indent("    "));
            sb.Append($"from {ifa.FullName} t\ninner join {ifa.FullViewName} v\n");
            sb.Append($"on t.{idAttribute.Name} = v.{idAttribute.Name}\n    and v.T_Modifikation = 'U'\n\n");
            return sb.ToString();
        }

        private string GenerateUpdatesForDimTableWithHistory(IBLInterface ifa)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-- Aktualisierung innerhalb des aktuellen Zeitraums\n");
            GenerateInTimeUpdateForDimTableWithHistory(ifa, sb);
            sb.Append("\n");
            sb.Append("-- Aktualisierung außerhalb des aktuellen Zeitruams = Abschließen eines Dimensionssatzes\n");
            GeneratePastTimeUpdateForDimTableWithHistory(ifa, sb);
            sb.Append("\n");
            return sb.ToString();
        }

        ///
        /// Generiert ein Update-Statement, das dann die historisierungsrelevanten
        /// Attribute des Interfaces aktualisiert, wenn der letzte Historiensatz
        /// in der gleichen Zeiteinheit angelegt wurde, für die die aktuell
        /// zu ladenden Daten geliefert wurden.
        ///
        private void GenerateInTimeUpdateForDimTableWithHistory(IBLInterface ifa, StringBuilder sb) {
            var idAttribute = ifa.Attributes.Where(a => a.IsIdentity).First();
            sb.Append("update t\n");
            sb.Append("set\n");
            foreach(var attr in ifa.UpdateCheckAttributes) {
                sb.Append($"t.{attr.Name} = v.{attr.Name}".Indent("    "));
                if(attr != ifa.UpdateCheckAttributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append($"from {ifa.FullName} t\n");
            sb.Append($"inner join {ifa.FullViewName} v\n");
            sb.Append($"on t.{idAttribute.Name} = v.{idAttribute.Name} \nand v.T_Modifikation = 'U'\n".Indent("    "));
            sb.Append("where exists (\n");

            sb.Append($"select t1.{idAttribute.Name}\nfrom {ifa.FullName} as t1\n".Indent("    "));
            sb.Append("where ".Indent("    "));
            foreach(var uk in ifa.UniqueKeyAttributes.Where(a => ifa.HistoryAttribute != a)) {
                sb.Append($"t1.{uk.Name} = t.{uk.Name} and\n          ");
            }
            sb.Append($"t1.{ifa.HistoryAttribute.Name} = dbo.GetCurrentTimeForHistory()\n");

            sb.Append(")\n");
        }

        ///
        /// Schließt den bisher aktuellen Dimensionssatz ab
        /// (Läuft i. d. R. wenn entweder noch kein abgeschlossener Satz
        ///  zum Unique-Key dieses Dimensionsobjekts existiert, oder der aktuellste
        ///  abgeschlossene Satz zu einem Zeitpunkt != GetCurrentTimeForHistory 
        ///  abgeschlossen wurde. Dieser Zusammenhang wird ausschließlich durch
        ///  vorheriges Ausführen von GenerateInTimeUpdateForDimTableWithHistory sichergestellt.)
        ///
        private void GeneratePastTimeUpdateForDimTableWithHistory(IBLInterface ifa, StringBuilder sb) {
            var idAttribute = ifa.Attributes.Where(a => a.IsIdentity).First();            
            sb.Append($"-- Update historized table: {ifa.FullName}\n");
            sb.Append("update t \nset t.T_Gueltig_Bis_Dat = dbo.GetCurrentTimeForHistory()\n");
            sb.Append($"from {ifa.FullName} t\n");
            sb.Append($"inner join {ifa.FullViewName} v\n");
            sb.Append($"on t.{idAttribute.Name} = v.{idAttribute.Name} \nand v.T_Modifikation = 'U'\n\n".Indent("    "));
        }

        // 
        // Fügt alle mit I markierten Sätze in die entsprechenden Tabellen ein ...
        //
        private string GenerateDimTableInsert(IBLInterface ifa) {
            string fieldList = "";
            var relevantAttributes = ifa.Attributes.Where(a => (!a.IsTechnicalAttribute || a.Name.Equals("T_Modifikation")) && !a.IsIdentity);

            foreach(var attr in relevantAttributes) {
                fieldList += attr.Name;                
                fieldList += ", \n";                
            }

            var upperFieldList = fieldList + "T_Ladelauf_NR, \nT_Benutzer, \nT_System, \nT_Erst_Dat, \nT_Aend_Dat";
            var lowerFieldList = fieldList + "0, \nSYSTEM_USER, \n'H', \nGETDATE(), \nGETDATE()";

            string code = $"-- Insert {ifa.FullName}\n";
            code += $"insert into {ifa.FullName} (\n{upperFieldList.Indent("    ")}\n)\n";
            code += $"select \n{lowerFieldList.Indent("    ")} \nfrom {ifa.FullViewName}\n";
            code += "where T_Modifikation = 'I'\n";
            code += "\n";
            return code;
        }
    }
}