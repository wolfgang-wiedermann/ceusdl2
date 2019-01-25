using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.AL;
using KDV.CeusDL.Model.AL.Star;
using System.Text;

namespace KDV.CeusDL.Generator.AL.Star {
    public class CopyStarALGenerator : IGenerator
    {
        private StarALModel model;

        public CopyStarALGenerator(CoreModel model) {
            this.model = new StarALModel(model);
        }

        public CopyStarALGenerator(StarALModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("AL_Star_CopyToDb.sql", GenerateCreateTables()));            
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
            sb.Append($"truncate table {ifa.Model.Config.ALDatabase}.dbo.{ifa.Name} \n");
            sb.Append($"insert into {ifa.Model.Config.ALDatabase}.dbo.{ifa.Name} (\n");
            foreach (var a in ifa.Attributes)
            {
                sb.Append($"{a.Name}".Indent(1));           
                if (a != ifa.Attributes.Last())
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append(") select \n");
            foreach (var a in ifa.Attributes)
            {
                sb.Append($"{a.Name}".Indent(1));           
                if (a != ifa.Attributes.Last())
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append($"from [{ifa.Model.Config.EtlDbServer}].{ifa.Model.Config.ALDatabase}.dbo.{ifa.Name};\n");            
            sb.Append("\n");
        }

        private void GenerateDimensionInterface(StringBuilder sb, StarDimensionTable ifa)
        {
            sb.Append($"truncate table {ifa.Config.ALDatabase}.dbo.{ifa.Name} \n");
            sb.Append($"insert into {ifa.Config.ALDatabase}.dbo.{ifa.Name} (\n");
            foreach (var a in ifa.Attributes)
            {
                sb.Append($"{a.Name}".Indent(1));           
                if (a != ifa.Attributes.Last())
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append(") select \n");
            foreach (var a in ifa.Attributes)
            {
                sb.Append($"{a.Name}".Indent(1));           
                if (a != ifa.Attributes.Last())
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append($"from [{ifa.Config.EtlDbServer}].{ifa.Config.ALDatabase}.dbo.{ifa.Name};\n");            
            sb.Append("\n");
        }

        private void GenerateUseStatement(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.ALDatabase)) {
                sb.Append($"use {model.Config.ALDatabase};\n\n");
            }
        }
    }
}