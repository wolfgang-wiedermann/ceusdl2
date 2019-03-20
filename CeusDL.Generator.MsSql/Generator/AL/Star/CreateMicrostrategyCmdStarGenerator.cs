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
            //result.Add(new GeneratorResult("MSI_CMD_CreateAttributes.scp", GenerateCreateAttributes()));
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
    }
}