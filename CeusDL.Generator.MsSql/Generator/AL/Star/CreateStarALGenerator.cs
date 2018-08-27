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
            foreach(var i in model.StarDimensionTables) {
                GenerateDimensionInterface(sb, i);                
            }
            foreach(var i in model.FactInterfaces) {
                GenerateFactInterface(sb, i);
            }       
            return sb.ToString();
        }

        private void GenerateFactInterface(StringBuilder sb, FactALInterface ifa)
        {
            sb.Append($"create table {ifa.Name} (\n");
            foreach (var a in ifa.Attributes)
            {
                sb.Append($"{a.Name} {a.SqlType}".Indent(1));
                if (a == ifa.IdColumn)
                {
                    sb.Append(" primary key not null");
                }           
                if (a != ifa.Attributes.Last())
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }            
            sb.Append(");\n");
            sb.Append("\n");
        }

        private void GenerateDimensionInterface(StringBuilder sb, StarDimensionTable i)
        {
            sb.Append($"create table {i.Name} (\n");
            foreach (var a in i.Attributes)
            {
                sb.Append($"{a.Name} {a.SqlType}".Indent(1));
                if (a == i.IdColumn)
                {
                    sb.Append(" primary key not null");
                }                
                if (a != i.Attributes.Last())
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append(");\n");
            sb.Append("\n");
        }

        private void GenerateUseStatement(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.ALDatabase)) {
                sb.Append($"use {model.Config.ALDatabase};\n\n");
            }
        }
    }
}