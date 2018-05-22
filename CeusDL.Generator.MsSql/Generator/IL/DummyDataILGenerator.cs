using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;
using System.Text;

namespace KDV.CeusDL.Generator.IL {
    public class DummyDataILGenerator : IGenerator
    {
        private ILModel model;
        const int MAX_NUM = 10;

        public DummyDataILGenerator(CoreModel model) {
            this.model = new ILModel(model);
        }

        public List<GeneratorResult> GenerateCode()
        {
            var result = new List<GeneratorResult>();            

            foreach(var ifa in model.Interfaces.Where(i => i.IsILRelevant())) {                
                result.Add(new GeneratorResult($"{ifa.ShortName}.csv", GenerateDummyData(ifa)));
            }
                                    
            return result;
        }

        private string GenerateDummyData(ILInterface ifa)
        {
            StringBuilder sb = new StringBuilder();
            GenerateHeadline(ifa, sb);
            for(int i = 0; i < MAX_NUM; i++) {
                GenerateDataLine(ifa, sb, i);
            }
            return sb.ToString();
        }

        private void GenerateDataLine(ILInterface ifa, StringBuilder sb, int i)
        {
            foreach(var attr in ifa.NonCalculatedAttributes) {
                switch(attr.CDataType) {
                    case CoreDataType.INT:
                        sb.Append($"\"{i}\"");
                        break;
                    case CoreDataType.DECIMAL:
                        sb.Append($"\"{i}.{i}\"");        
                        break;
                    case CoreDataType.DATE:
                        sb.Append("\"2012-01-01\"");
                        break;
                    case CoreDataType.DATETIME:
                        sb.Append("\"2013-01-01 12:00:00\"");
                        break;
                    case CoreDataType.TIME:
                        sb.Append("\"12:11:22\"");
                        break;
                    case CoreDataType.VARCHAR:
                        if(attr.Name.Equals("Mandant_KNZ")) {
                            sb.Append("\"9999\"");
                        } else {
                            sb.Append($"\"TXT{i}\"");
                        }
                        break;
                    default:
                        throw new InvalidDataTypeException();
                }                
                if(ifa.NonCalculatedAttributes.Last() != attr) {
                    sb.Append(";");
                } else {
                    sb.Append("\n");
                }
            }
        }

        private void GenerateHeadline(ILInterface ifa, StringBuilder sb)
        {
            foreach(var attr in ifa.NonCalculatedAttributes) {
                sb.Append($"\"{attr.Name}\"");
                if(ifa.NonCalculatedAttributes.Last() != attr) {
                    sb.Append(";");
                } else {
                    sb.Append("\n");
                }
            }
        }
    }
}