using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BT;
using System.Text;

namespace KDV.CeusDL.Generator.MySql.BT {
    public class CreateBTGenerator : IGenerator
    {
        private BTModel model;

        public CreateBTGenerator(CoreModel model) {
            this.model = new BTModel(model);
        }

        public CreateBTGenerator(BTModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode()
        {            
            List<GeneratorResult> result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BT_Create.sql", GenerateCreateTables()));
            return result;
        }

        private string GenerateCreateTables() {
            StringBuilder sb = new StringBuilder();
            sb.Append("--\n-- Tabellen für BaseLayer Transformation (BT) anlegen\n--\n\n");
            CreateDatabase(sb);
            CreateUsing(sb);
            foreach(var ifa in model.Interfaces) {
                CreateCreateTable(sb, ifa);
            }
            return sb.ToString();
        }

        private void CreateUsing(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.BTDatabase)) {
                sb.Append($"use {model.Config.BTDatabase};\n\n");
            }
        }

        private void CreateDatabase(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.BTDatabase)) {
                sb.Append($"create database if not exists {model.Config.BTDatabase};\n\n");
            }
        }
        
        private void CreateCreateTable(StringBuilder sb, BTInterface ifa)
        {
            if(ifa.IsHistoryTable) {
                sb.Append($"-- Historientabelle für {ifa.ShortName} anlegen\n");
            } else {
                sb.Append($"-- Tabelle für {ifa.ShortName} anlegen\n");
            }            
            
            sb.Append($"create table {ifa.FullName} (\n");
            
            foreach(var attr in ifa.Attributes) {
                if(attr is BaseBTAttribute) {
                    var baseAttr = (BaseBTAttribute)attr;
                    sb.Append($"{baseAttr.Name} {baseAttr.GetSqlDataTypeDefinition()}".Indent("    "));                    
                } else if(attr is RefBTAttribute) {
                    var refAttr = (RefBTAttribute)attr;
                    sb.Append($"{refAttr.IdAttribute.Name} {refAttr.IdAttribute.SqlDataType},\n".Indent("    "));
                    sb.Append($"{refAttr.KnzAttribute.Name} {refAttr.KnzAttribute.SqlDataType}".Indent("    "));
                }
                if(attr != ifa.Attributes.Last()) {
                    sb.Append(",\n");
                } else {
                    sb.Append("\n");
                }
            }

            sb.Append(");\n\n");
        }
    }
}