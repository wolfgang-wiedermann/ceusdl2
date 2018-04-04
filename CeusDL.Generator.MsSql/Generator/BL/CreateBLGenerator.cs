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

            foreach(var ifa in model.DefTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {
                code += GenerateBLTable(ifa);
                code += GenerateUniqueKeyConstraint(ifa);             
            }

            foreach(var ifa in model.DimViewInterfaces.OrderBy(i => i.MaxReferenceDepth)) {
                code += "/*\n";
                code += GenerateBLTable(ifa).Indent(" * ");                
                code += " */\n\n";
            }

            foreach(var ifa in model.DimTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {                
                code += GenerateBLTable(ifa);  
                code += GenerateUniqueKeyConstraint(ifa);
                if(ifa.IsHistorized) {
                    code += GenerateHistorizedDimView(ifa);                    
                } else {
                    code += GenerateDimView(ifa);                              
                }
            }

            foreach(var ifa in model.FactTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {                
                code += GenerateBLTable(ifa);
                code += GenerateUniqueKeyConstraint(ifa);                     
                //TODO: code += GenerateFactView(ifa);
            }
            
            return code;
        }

        private string GenerateBLTable(IBLInterface ifa)
        {
            // Create Table
            string code = $"create table {ifa.FullName} (\n";
            foreach(var attr in ifa.Attributes) {
                code += GenerateAttribute(attr, ifa).Indent("    ");
                code += "\n";
            }
            code += ");\n\n";
            return code;
        }

        private string GenerateUniqueKeyConstraint(IBLInterface ifa) {
            // Create Unique-Key-Constraint
            var code = $"alter table {ifa.FullName} \n";
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
            string code = "--\n-- FK-Constraints\n--\n";

            if(!string.IsNullOrEmpty(model.Config.BLDatabase)) {
                code += $"\nuse {model.Config.BLDatabase};\n\n";
            }
            
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

        private string GenerateDimView(IBLInterface ifa) {            
            if(ifa.IsHistorized)
                throw new InvalidInterfaceTypeException("LOGICAL_ERROR: Die Methode GenerateDimView in CreateBLGenerator.cs ist nur für nicht historisierte DimTables vorgesehen");

            string code = $"go\ncreate view {ifa.Name}_VW as\n";
            code += $"select\n";
            foreach(var attr in ifa.Attributes) {
                if(!attr.IsTechnicalAttribute) {
                    var il = attr.GetILAttribute();

                    if(il != null) {                        
                        code += $"il.{il.Name} as {attr.Name},\n".Indent("    ");                
                    } else if(attr.IsPrimaryKey) {
                        code += $"bl.{attr.Name},\n".Indent("    ");                        
                    }                    
                }
            }

            // T_Modifikation berechnen
            // TODO: Berücksichtigung von _VERSION fehlt noch!
            var pk = ifa.PrimaryKeyAttributes?.First();            
            if(pk == null) 
                throw new InvalidParameterException($"{ifa.FullName} has no PrimaryKey-Attributes");

            code += $"case\n".Indent("    ");
            code += $"when bl.{pk.Name} is null then 'I'\n".Indent("        ");            

            var historyCheckAttrs = ifa.Attributes
                                .Where(a => !a.IsPrimaryKey 
                                       && !a.IsIdentity 
                                       && !a.IsPartOfUniqueKey 
                                       && !a.IsTechnicalAttribute);

            foreach(var attr in historyCheckAttrs) {
                if(historyCheckAttrs.First() == attr) {
                    code += $"when bl.{attr.Name} <> il.{attr.GetILAttribute().Name}\n".Indent("        ");                    
                } else {
                    code += $"  or bl.{attr.Name} <> il.{attr.GetILAttribute().Name}\n".Indent("        ");
                }
            }
            code += "then 'U'\nelse 'X'\n".Indent("        ");
            code += "end as T_Modifikation\n".Indent("    ");

            // Join generieren ...
            code += $"\nfrom {ifa.GetILInterface().FullName} as il \n";
            code += $"left outer join {ifa.FullName} as bl\n".Indent("    ");
            foreach(var attr in ifa.UniqueKeyAttributes) {
                if(attr == ifa.UniqueKeyAttributes.First()) {
                    code += $"  on il.{attr.Name} = bl.{attr.Name}\n".Indent("    "); // TODO: sollte eigentlich attr.GetILAttribute().Name sein, geht aber bei Mandant_KNZ nicht!
                } else {
                    code += $" and il.{attr.Name} = bl.{attr.Name}\n".Indent("    ");
                }
            }            
            code += ";\ngo\n\n";
            return code;
        }

        private string GenerateHistorizedDimView(IBLInterface ifa) {
            string code = "-- TODO: Historisierte View generieren\n\n";
            return code;
        }
    }
}