using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KDV.CeusDL.Model.Core;

using static KDV.CeusDL.Model.Core.CoreInterfaceType;

namespace KDV.CeusDL.Generator.CeusDL 
{
    public class GraphvizCeusDLGenerator : IGenerator {

        private CoreModel model = null;
        private CeusDLGenerator helper = null;

        private bool generateSlim = false;

        public GraphvizCeusDLGenerator(CoreModel model, bool generateSlim) {
            this.generateSlim = generateSlim;
            this.model = model;            
            this.helper = new CeusDLGenerator(model);
        }

        public GraphvizCeusDLGenerator(CoreImport model, bool generateSlim) {
            this.generateSlim = generateSlim;      
            this.model = new CoreModel(model);  
            this.helper = new CeusDLGenerator(model);          
        }

        public List<GeneratorResult> GenerateCode()
        {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("CeusDL_Diagram.gv", GenerateCodeInternal()));   
            return result;
        }

        public string GenerateCodeInternal()
        {            
            StringBuilder sb = new StringBuilder();
            AppendTopDefinition(sb);

            foreach(var ifa in model.Interfaces) {
                GenerateInterface(sb, ifa);
            }

            foreach(var ifa in model.Interfaces) {
                GenerateRelations(sb, ifa);
            }

            sb.Append("}\n");
                   
            return sb.ToString();
        }

        private void GenerateInterface(StringBuilder sb, CoreInterface ifa) 
        {
            sb.Append($"{ifa.Name}[label=<\n");
            sb.Append("<table border=\"0\" cellborder=\"1\" cellspacing=\"0\">\n");
            sb.Append($"<tr><td align=\"left\"><b>{ifa.Name}:{InterfaceTypeToString(ifa.Type)}    </b></td></tr>\n");

            foreach(var attr in ifa.Attributes) {
                if(attr is CoreFactAttribute) {
                    var fattr = (CoreFactAttribute)attr;
                    string tmp = helper.GenerateFactAttribute(fattr, ifa, model);
                    if(generateSlim) {
                        tmp = $"fact {fattr.Name}";
                    } 
                    sb.Append($"<tr><td port=\"{attr.Name}\" align=\"left\">{tmp}</td></tr>\n");
                } else if(attr is CoreBaseAttribute) {
                    var battr = (CoreBaseAttribute)attr;
                    string tmp = helper.GenerateBaseAttribute(battr, ifa, model);
                    if(generateSlim) {
                        tmp = $"base {battr.Name}";
                    } 
                    sb.Append($"<tr><td port=\"{attr.Name}\" align=\"left\">{tmp}</td></tr>\n");
                } else if(attr is CoreRefAttribute) {
                    var rattr = (CoreRefAttribute)attr;
                    string tmp = helper.GenerateRefAttribute(rattr, ifa, model);
                    if(generateSlim) {
                        tmp = $"{rattr.ReferencedInterface.Name}.{rattr.ReferencedAttribute.Name}";
                        if(!string.IsNullOrEmpty(rattr.Alias)) {
                            tmp += $" as {rattr.Alias}";
                        }
                    } 
                    sb.Append($"<tr><td port=\"{attr.Name}\" align=\"left\">{tmp}</td></tr>\n");
                }
            }

            sb.Append("</table>>];\n\n");
        }

        private void GenerateRelations(StringBuilder sb, CoreInterface ifa) 
        {
            sb.Append("# Beziehungen \n");
            foreach(var r in ifa.Attributes.Where(a => a is CoreRefAttribute).Select(a => (CoreRefAttribute)a)) {
                sb.Append($"{ifa.Name}:{r.Name} -- {r.ReferencedAttribute.ParentInterface.Name}:{r.ReferencedAttribute.Name}\n");
            }
        }

        private void AppendTopDefinition(StringBuilder sb) 
        {
            sb.Append("graph G \n");
            sb.Append("{\n");
            sb.Append("    graph [pad=\"0.5\", nodesep=\"0.5\", ranksep=\"2\"]\n");
            sb.Append("    node [shape=none]\n");
            sb.Append("    rankdir=LR\n");
            if(!generateSlim) {
                sb.Append("    labelloc=t\n"); 
                sb.Append("    label=\"Diagramm des ceusdl-Modells\"\n");
            }
        }


        private string DataTypeToString(CoreDataType dataType)
        {
            switch(dataType) {
                case CoreDataType.INT:
                    return "int";
                case CoreDataType.DECIMAL:
                    return "decimal";
                case CoreDataType.VARCHAR:
                    return "varchar";
                case CoreDataType.DATE:
                    return "date";
                case CoreDataType.DATETIME:
                    return "datetime";
                case CoreDataType.TIME:
                    return "time";
                default:
                    throw new InvalidDataTypeException();
            }
        }

        private string InterfaceTypeToString(CoreInterfaceType type) {
            switch(type) {
                case DEF_TABLE:
                    return "DefTable";
                case TEMPORAL_TABLE:
                    return "TemporalTable";                     
                case DIM_TABLE:
                    return "DimTable";
                case DIM_VIEW:
                    return "DimView";
                case FACT_TABLE:
                    return "FactTable";
                default:
                    return "DimTable";                    
            }
        }
    }
}