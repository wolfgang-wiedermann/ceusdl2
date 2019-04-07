using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.AL;
using KDV.CeusDL.Model.MySql.AL.Star;
using System.Text;

namespace KDV.CeusDL.Generator.MySql.AL.Star {
    public class DropStarALGenerator : IGenerator
    {
        private StarALModel model;

        public DropStarALGenerator(CoreModel model) {
            this.model = new StarALModel(model);
        }

        public DropStarALGenerator(StarALModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("AL_Star_Drop.sql", GenerateDropTables()));            
            return result;
        }

        private string GenerateDropTables() {
            StringBuilder sb = new StringBuilder();
            GenerateUseStatement(sb);            
            foreach(var i in model.StarDimensionTables) {
                GenerateDimensionInterface(sb, i);                
            }
            foreach(var i in model.FactInterfaces) {
                GenerateFactInterface(sb, i);
                if(i.IsWithNowTable) {
                    GenerateNowInterface(sb, i);
                }
            }       
            return sb.ToString();
        }

        private void GenerateFactInterface(StringBuilder sb, FactALInterface ifa)
        {
            sb.Append($"drop table if exists {ifa.Name};\n");
        }

        private void GenerateNowInterface(StringBuilder sb, FactALInterface ifa)
        {
            sb.Append($"drop table if exists {ifa.Name}_NOW;\n");
        }

        private void GenerateDimensionInterface(StringBuilder sb, StarDimensionTable ifa)
        {
            sb.Append($"drop table if exists {ifa.Name};\n");
        }

        private void GenerateUseStatement(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.ALDatabase)) {
                sb.Append($"use {model.Config.ALDatabase};\n\n");
            }
        }
    }
}