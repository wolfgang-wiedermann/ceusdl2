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
            foreach(var ifa in model.Interfaces) {
                if(ifa.InterfaceType == CoreInterfaceType.DEF_TABLE || ifa.InterfaceType == CoreInterfaceType.TEMPORAL_TABLE) {
                    code += GenerateDefTable(ifa);
                }
            }

            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Create.sql", code));
            return result;
        }

        private string GenerateDefTable(IBLInterface ifa)
        {
            string code = $"create table {ifa.FullName} (\n";
            foreach(var attr in ifa.Attributes) {
                code += GenerateAttribute(attr).Indent("    ");
                code += "\n";
            }
            code += ")\n\n";
            return code;
        }

        private string GenerateAttribute(IBLAttribute attr)
        {
            string code = $"{attr.Name} {attr.GetSqlDataTypeDefinition()}";
            if(model.Interfaces.Last() != attr) {
                code += ", ";
            }            
            return code;
        }
    }
}