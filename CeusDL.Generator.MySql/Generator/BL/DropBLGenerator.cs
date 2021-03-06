using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BL;

namespace KDV.CeusDL.Generator.MySql.BL {
    public class DropBLGenerator : IGenerator
    {
        private BLModel model;

        public DropBLGenerator(CoreModel model) {
            this.model = new BLModel(model);
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Drop.sql", GenerateDropBLTablesAndViews()));
            result.Add(new GeneratorResult("BL_Drop_FKs.sql", GenerateDropForeignKeys()));            
            return result;
        }

        internal string GenerateDropBLTablesAndViews() {
            string code = "/*\n * BaseLayer-Tabellen aus der Datenbank entfernen\n*/\n";
            code += $"use {model.Config.BLDatabase};\n\n";

            // Constraints entfernen
            foreach(var ifa in model.Interfaces.Where(i => i.Attributes.Where(a => a is RefBLAttribute).Count() > 0)) {
                code += $"\n/* FKs von {ifa.FullName} löschen */\n";
                // Nur die Ref-Attribute durchlaufen
                foreach(var attr in ifa.Attributes.Where(a => a is RefBLAttribute)) {
                    var refAttr = (RefBLAttribute)attr;                
                    // Wir legen keine FKs für Beziehungen zwischen Faktentabellen an, das bremst zu viel
                    // außerdem legen wir auch keine FKs zu DimViews an.
                    if(refAttr.ReferencedAttribute.ParentInterface.InterfaceType != CoreInterfaceType.FACT_TABLE
                        && refAttr.ReferencedAttribute.ParentInterface.InterfaceType != CoreInterfaceType.DIM_VIEW) {
                        string fkName = $"{attr.ParentInterface.Name}_{attr.Name}_FK";
                        code += $"alter table {ifa.FullName} drop foreign key if exists {fkName};\n";
                    }
                }
            }

            // Tabellen löschen
            // TODO: Einschränken, die Tabellen zu den DimViews sollten nicht entfernt werden!
            foreach(var ifa in model.Interfaces.Where(i => i.InterfaceType != CoreInterfaceType.DIM_VIEW)) {
                code += $"/* Tabelle und View zu {ifa.ShortName} entfernen */\n";
                // View löschen
                code += $"drop view if exists {ifa.ViewName};\n";
                // Tabelle löschen
                code += $"drop table if exists {ifa.FullName};\n\n";
            }

            return code;
        }          


        internal string GenerateDropForeignKeys() {
            string code = "--\n-- Fremdschlüssel der BaseLayer-Tabellen aus der Datenbank entfernen\n--\n";
            code += $"use {model.Config.BLDatabase}\n;\n\n";

            foreach(var ifa in model.Interfaces) {
                foreach(var attr in ifa.Attributes.Where(a => a is RefBLAttribute)) {
                    var refAttr = (RefBLAttribute)attr;
                    string constraintName = $"{attr.ParentInterface.Name}_{attr.Name}_FK";
                    code += $"alter table {attr.ParentInterface.Name} drop foreign key if exists {constraintName};\n\n";
                }
            }

            return code;
        }
    }
}