using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.AL;
using KDV.CeusDL.Model.AL.Star;
using System.Text;

namespace KDV.CeusDL.Generator.AL.Star {
    public class CreateMicrostrategyCmdStarGenerator : IGenerator
    {
        private StarALModel model;

        public CreateMicrostrategyCmdStarGenerator(CoreModel model) {
            this.model = new StarALModel(model);
        }

        public CreateMicrostrategyCmdStarGenerator(StarALModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("MSI_CMD_CreateFacts.scp", GenerateCreateFacts()));            
            result.Add(new GeneratorResult("MSI_CMD_CreateAttributes.scp", GenerateCreateAttributes()));
            //result.Add(new GeneratorResult("MSI_CMD_CreateMetrics.scp", GenerateCreateMetrics()));
            return result;
        }

        private string GenerateCreateFacts()
        {            
            var sb = new StringBuilder();
            foreach(var fi in model.FactInterfaces) {
                foreach(var fa in fi.Attributes.Where(a => a.IsFact)) {
                    sb.Append($"CREATE FACT \"{fa.ParentInterface.Core.Name} - {fa.Core.Name}\" IN FOLDER \"\\Schemaobjekte\\Fakten\" EXPRESSION \"{fa.Name}\" FOR PROJECT \"PROJECTNAME_TO_REPLACE\";\n");
                }
            }            
            return sb.ToString();
        }

        /*
        Beispiel:
        CREATE ATTRIBUTE "Month" 
        IN FOLDER "\Schema Objects\Attributes" 
        ATTRIBUTEFORM "ID" FORMDESC "Basic ID form" FORMTYPE TEXT 
        SORT ASC EXPRESSION "[MONTH_ID]" LOOKUPTABLE "LU_MONTH" 
        FOR PROJECT "MicroStrategy Tutorial";
         */
        private string GenerateCreateAttributes() {
            var sb = new StringBuilder();
            foreach(var attrIfa in model.DimensionInterfaces) {
                sb.Append($"CREATE ATTRIBUTE \"{attrIfa.Core.Name}\"\n");
                sb.Append("IN FOLDER \"\\Schemaobjekte\\Attribute\"\n");
                foreach(var a in attrIfa.Attributes
                                        .Where(a1 => a1 is BaseALAttribute)
                                        .Where(a1 => a1.Name != "Mandant_ID")
                                        .Select(a1 => (BaseALAttribute)a1)) {

                    sb.Append($"ATTRIBUTEFORM \"{a.Name}\" ");
                    if(a == attrIfa.IdColumn) {
                        sb.Append("FORMCATEGORY \"ID\" ");
                        sb.Append("FORMTYPE NUMBER\n");
                    } else {
                        sb.Append($"FORMCATEGORY \"{a.Core.Name}\" ");

                        var t = ((CoreBaseAttribute)a.Core).DataType;
                        if(t == CoreDataType.INT || t == CoreDataType.DECIMAL) {
                            sb.Append("FORMTYPE NUMBER\n");
                        } else if(t == CoreDataType.DATE) {
                            sb.Append("FORMTYPE DATE\n");
                        } else if(t == CoreDataType.DATETIME) {
                            sb.Append("FORMTYPE DATETIME\n");
                        } else if(t == CoreDataType.TIME) {
                            sb.Append("FORMTYPE TIME\n");
                        } else {
                            sb.Append("FORMTYPE TEXT\n");
                        }
                    }
                }
                sb.Append($"SORT ASC EXPRESSION \"{attrIfa.IdColumn.Name}\"\n");
                sb.Append($"MAPPINGMODE AUTOMATIC\n");
                // DAS PASST BEI STAR NOCH NICHT!
                // DA MUSS IMMER DAS _1_ Dim-Interface rein!
                // Bei Snowflake müsste es passen
                sb.Append($"LOOKUPTABLE \"{attrIfa.Name}\"\n"); 
                sb.Append("FOR PROJECT \"PROJECTNAME_TO_REPLACE\";\n");
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}