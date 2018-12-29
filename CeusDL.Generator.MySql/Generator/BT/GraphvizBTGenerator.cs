using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BT;
using System.Text;

namespace KDV.CeusDL.Generator.MySql.BT {
    public class GraphvizBTGenerator : IGenerator
    {
        private BTModel model;

        public GraphvizBTGenerator(CoreModel model) {
            this.model = new BTModel(model);
        }

        public GraphvizBTGenerator(BTModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode()
        {            
            List<GeneratorResult> result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BT_Diagram.gv", GenerateDiagram()));
            return result;
        }

        private string GenerateDiagram()
        {
            string code = "graph G \n";
            code += "{\n";
            code += "    graph [pad=\"0.5\", nodesep=\"0.5\", ranksep=\"2\"]\n";
            code += "    node [shape=none]\n";
            code += "    rankdir=LR\n";
            code += "    labelloc=t\n"; 
            code += "    label=\"Diagramm der BT\"\n";

            foreach(var ifa in model.Interfaces) {
                code += GenerateInterface(ifa).Indent(1);
            }

            foreach(var ifa in model.Interfaces) {
                code += GenerateRelations(ifa).Indent(1);
            }

            code += "}\n";
            return code;
        }

        private string GenerateRelations(BTInterface ifa)
        {
            var relevantAttributes = ifa.Attributes.Where(a => a is RefBTAttribute).Select(a => (RefBTAttribute)a);
            if(relevantAttributes.Count() > 0) {
                string code = "# Beziehungen \n";
                foreach(var r in relevantAttributes) {
                    code += $"{ifa.Name}:{r.IdAttribute.Name} -- {r.ReferencedBTInterface.Name}:{((BaseBTAttribute)r.ReferencedBTAttribute).Name}\n";
                }
                return code;
            } else {
                return "";
            }
        }

        private string GenerateInterface(BTInterface ifa)
        {
            string code = $"{ifa.Name}[label=<\n";
            code += "<table border=\"0\" cellborder=\"1\" cellspacing=\"0\">\n";
            code += $"<tr><td><b>{ifa.Name}:{ifa.InterfaceType}</b></td></tr>\n";

            foreach(var attr in ifa.Attributes) {
                if(attr is BaseBTAttribute) {
                    var baseAttr = (BaseBTAttribute)attr;
                    code += $"<tr><td port=\"{baseAttr.Name}\">{baseAttr.Name}:{baseAttr.GetSqlDataTypeDefinition()}</td></tr>\n";
                } else if(attr is RefBTAttribute) {
                    var baseAttr = (RefBTAttribute)attr;
                    code += $"<tr><td port=\"{baseAttr.IdAttribute.Name}\">{baseAttr.IdAttribute.Name}:{baseAttr.IdAttribute.SqlDataType}</td></tr>\n";
                    code += $"<tr><td>{baseAttr.KnzAttribute.Name}:{baseAttr.KnzAttribute.SqlDataType}</td></tr>\n";
                }                
            }

            code += "</table>>];\n\n";
            return code;
        }
    }    
}