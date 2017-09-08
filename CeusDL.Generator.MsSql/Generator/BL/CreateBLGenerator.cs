using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;

namespace KDV.CeusDL.Generator.BL {
    public class CreateBLGenerator : IGenerator
    {
        private BLModel model;

        public CreateBLGenerator(CoreModel model) {
            this.model = new BLModel(model);
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Create.sql", GenerateCreateTables()));
            result.Add(new GeneratorResult("BL_Create_FKs.sql", GenerateAllForeignKeyConstraints()));
            return result;
        }

        private string GenerateCreateTables() {
            string code = "--\n-- BaseLayer \n--\n";
            
            if(!string.IsNullOrEmpty(model.Config.BLDatabase)) {
                code += $"\nuse {model.Config.BLDatabase};\n\n";
            }

            // TODO: Das ist so natürlich noch nicht der Weisheit letzter Schluss
            //       Reihenfolge in Zukunft: Intern sortiert nach MaxReferenceDepth aufsteigend.
            foreach(var ifa in model.DefTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {
                code += GenerateDefTable(ifa);                
            }

            foreach(var ifa in model.DimViewInterfaces.OrderBy(i => i.MaxReferenceDepth)) {
                code += "/*\n";
                code += GenerateDefTable(ifa).Indent(" * ");                
                code += " */\n\n";
            }

            foreach(var ifa in model.DimTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {                
                code += GenerateDefTable(ifa);  
                //TODO: code += GenerateDimView(ifa);                              
            }

            foreach(var ifa in model.FactTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {                
                code += GenerateDefTable(ifa);                                
                //TODO: code += GenerateFactView(ifa);
            }
            
            return code;
        }

        // Wird derzeit nicht nur für DefTables verwendet sondern für alle!!!
        // d.h. sollte evtl. umbenannt werden.
        private string GenerateDefTable(IBLInterface ifa)
        {
            // Create Table
            string code = $"create table {ifa.FullName} (\n";
            foreach(var attr in ifa.Attributes) {
                code += GenerateAttribute(attr, ifa).Indent("    ");
                code += "\n";
            }
            code += ");\n\n";

            // Create Unique-Key-Constraint
            code += $"alter table {ifa.FullName} \n";
            code += $"add constraint {ifa.Name}_UK unique nonclustered (\n";
            foreach(var attr in ifa.UniqueKeyAttributes) {
                code += $"    {attr.Name} ASC";
                if(ifa.UniqueKeyAttributes.Last() != attr) {
                    code += ",";
                }
                code += "\n";
            }            
            code += ");\n\n";

            return code;
        }

        private string GenerateAttribute(IBLAttribute attr, IBLInterface ifa)
        {
            string code = $"{attr.Name} {attr.GetSqlDataTypeDefinition()}";
            if(ifa.Attributes.Last() != attr) {
                code += ", ";
            }            
            return code;
        }

        // Alle Foreign-Key-Constraints für die Interfaces des Models generieren
        private string GenerateAllForeignKeyConstraints()
        {
            string code = "-- TODO: Generierung der FK-Constraints umsetzen\n";
            foreach(var ifa in model.Interfaces.Where(i => i.Attributes.Where(a => a is RefBLAttribute).Count() > 0)) {
                code += GenerateForeignKeyConstraints(ifa);
            }
            return code;
        }

        // Foreign-Key-Constraints für ein einzelnes Interface generieren
        private string GenerateForeignKeyConstraints(IBLInterface ifa)
        {
            string code = $"-- FKs von {ifa.FullName}\n--\n\n";
            // Nur die Ref-Attribute durchlaufen
            foreach(var attr in ifa.Attributes.Where(a => a is RefBLAttribute)) {
                var refAttr = (RefBLAttribute)attr;                
                // Wir legen keine FKs für Beziehungen zwischen Faktentabellen an, das bremst zu viel
                // außerdem legen wir auch keine FKs zu DimViews an.
                if(refAttr.ReferencedAttribute.ParentInterface.InterfaceType != CoreInterfaceType.FACT_TABLE
                    && refAttr.ReferencedAttribute.ParentInterface.InterfaceType != CoreInterfaceType.DIM_VIEW) {
                    code += GenerateSingleForeignKeyConstraint(refAttr);
                }
            }
            code += "\n";
            return code;
        }

        // Einzelnes Foreign-Key-Constraint anlegen
        private string GenerateSingleForeignKeyConstraint(RefBLAttribute attr)
        {                        
            string code = $"alter table {attr.ParentInterface.FullName}\n";
            code += $"add constraint {attr.ParentInterface.Name}_{attr.Name}_FK\n";
            // TODO: Wenn das Ganze mit Mandant ist dann kommt hier der Mandant mit rein, 
            //       und bei Historisierung zusätzlich noch das Historisierungsattribut ... 
            //       (wenn auf beiden Seiten unterstützt!)
            if(attr.ParentInterface.IsMandant && attr.ReferencedAttribute.ParentInterface.IsMandant) {
                var attrNames = new List<string>();
                attrNames.Add("Mandant_KNZ");
                attrNames.Add(attr.Name);                                

                code += "foreign key (";
                foreach(var attrName in attrNames) {
                    code += attrName;
                    if(attrName != attrNames.Last()) {
                        code += ", ";
                    }
                }

                code += ")\n"; 
                
                var attrNames2 = attr.ReferencedAttribute.ParentInterface.UniqueKeyAttributes.Select(a => a.Name);
                code += $"references {attr.ReferencedAttribute.ParentInterface.Name} (";                
                foreach(var attrName in attrNames2) {
                    code += attrName;
                    if(attrName != attrNames2.Last()) {
                        code += ", ";
                    }
                }                
                code += ");\n\n";
            } else {
                code += $"foreign key ({attr.Name})\n"; 
                code += $"references {attr.ReferencedAttribute.ParentInterface.Name} ({attr.ReferencedAttribute.Name});\n\n";
            }            
            return code;
        }
    }
}