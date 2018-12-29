using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.IL;
using System.Text;

namespace KDV.CeusDL.Generator.MySql.IL {
    public class DummyILPythonGenerator : IGenerator
    {
        private ILModel model;

        public DummyILPythonGenerator(CoreModel model) {
            this.model = new ILModel(model);
        }

        public List<GeneratorResult> GenerateCode()
        {        
            var result = new List<GeneratorResult>();
            foreach(var ifa in model.Interfaces.Where(i => i.IsILRelevant())) {  
                result.Add(new GeneratorResult($"{ifa.Name}.py", GenerateIfaPythonCode(ifa)));
            }
            return result;
        }

        private string GenerateIfaPythonCode(ILInterface ifa) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"#!/usr/bin/python\n#\n# CSV-Generator fuer {ifa.FullName}\n#\n\n");

            sb.Append("def print_line(");
            foreach(var attr in ifa.NonCalculatedAttributes) {
                sb.Append(attr.Name);
                if(attr != ifa.NonCalculatedAttributes.Last()) {
                    sb.Append(", ");
                } 
            }
            sb.Append("):\n");
            sb.Append($"line = ''\n".Indent("    "));
            foreach(var attr in ifa.NonCalculatedAttributes) {
                if(attr != ifa.NonCalculatedAttributes.Last()) {
                    sb.Append($"line += '\"{{0}}\";'.format({attr.Name})\n".Indent("    "));
                } else {
                    sb.Append($"line += '\"{{0}}\"'.format({attr.Name})\n".Indent("    "));
                }
            }
            sb.Append("return line\n".Indent("    "));
            sb.Append("\n\n");

            sb.Append("print('");
            foreach(var attr in ifa.NonCalculatedAttributes) {
                sb.Append($"\"{attr.Name}\"");
                if(attr != ifa.NonCalculatedAttributes.Last()) {
                    sb.Append(";");
                }
            }
            sb.Append("')\n# TODO: hier den eigenen Code einfügen\n\n");

            sb.Append("# Beispiel:\n");
            sb.Append("print(print_line(");
            foreach(var attr in ifa.NonCalculatedAttributes) {
                switch(attr.CDataType) {
                    case CoreDataType.INT:
                        sb.Append("-1");
                        break;
                    case CoreDataType.VARCHAR:
                        if(attr.Name.Contains("BEZ") || attr.Name.Contains("DESC")) {
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
                if(attr != ifa.NonCalculatedAttributes.Last()) {
                    sb.Append(", ");
                }
            }
            sb.Append("))\n");


            return sb.ToString();
        }
    }
}