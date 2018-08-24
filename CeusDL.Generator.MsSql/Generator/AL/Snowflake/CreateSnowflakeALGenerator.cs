using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.AL;
using KDV.CeusDL.Model.AL.Star;
using System.Text;

namespace KDV.CeusDL.Generator.AL.Snowflake {
    public class CreateSnowflakeALGenerator : IGenerator
    {
        private StarALModel model;

        public CreateSnowflakeALGenerator(CoreModel model) {
            this.model = new StarALModel(model);
        }

        public CreateSnowflakeALGenerator(StarALModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("AL_Snowflake_Create.sql", GenerateCreateTables()));            
            return result;
        }

        private string GenerateCreateTables() {
            StringBuilder sb = new StringBuilder();
            GenerateUseStatement(sb);
            foreach(var i in model.DimensionInterfaces) {
                sb.Append($"create table {i.Name} (\n");
                foreach(var a in i.Attributes) {
                    sb.Append($"{a.Name} {a.SqlType}".Indent(1));
                    if(a == i.IdColumn) {
                        sb.Append(" primary key not null");
                    }
                    if(a != i.Attributes.Last()) {
                        sb.Append(",");
                    }
                    sb.Append("\n");
                }
                sb.Append(");\n");
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private void GenerateUseStatement(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.ALDatabase)) {
                sb.Append($"use {model.Config.ALDatabase};\n\n");
            }
        }
    }
}