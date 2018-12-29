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
            string code = "--\n-- BaseLayer-Tabellen aus der Datenbank entfernen\n--\n";
            code += $"use {model.Config.BLDatabase}\nGO\n\n";

            // Constraints entfernen
            foreach(var ifa in model.Interfaces.Where(i => i.Attributes.Where(a => a is RefBLAttribute).Count() > 0)) {
                code += $"\n-- FKs von {ifa.FullName} löschen\n--\n";
                // Nur die Ref-Attribute durchlaufen
                foreach(var attr in ifa.Attributes.Where(a => a is RefBLAttribute)) {
                    var refAttr = (RefBLAttribute)attr;                
                    // Wir legen keine FKs für Beziehungen zwischen Faktentabellen an, das bremst zu viel
                    // außerdem legen wir auch keine FKs zu DimViews an.
                    if(refAttr.ReferencedAttribute.ParentInterface.InterfaceType != CoreInterfaceType.FACT_TABLE
                        && refAttr.ReferencedAttribute.ParentInterface.InterfaceType != CoreInterfaceType.DIM_VIEW) {
                        string fkName = $"{attr.ParentInterface.Name}_{attr.Name}_FK";
                        code += $"IF (OBJECT_ID('{fkName}', 'F') IS NOT NULL) BEGIN \n";
                        code += $"    alter table {ifa.FullName} drop constraint {fkName};\n";
                        code += "END\n\n";
                    }
                }
            }

            // Tabellen löschen
            // TODO: Einschränken, die Tabellen zu den DimViews sollten nicht entfernt werden!
            foreach(var ifa in model.Interfaces.Where(i => i.InterfaceType != CoreInterfaceType.DIM_VIEW)) {
                code += $"-- Tabelle und View zu {ifa.ShortName} entfernen\n";
                // View löschen
                code += $"IF OBJECT_ID(N'{ifa.ViewName}', N'V') IS NOT NULL\n";
                code += $"DROP VIEW {ifa.ViewName}\n";
                code += "go\n\n";
                // Tabelle löschen
                code += $"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{ifa.Name}]') AND type in (N'U'))\n";
                code += $"drop table {ifa.FullName}\n";
                code += "go\n\n";
            }

            return code;
        }          


        internal string GenerateDropForeignKeys() {
            string code = "--\n-- Fremdschlüssel der BaseLayer-Tabellen aus der Datenbank entfernen\n--\n";
            code += $"use {model.Config.BLDatabase}\nGO\n\n";

            foreach(var ifa in model.Interfaces) {
                foreach(var attr in ifa.Attributes.Where(a => a is RefBLAttribute)) {
                    var refAttr = (RefBLAttribute)attr;
                    string constraintName = $"{attr.ParentInterface.Name}_{attr.Name}_FK";
                    code += "if exists (\n";
                    code += "select constraint_name from information_schema.referential_constraints \n".Indent("    ");
                    code += $"where constraint_catalog = '{model.Config.BLDatabase}'\n".Indent("    ");
                    code += "and constraint_schema = 'dbo'\n".Indent("    ");
                    code += $"and constraint_name = '{constraintName}'\n".Indent("    ");
                    code += ")\n";
                    code += $"alter table {attr.ParentInterface.FullName} drop constraint {constraintName};\n\n";
                }
            }

            return code;
        }
    }
}