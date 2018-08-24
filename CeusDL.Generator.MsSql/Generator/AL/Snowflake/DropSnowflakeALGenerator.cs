using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.AL;
using KDV.CeusDL.Model.AL.Snowflake;
using System.Text;

namespace KDV.CeusDL.Generator.AL.Snowflake {
    public class DropSnowflakeALGenerator : IGenerator
    {
        private SnowflakeALModel model;

        public DropSnowflakeALGenerator(CoreModel model) {
            this.model = new SnowflakeALModel(model);
        }

        public DropSnowflakeALGenerator(SnowflakeALModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("AL_Snowflake_Drop.sql", GenerateCreateTables()));            
            return result;
        }

        private string GenerateCreateTables() {
            StringBuilder sb = new StringBuilder();
            GenerateUseStatement(sb);
            foreach(var ifa in model.DimensionInterfaces)
            {
                GenerateDropInterface(sb, ifa);
            }
            foreach(var ifa in model.FactInterfaces) {
                GenerateDropInterface(sb, ifa);
            }
            return sb.ToString();
        }
        
        private void GenerateDropInterface(StringBuilder sb, IALInterface ifa)
        {
            sb.Append($"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{ifa.Name}]') AND type in (N'U'))\n");
            sb.Append($"drop table {ifa.Name}\n".Indent(1));
            sb.Append("go\n\n");
        }

        private void GenerateUseStatement(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.ALDatabase)) {
                sb.Append($"use {model.Config.ALDatabase};\n\n");
            }
        }
    }
}