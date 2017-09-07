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
            string code = "--\n-- BaseLayer \n--\n";

            // TODO: Das ist so natürlich noch nicht der Weisheit letzter Schluss
            //       Reihenfolge in Zukunft: 1. Def-Interfaces, 2. Dim-Interfaces, 3. Fact-Interfaces
            //       Intern sortiert nach MaxReferenceDepth aufsteigend.
            foreach(var ifa in model.Interfaces.Where(i => i.InterfaceType == CoreInterfaceType.DEF_TABLE || i.InterfaceType == CoreInterfaceType.TEMPORAL_TABLE)) {
                code += GenerateDefTable(ifa);                
            }

            foreach(var ifa in model.Interfaces.Where(i => i.InterfaceType == CoreInterfaceType.DIM_VIEW)) {
                code += "/*\n";
                code += GenerateDefTable(ifa).Indent(" * ");                
                code += " */\n\n";
            }

            foreach(var ifa in model.Interfaces.Where(i => i.InterfaceType == CoreInterfaceType.DIM_TABLE)) {                
                code += GenerateDefTable(ifa);                                
            }

            foreach(var ifa in model.Interfaces.Where(i => i.InterfaceType == CoreInterfaceType.FACT_TABLE)) {                
                code += GenerateDefTable(ifa);                                
            }

            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Create.sql", code));
            return result;
        }

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
    }
}