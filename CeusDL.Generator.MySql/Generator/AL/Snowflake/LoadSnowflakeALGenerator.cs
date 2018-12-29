using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.AL;
using KDV.CeusDL.Model.MySql.AL.Snowflake;
using System.Text;
using KDV.CeusDL.Model.MySql.BT;

namespace KDV.CeusDL.Generator.MySql.AL.Snowflake {
    public class LoadSnowflakeALGenerator : IGenerator
    {
        private SnowflakeALModel model;

        public LoadSnowflakeALGenerator(CoreModel model) {
            this.model = new SnowflakeALModel(model);
        }

        public LoadSnowflakeALGenerator(SnowflakeALModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("AL_Snowflake_Load.sql", GenerateCreateTables()));            
            return result;
        }

        private string GenerateCreateTables() {
            StringBuilder sb = new StringBuilder();
            GenerateUseStatement(sb);
            // Löschen
            foreach(var ifa in model.FactInterfaces) {
                GenerateTruncateFactInterface(sb, ifa);
            }
            foreach(var ifa in model.DimensionInterfaces)
            {
                GenerateTruncateDimInterface(sb, ifa);
            }
            // Laden
            foreach(var ifa in model.DimensionInterfaces)
            {
                GenerateLoadDimInterface(sb, ifa);
            }
            foreach(var ifa in model.FactInterfaces) {
                GenerateLoadFactInterface(sb, ifa);
            }
            return sb.ToString();
        }

        private void GenerateTruncateDimInterface(StringBuilder sb, DimensionALInterface ifa)
        {
            sb.Append($"-- Inhalt von {ifa.Name} löschen\n");
            sb.Append($"truncate table {ifa.Name};\n\n");
        }

        private void GenerateTruncateFactInterface(StringBuilder sb, FactALInterface ifa)
        {
            sb.Append($"-- Inhalt von {ifa.Name} löschen\n");
            sb.Append($"truncate table {ifa.Name};\n\n");
        }

        private void GenerateLoadDimInterface(StringBuilder sb, DimensionALInterface ifa)
        {
            sb.Append($"-- Load DimensionInterface {ifa.Name}\n");            
            sb.Append($"insert into {ifa.Name} (\n");
            foreach(var attr in ifa.Attributes) {
                sb.Append($"{attr.Name}".Indent(1));
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append(") \n");
            sb.Append("select \n");
            foreach(var attr in ifa.Attributes) {
                sb.Append($"{GetBTName(attr.BTAttribute)} as {attr.Name}".Indent(1));
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append($"from {ifa.BTInterface.FullName}\n\n");
        }

        private void GenerateLoadFactInterface(StringBuilder sb, FactALInterface ifa)
        {
            sb.Append($"-- Load FactInterface {ifa.Name}\n");
            sb.Append($"insert into {ifa.Name} (\n");
            foreach(var attr in ifa.Attributes) {
                sb.Append($"{attr.Name}".Indent(1));
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append(") \n");
            // TODO: select-Aliase für die verschiedenen BT-Tabellen verwenden
            sb.Append("select \n");
            foreach(var attr in ifa.Attributes) {
                sb.Append($"{attr.JoinAlias}.{GetBTName(attr.BTAttribute)} as {attr.Name}".Indent(1));
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }            
            sb.Append($"from {ifa.BTInterface.FullName} as t0\n");          
            // referenzierte BT-Fakttabellen joinen
            foreach(var fref in ifa.FactInterfaceReferences) {
                sb.Append($"inner join {fref.BTInterface.FullName} as {fref.JoinAlias}\n".Indent(1));
                sb.Append($"on t0.{fref.RefColumnName} = {fref.JoinAlias}.{fref.RefColumnName}".Indent(1));
            }
            sb.Append("\n\n");
        }

        private void GenerateUseStatement(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.ALDatabase)) {
                sb.Append($"use {model.Config.ALDatabase};\n\n");
            }
        }

        private object GetBTName(IBTAttribute btAttribute)
        {
            if(btAttribute is RefBTAttribute) {
                var refAttr = (RefBTAttribute)btAttribute;
                return refAttr.IdAttribute.Name;
            } else if(btAttribute is BaseBTAttribute) {
                var baseAttr = (BaseBTAttribute)btAttribute;
                return baseAttr.Name;
            } else {
                throw new InvalidAttributeTypeException();
            }
        }
    }
}