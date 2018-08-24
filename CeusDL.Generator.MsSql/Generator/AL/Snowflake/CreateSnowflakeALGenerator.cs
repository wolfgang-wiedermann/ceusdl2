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
            foreach(var ifa in model.DimensionInterfaces)
            {
                GenerateDimensionInterface(sb, ifa);
            }
            foreach(var ifa in model.FactInterfaces) {
                GenerateFactInterface(sb, ifa);
            }
            return sb.ToString();
        }

        ///
        /// TODO: Generierung von Fakt-Interfaces weiterentwickeln
        ///
        private void GenerateFactInterface(StringBuilder sb, FactALInterface ifa)
        {
            sb.Append($"create table {ifa.Name} (\n");
            foreach (var a in ifa.Attributes)
            {
                sb.Append($"{a.Name} {a.SqlType}".Indent(1));                
                if (a != ifa.Attributes.Last())
                {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            // TODO: Primary-Key-Spezifikation noch offen! -> evtl. sollte ich auch mehrelementige PKs unterstützen?
            //       aber kanns die überhaupt geben?
            sb.Append(");\n");
            sb.Append("\n");
        }

        private void GenerateDimensionInterface(StringBuilder sb, DimensionALInterface i)
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