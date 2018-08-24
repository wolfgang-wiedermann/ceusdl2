using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.AL;
using KDV.CeusDL.Model.AL.Star;
using System.Text;

namespace KDV.CeusDL.Generator.AL.Star {
    public class CreateStarALGenerator : IGenerator
    {
        private StarALModel model;

        public CreateStarALGenerator(CoreModel model) {
            this.model = new StarALModel(model);
        }

        public CreateStarALGenerator(StarALModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("AL_Star_Create.sql", GenerateCreateTables()));            
            return result;
        }

        private string GenerateCreateTables() {
            StringBuilder sb = new StringBuilder();
            GenerateUseStatement(sb);
            sb.Append("/*\n");
            foreach(var i in model.DimensionInterfaces) {
                sb.Append(i.Name);
                sb.Append("\n");
            }
            sb.Append("*/\n");
            return sb.ToString();
        }

        private void GenerateUseStatement(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.ALDatabase)) {
                sb.Append($"use {model.Config.ALDatabase};\n\n");
            }
        }
    }
}