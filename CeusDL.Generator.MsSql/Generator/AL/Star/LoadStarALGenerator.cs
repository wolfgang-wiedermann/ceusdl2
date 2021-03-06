using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.AL;
using KDV.CeusDL.Model.AL.Star;
using System.Text;
using KDV.CeusDL.Model.BT;

namespace KDV.CeusDL.Generator.AL.Star {
    public class LoadStarALGenerator : IGenerator
    {
        private StarALModel model;

        public LoadStarALGenerator(CoreModel model) {
            this.model = new StarALModel(model);
        }

        public LoadStarALGenerator(StarALModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("AL_Star_Load.sql", GenerateCreateTables()));            
            return result;
        }

        private string GenerateCreateTables() {
            StringBuilder sb = new StringBuilder();
            GenerateUseStatement(sb);
            // Löschen
            foreach(var ifa in model.FactInterfaces) {
                GenerateTruncateFactInterface(sb, ifa);
            }
            foreach(var ifa in model.StarDimensionTables)
            {
                GenerateTruncateDimInterface(sb, ifa);
            }
            // Laden
            foreach(var ifa in model.StarDimensionTables)
            {
                GenerateLoadDimInterface(sb, ifa);
            }
            foreach(var ifa in model.FactInterfaces) {
                GenerateLoadFactInterface(sb, ifa);
            }
            foreach(var ifa in model.FactInterfaces.Where(i => i.IsWithNowTable)) {
                GenerateLoadNowTable(sb, ifa);
            }
            return sb.ToString();
        }

        private void GenerateTruncateDimInterface(StringBuilder sb, StarDimensionTable ifa)
        {
            sb.Append($"-- Inhalt von {ifa.Name} löschen\n");
            sb.Append($"truncate table {ifa.Name};\n\n");
        }

        private void GenerateTruncateFactInterface(StringBuilder sb, FactALInterface ifa)
        {
            sb.Append($"-- Inhalt von {ifa.Name} löschen\n");
            sb.Append($"truncate table {ifa.Name};\n\n");
        }

        private void GenerateLoadDimInterface(StringBuilder sb, StarDimensionTable ifa)
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
                sb.Append($"{attr.JoinAlias}.{GetBTName(attr.BTAttribute)} as {attr.Name}".Indent(1));
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append($"from {ifa.MainBTInterface.FullName} as t0\n");

            // Referenzierte/untergeordnete Dimensionen joinen
            foreach(var ifaRef in ifa.InterfaceReferences) {
                sb.Append($"left join {ifaRef.ReferencedBTInterface.FullName} as {ifaRef.JoinAlias}\n");
                sb.Append($"on {ifaRef.JoinAlias}.{ifaRef.ReferencedRefColumnName} = {ifaRef.ParentJoinAlias}.{ifaRef.ParentRefColumnName}\n".Indent(1));
            }
            sb.Append("\n");            
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

        private void GenerateLoadNowTable(StringBuilder sb, FactALInterface ifa) {
            sb.Append($"-- Load NowTable for FactInteface {ifa.Core.Name}\n");
            sb.Append($"truncate table {ifa.Name}_NOW;\n");
            sb.Append($"insert into {ifa.Name}_NOW (\n");
            foreach(var attr in ifa.Attributes) {
                if(attr.IsFact) {
                    sb.Append($"{attr.Name}_NOW".Indent(1));
                } else {
                    sb.Append($"{attr.Name}".Indent(1));
                }
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append(") \n");
            
            sb.Append("select\n");
            foreach(var attr in ifa.Attributes) {
                sb.Append($"{attr.Name}".Indent(1));
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append($"from {ifa.Name} a\n");
            if(ifa.Core.IsMandant) {
                sb.Append($"where a.{ifa.HistoryAttribute.Name} = (select max({ifa.HistoryAttribute.Name}) from {ifa.Name} as i where i.Mandant_ID = a.Mandant_ID)");
            } else {
                sb.Append($"where a.{ifa.HistoryAttribute.Name} = (select max({ifa.HistoryAttribute.Name}) from {ifa.Name})");
            }
            sb.Append(";\n\n");
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