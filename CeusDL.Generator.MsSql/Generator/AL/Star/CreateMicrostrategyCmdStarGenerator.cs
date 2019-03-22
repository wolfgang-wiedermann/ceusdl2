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
                    sb.Append($"CREATE FACT \"{fa.ParentInterface.Core.Name} - {fa.Core.Name}\"\n");
                    sb.Append($"  IN FOLDER \"\\Schemaobjekte\\Fakten\"\n");
                    sb.Append($"  EXPRESSION \"{fa.Name}\"\n");
                    sb.Append($"  FOR PROJECT \"PROJECTNAME_TO_REPLACE\";\n\n");
                }
            }            
            return sb.ToString();
        }

        private string GenerateCreateAttributes() {
            var sb = new StringBuilder();
            foreach(var attrIfa in model.DimensionInterfaces) {
                var sDim = model.StarDimensionTables.Where(s => s.ConstainsDimInterface(attrIfa)).SingleOrDefault();            

                if(sDim != null) {
                    var attrName = attrIfa.ShortName;                    
                    sb.Append($"// Attribut: {attrIfa.Name}\n");
                    // Attribut anlegen
                    sb.Append($"CREATE ATTRIBUTE \"{attrName}\"\n");
                    sb.Append( "  IN FOLDER \"\\Schemaobjekte\\Attribute\"\n");                    
                    sb.Append($"  ATTRIBUTEFORM \"{attrIfa.IdColumn.Name}\" ");                        
                    sb.Append("FORMCATEGORY \"ID\" ");
                    sb.Append("FORMTYPE NUMBER\n");                    
                    sb.Append($"  EXPRESSION \"{attrIfa.IdColumn.Name}\"\n");
                    sb.Append($"  MAPPINGMODE AUTOMATIC\n");                                    
                    sb.Append($"  LOOKUPTABLE \"{sDim.Name}\"\n"); 
                    sb.Append( "  FOR PROJECT \"PROJECTNAME_TO_REPLACE\";\n");
                    sb.Append("\n");

                    // Attributfelder hinzufügen
                    foreach(var a in attrIfa.Attributes
                                            .Where(a1 => a1 is BaseALAttribute)
                                            .Where(a1 => a1.Name != "Mandant_ID")
                                            .Where(a1 => a1 != attrIfa.IdColumn)
                                            .Select(a1 => (BaseALAttribute)a1)) {

                        sb.Append($"ADD ATTRIBUTEFORM \"{a.Core.Name}\" ");
                        sb.Append($"FORMCATEGORY \"{a.Core.Name}\"\n  ");

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

                        sb.Append($"  EXPRESSION \"{a.Name}\"\n");
                        sb.Append($"  LOOKUPTABLE \"{sDim.Name}\"\n");
                        sb.Append($"  TO ATTRIBUTE \"{attrName}\" IN FOLDER \"\\Schemaobjekte\\Attribute\"\n");
                        sb.Append( "  FOR PROJECT \"PROJECTNAME_TO_REPLACE\";\n");
                        sb.Append("\n");
                    }

                } else {
                    sb.Append($"// Die Dimension {attrIfa.Name} scheint in keiner StarDimension enthalten zu sein\n\n");
                }
            }
            return sb.ToString();
        }
    }
}