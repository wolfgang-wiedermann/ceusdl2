using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BL;
using System.Text;

namespace KDV.CeusDL.Generator.MySql.BL {
    public class CreateDefDataGenerator : IGenerator
    {
        private BLModel model;

        public CreateDefDataGenerator(CoreModel model) {
            this.model = new BLModel(model);
        }

        public CreateDefDataGenerator(BLModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            foreach(var def in model.DefTableInterfaces) {
                result.Add(new GeneratorResult($"{def.Name}.py", GenerateCreateData(def)));
            }            
            return result;
        }

        private string GenerateCreateData(IBLInterface def)
        {
            var sb = new StringBuilder();
            sb.Append("#!/usr/bin/python\n# -*- coding: utf-8 -*-\n#\n# @generator ceusdl\n");
            sb.Append($"# @interface {def.FullName}\n#\n");
            sb.Append($"TABLE_NAME = \"{def.Name}\"\n");
            sb.Append($"DATABASE_NAME = \"{def.DatabaseName}\"\n");
            sb.Append("SQL_TEMPLATE = \"\"\"\n");
            
            sb.Append($"insert into {def.FullName} (\n");
            var sortedAttributes = def.Attributes.OrderBy(a => a.SortId).ToList();
            foreach(var a in sortedAttributes) {
                sb.Append(a.Name.Indent("  "));
                if(a != sortedAttributes.Last()) {
                    sb.Append(", \n");
                }
            }
            sb.Append(") values (\n");
            for(int i = 0; i < sortedAttributes.Count(); i++) {
                var attr = sortedAttributes[i];

                switch(attr.Name) {
                    case "T_Benutzer":
                        sb.Append("SYSTEM_USER");
                        break;
                    case "T_System":
                        sb.Append("'PY'");
                        break;
                    case "T_Erst_Dat":
                    case "T_Aend_Dat":
                        sb.Append("GETDATE()");
                        break;
                    default:
                        if(attr.DataType == CoreDataType.INT || attr.DataType == CoreDataType.DECIMAL) {
                            sb.Append($"{{{i}}}");
                        } else {
                            sb.Append($"'{{{i}}}'");
                        }
                        break;
                }
                
                if(i < sortedAttributes.Count()-1) {
                    sb.Append(", ");
                }
            }
            sb.Append("\n)\n");

            sb.Append("\"\"\"\n\n");

            sb.Append($"\nprint(\"set identity_insert {def.FullName} on;\")\n\n");
            sb.Append("# Write your code here with for example a loop to create the content of your def table.\n\n");
            
            // Einen Beispielsatz mit -1 generieren
            sb.Append("print(SQL_TEMPLATE.format(");
            foreach(var a in sortedAttributes) {
                switch(a.DataType) {
                    case CoreDataType.INT:
                        sb.Append("-1");
                        break;
                    case CoreDataType.VARCHAR:
                        if(a.Name.Contains("BEZ") || a.Name.Contains("DESC")) {
                            sb.Append("'unbekannt'");
                        } else {
                            sb.Append("'-1'");
                        }
                        break;
                    case CoreDataType.DATE:
                    case CoreDataType.DATETIME:
                        sb.Append("'2000-01-01'");
                        break;
                    case CoreDataType.TIME:
                        sb.Append("'00:00:00'");
                        break;
                    case CoreDataType.DECIMAL:
                        sb.Append("0.0");
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if(a != sortedAttributes.Last()) {
                    sb.Append(", ");
                }
            }
            sb.Append("))\n");
            
            sb.Append($"\nprint(\"set identity_insert {def.FullName} off;\")\n");
            return sb.ToString();
        }
    }
}