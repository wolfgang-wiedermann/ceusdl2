using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BL;
using System.Text;

namespace KDV.CeusDL.Generator.MySql.BL {
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

            if(!string.IsNullOrEmpty(model.Config.BLDatabase)) {
                sb.Append($"\nuse {model.Config.BLDatabase};\n\n");
            }

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

            // Kaskadierende Versionsgenerierung (nur DerivedInterfaces mit Ref-Attributen durchlaufen)
            var ifaForCascade = model.Interfaces
                .Where(i => i is DerivedBLInterface && 
                    (i.InterfaceType == CoreInterfaceType.DIM_TABLE 
                     || i.InterfaceType == CoreInterfaceType.DIM_VIEW
                    )
                )
                .Where(i => i.Attributes.Where(a => a is RefBLAttribute).Count() > 0)
                .OrderByDescending(i => i.MaxReferenceDepth) // Wichtig!
                .Select(i => (DerivedBLInterface)i);

            sb.Append(";\n"); // Ohne dass die vorherigen Anwendungen mit ; abgeschlossen sind geht die CTE in CascadeVersions nicht!
            foreach(var ifa in ifaForCascade) {
                sb.Append(GenerateCascadeVersions(ifa));
            }

            return sb.ToString();
        }

        private string GenerateCascadeVersions(DerivedBLInterface ifa)
        {
            StringBuilder sb = new StringBuilder();            

            // Nur Beziehungen zwischen zwei historisierten Dimensionen sind relevant
            var relevantRelationships = ifa.Attributes.Where(a => a is RefBLAttribute)
                .Select(a => (RefBLAttribute)a)
                .Where(a => a.ReferencedAttribute.ParentInterface.IsHistorized)
                .ToList();

            foreach(var rel in relevantRelationships) {                
                GenerateCascadeVersions(ifa, rel, sb);
            }
            
            return sb.ToString();
        }

        ///
        /// Kaskadierendes Auflösung von Versionen von Kind-Dimensionen in den 
        /// Elterndimensionen für ein einzelnes Eltern-Kind-Paar auflösen
        ///
        private void GenerateCascadeVersions(DerivedBLInterface childIfa, RefBLAttribute reference, StringBuilder sb) {
            var parentIfa = GetDerivedForDefault(reference.ReferencedAttribute.ParentInterface);
            var parentNonIdentityAttributes = parentIfa.Attributes.Where(a => !a.IsIdentity);
            var max = GetMaxValueForHistoryAttribute(parentIfa.HistoryAttribute);

            sb.Append($"-- Cascade Versions for {parentIfa.Name} -> {reference.ReferencedAttribute.FullName}\n");            
            sb.Append($"-- {childIfa.Name}\n");

            // Ermittlung der im parentIfa fehlenden Versionen
            sb.Append("with missing_versions as (\n");
            sb.Append("select\n".Indent(1));
            foreach(var uk in childIfa.UniqueKeyAttributes.Where(a => childIfa.HistoryAttribute != a)) {
                sb.Append($"t2.{uk.Name},\n".Indent(2));
            }
            sb.Append($"t2.{reference.Name} as {reference.ReferencedAttribute.Name},\n".Indent(2));
            sb.Append($"t2.{childIfa.HistoryAttribute.Name}\n".Indent(2));
            sb.Append($"from {childIfa.FullName} as t2\nleft outer join {parentIfa.FullName} as t1\n".Indent(1));
            sb.Append($"on t2.{reference.Name} = t1.{reference.ReferencedAttribute.Name}\n".Indent(2));
            if(childIfa.IsMandant && parentIfa.IsMandant) {
                sb.Append($"and t2.Mandant_KNZ = t1.Mandant_KNZ\n".Indent(2));
            }
            sb.Append($"and coalesce(t1.{parentIfa.HistoryAttribute.Name}, 'NOW') = coalesce(t2.{childIfa.HistoryAttribute.Name}, 'NOW')\n".Indent(2));
            sb.Append($"where t1.{parentIfa.PrimaryKeyAttributes.First().Name} is null\n".Indent(1));
            sb.Append(")\n");

            // Einfügen der fehlenden Versionen in die BL-Tabelle zu parrentIfa
            sb.Append($"insert into {parentIfa.FullName} (\n");            
            foreach(var attr in parentNonIdentityAttributes) {                
                sb.Append(attr.Name.Indent(1));
                if(attr != parentNonIdentityAttributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append(")\n");
            sb.Append("select\n");
            foreach(var attr in parentNonIdentityAttributes.Where(a => !a.IsTechnicalAttribute)) {
                sb.Append($"t1.{attr.Name},\n".Indent(1));
            }
            sb.Append("'I' as T_Modifikation,\n".Indent(1));
            sb.Append($"cast('Cascaded for {reference.FullName}' as varchar(100)) as T_Bemerkung,\n".Indent(1));
            sb.Append("CURRENT_USER() as T_Benutzer,\n".Indent(1));
            sb.Append("'H' as T_System,\n".Indent(1));
            sb.Append($"mv.{childIfa.HistoryAttribute.Name} as {parentIfa.HistoryAttribute.Name},\n".Indent(1));
            sb.Append("now() as T_Erst_Dat,\now() as T_Aend_Dat,\n".Indent(1));
            sb.Append("t1.T_Ladelauf_NR\n".Indent(1));
            sb.Append($"from {parentIfa.FullName} as t1\n");
            sb.Append($"inner join missing_versions as mv\n");
            var parentUkWithoutHistory = parentIfa.UniqueKeyAttributes.Where(a => parentIfa.HistoryAttribute != a);
            foreach(var uk in parentUkWithoutHistory) {
                if(uk == parentUkWithoutHistory.First()) {
                    sb.Append($"on t1.{uk.Name} = mv.{uk.Name}\n".Indent(1));
                } else {
                    sb.Append($"and t1.{uk.Name} = mv.{uk.Name}\n".Indent(1));
                }                
            }            
            sb.Append($"and coalesce(t1.{parentIfa.HistoryAttribute.Name}, '{max}') = (\n".Indent(1));
            sb.Append($"select min(coalesce(z.{parentIfa.HistoryAttribute.Name}, '{max}'))\n".Indent(2));
            sb.Append($"from {parentIfa.FullName} as z\n".Indent(2));
            foreach(var uk in parentUkWithoutHistory) {
                if(uk == parentUkWithoutHistory.First()) {
                    sb.Append($"where z.{uk.Name} = t1.{uk.Name}\n".Indent(2));
                } else {
                    sb.Append($"and z.{uk.Name} = t1.{uk.Name}\n".Indent("          "));
                }
            }
            sb.Append($"and coalesce(z.{parentIfa.HistoryAttribute.Name}, '{max}') >= coalesce(mv.{parentIfa.HistoryAttribute.Name}, '{max}')\n".Indent("          "));
            sb.Append(")\n".Indent(1));
            sb.Append(";\n");
        }

        public static string GetMaxValueForHistoryAttribute(IBLAttribute hist) {
            switch(hist.DataType) {
                case CoreDataType.INT: return Int32.MaxValue.ToString();
                case CoreDataType.DECIMAL: return Decimal.MaxValue.ToString();
                case CoreDataType.VARCHAR: return "MAX_VALUE";
                case CoreDataType.DATETIME: return DateTime.MaxValue.ToString("yyyy-MM-dd hh:mm:ss");
                case CoreDataType.DATE: return DateTime.MaxValue.ToString("yyyy-MM-dd");
                case CoreDataType.TIME: return DateTime.MaxValue.ToString("hh:mm:ss");                
                default: 
                    throw new InvalidDataTypeException();
            }
        }

        private DerivedBLInterface GetDerivedForDefault(IBLInterface ifa) {
            if(!(ifa is DefaultBLInterface))
                throw new InvalidInterfaceTypeException("GetDerivedForDefault erwartet ein DefaultInterface!");

            return this.model.Interfaces
                .Where(i => i is DerivedBLInterface)
                .Select(i => (DerivedBLInterface)i)
                .Where(i => i.DefaultInterface == ifa)
                .FirstOrDefault();
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
                            code += "CURRENT_USER()".Indent("    ");
                            break;
                        case "T_System":
                            code += "'SRC'".Indent("    ");
                            break;
                        case "T_Erst_Dat":
                            code += "now()".Indent("    ");
                            break;
                        case "T_Aend_Dat":
                            code += "now()".Indent("    ");
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
            code += $"\nfrom {ifa.GetILInterface().FullName};\n\n";
            return code;
        }

        internal string GenerateFactTableDelete(IBLInterface ifa) {
            string code = $"-- Löschen der neu zu ladenden Inhalte von {ifa.FullName}\n";
            code += $"delete from {ifa.FullName} \n";                        
            code += GenerateFactRelevantTimeSelector(ifa);
            code += ";\n";
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
            sb.Append($"update {ifa.FullName} t\ninner join {ifa.FullViewName} v\n");
            sb.Append($"on t.{idAttribute.Name} = v.{idAttribute.Name}\n    and v.T_Modifikation = 'U'\nset ");            
            foreach(var attr in ifa.UpdateCheckAttributes) {
                sb.Append($"t.{attr.Name} = v.{attr.Name},\n".Indent("    "));
            }
            sb.Append("t.T_Modifikation = 'U',\nt.T_Aend_Dat = now(),\nt.T_Benutzer = CURRENT_USER();\n\n".Indent("    "));
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
            sb.Append($"update {ifa.FullName} t\n");
            sb.Append($"inner join {ifa.FullViewName} v\n");
            sb.Append($"on t.{idAttribute.Name} = v.{idAttribute.Name} \nand v.T_Modifikation = 'U'\n".Indent("    "));
            sb.Append("set\n");
            foreach(var attr in ifa.UpdateCheckAttributes) {
                sb.Append($"t.{attr.Name} = v.{attr.Name}".Indent("    "));
                if(attr != ifa.UpdateCheckAttributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append("where exists (\n");

            sb.Append($"select t1.{idAttribute.Name}\nfrom {ifa.FullName} as t1\n".Indent("    "));
            sb.Append("where ".Indent("    "));
            foreach(var uk in ifa.UniqueKeyAttributes.Where(a => ifa.HistoryAttribute != a)) {
                sb.Append($"t1.{uk.Name} = t.{uk.Name} and\n          ");
            }
            sb.Append($"t1.{ifa.HistoryAttribute.Name} = dbo.GetCurrentTimeForHistory()\n");

            sb.Append(");\n\n");
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
            sb.Append($"update {ifa.FullName} as t\n");
            sb.Append($"inner join {ifa.FullViewName} v\n");
            sb.Append($"on t.{idAttribute.Name} = v.{idAttribute.Name} \nand v.T_Modifikation = 'U'\n".Indent("    "));
            sb.Append($"set t.T_Gueltig_Bis_Dat = dbo.GetCurrentTimeForHistory();\n\n");
            
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
            var lowerFieldList = fieldList + "0, \nCURRENT_USER(), \n'H', \nnow(), \nnow()";

            string code = $"-- Insert {ifa.FullName}\n";
            code += $"insert into {ifa.FullName} (\n{upperFieldList.Indent("    ")}\n)\n";
            code += $"select \n{lowerFieldList.Indent("    ")} \nfrom {ifa.FullViewName}\n";
            code += "where T_Modifikation = 'I';\n";
            code += "\n";
            return code;
        }
    }
}