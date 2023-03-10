using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BT;
using System.Text;

namespace KDV.CeusDL.Generator.BT {
    public class LoadBTGenerator : IGenerator
    {
        private BTModel model;

        public LoadBTGenerator(CoreModel model) {
            this.model = new BTModel(model);
        }

        public LoadBTGenerator(BTModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode()
        {            
            List<GeneratorResult> result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BT_Load.sql", GenerateLoadSkript()));
            return result;
        }

        private string GenerateLoadSkript()
        {
            var sb = new StringBuilder();
            sb.Append("--\n-- Laden der Base Layer Transformation\n--\n\n");
            CreateUsing(sb);
            foreach(var ifa in model.Interfaces) {
                GenerateTruncate(sb, ifa);
                GenerateInsert(sb, ifa);
            }            
            return sb.ToString();
        }

        private void GenerateInsert(StringBuilder sb, BTInterface ifa)
        {            
            var refAttributes = ifa.Attributes.Where(a => a is RefBTAttribute)
                                              .Select(a => (RefBTAttribute)a)
                                              .ToList<RefBTAttribute>();

            sb.Append($"insert into {ifa.FullName} (\n");
            foreach(var attr in ifa.Attributes) {
                if(attr is BaseBTAttribute) {
                    var baseAttr = (BaseBTAttribute)attr;
                    sb.Append($"{baseAttr.Name}".Indent("    "));
                } else {
                    var refAttr = (RefBTAttribute)attr;
                    sb.Append($"{refAttr.IdAttribute.Name},\n{refAttr.KnzAttribute.Name}".Indent("    "));
                }
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append(")\nselect\n");
            foreach(var attr in ifa.Attributes) {
                if(attr is BaseBTAttribute) {
                    var baseAttr = (BaseBTAttribute)attr;
                    sb.Append($"t.{baseAttr.blAttribute.Name} as {baseAttr.Name}".Indent("    "));
                } else {
                    var refAttr = (RefBTAttribute)attr;
                    var idAttr = refAttr.ReferencedBLInterface.Attributes.Single(a => a.IsIdentity);
                    sb.Append($"coalesce({refAttr.JoinAlias}.{idAttr.Name}, -1) as {refAttr.IdAttribute.Name},\nt.{attr.GetBLAttribute().Name} as {refAttr.KnzAttribute.Name}".Indent("    "));
                }
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",");
                }
                sb.Append("\n");
            }
            sb.Append($"from {ifa.blInterface.FullName} as t\n");
            foreach(var attr in refAttributes) {
                sb.Append($"left join {attr.ReferencedBLInterface.FullName} as {attr.JoinAlias}\n");                
                sb.Append($"on t.{attr.blAttribute.Name} = {attr.JoinAlias}.{attr.ReferencedBLAttribute.Name}\n".Indent("    "));
                if(ifa.IsMandant && attr.ReferencedBLInterface.IsMandant) {
                    sb.Append($"and t.Mandant_KNZ = {attr.JoinAlias}.Mandant_KNZ\n".Indent("    "));
                }
                if(ifa.blInterface.IsHistorized && attr.ReferencedBLInterface.IsHistorized) {
                    GenerateHistoryCondition(sb, attr, ifa);
                }
            }   
            sb.Append("\n"); 
        }

        private void GenerateHistoryCondition(StringBuilder sb, RefBTAttribute attr, BTInterface ifa)
        {            
            if(ifa.InterfaceType == CoreInterfaceType.FACT_TABLE 
                && attr.ReferencedBTInterface.InterfaceType == CoreInterfaceType.FACT_TABLE) {
                // Beziehung zwischen zwei historisierten Fakt-Tabellen
                GenerateF2FHistoryCondition(sb, attr);
            } else if(ifa.InterfaceType == CoreInterfaceType.FACT_TABLE && attr.ReferencedBTInterface.IsHistoryTable) {
                // Beziehung zwischen einer historisierten Fakt-Tabelle und eienr historisierten Dimension
                GenerateF2DHistoryCondition(sb, attr);
            } else if(attr.ReferencedBTInterface.IsHistoryTable && ifa.IsHistoryTable) {
                // Beziehung zwischen zwei historisierten Dimensionen
                GenerateF2DHistoryCondition(sb, attr); // Durch die kaskadierung der Versionen in BL sollte das jetzt gehen.
            }
        }

        private void GenerateF2DHistoryCondition(StringBuilder sb, RefBTAttribute attr)
        {
            var max = BL.LoadBLGenerator.GetMaxValueForHistoryAttribute(attr.ReferencedBLInterface.HistoryAttribute);
            var idColumn = attr.ReferencedBLInterface.Attributes.Single(a => a.IsIdentity);

            sb.Append($"and coalesce({attr.JoinAlias}.{attr.ReferencedBLInterface.HistoryAttribute.Name}, '{max}') = (\n".Indent(1));
            sb.Append($"select min(coalesce(tx.{attr.ReferencedBLInterface.HistoryAttribute.Name}, '{max}'))\n".Indent(2));
            sb.Append($"from {attr.ReferencedBLInterface.FullName} tx\n".Indent(2));
            sb.Append($"where tx.{attr.ReferencedBLAttribute.Name} = {attr.JoinAlias}.{attr.ReferencedBLAttribute.Name}\n".Indent(2));
            if(attr.ParentInterface.IsMandant && attr.ReferencedBLInterface.IsMandant) {
                sb.Append($"and tx.Mandant_KNZ = {attr.JoinAlias}.Mandant_KNZ\n".Indent(2));
            }
            // TODO: 99991231 noch durch Typbezogene Werte wie in BL beim Cascade ersetzen!!
            sb.Append($"and coalesce(tx.{attr.ReferencedBLInterface.HistoryAttribute.Name}, '{max}') > ".Indent(2));
            sb.Append($"coalesce(t.{attr.ParentInterface.blInterface.HistoryAttribute.Name}, '{max}')\n");            
            sb.Append(")\n".Indent(1));
        }

        private void GenerateF2FHistoryCondition(StringBuilder sb, RefBTAttribute attr)
        {            
            sb.Append($"and t.{attr.ReferencedBLInterface.HistoryAttribute.Name} = {attr.JoinAlias}.{attr.ParentInterface.blInterface.HistoryAttribute.Name}\n".Indent("    "));                        
        }        

        private void GenerateTruncate(StringBuilder sb, BTInterface ifa)
        {
            sb.Append($"truncate table {ifa.FullName};\n\n");
        }

        private void CreateUsing(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.BTDatabase)) {
                sb.Append($"use {model.Config.BTDatabase}\n\n");
            }
        }
    }
}