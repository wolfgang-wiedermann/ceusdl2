// TODO: BL als GraphViz Graphen abbilden...
// Coole Anleitung siehe https://spin.atomicobject.com/2017/11/15/table-rel-diagrams-graphviz/
// In VisualStudio Code verwende ich https://github.com/EFanZh/Graphviz-Preview
using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;

namespace KDV.CeusDL.Generator.BL {
    public class GraphvizBLGenerator : IGenerator
    {
        private BLModel model;

        public GraphvizBLGenerator(CoreModel model) {
            this.model = new BLModel(model);
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Diagram.gv", GenerateDiagram()));            
            return result;
        }

        private string GenerateDiagram() {
            string code = "graph G \n";
            code += "{\n";
            code += "    graph [pad=\"0.5\", nodesep=\"0.5\", ranksep=\"2\"]\n";
            code += "    node [shape=none]\n";
            code += "    rankdir=LR\n";
            code += "    labelloc=t\n"; 
            code += "    label=\"Diagramm der BaseLayer\"\n";

            foreach(var ifa in model.Interfaces) {
                code += GenerateInterface(ifa).Indent("    ");
            }

            foreach(var ifa in model.Interfaces) {
                code += GenerateRelations(ifa).Indent("    ");
            }

            code += "}\n";
            return code;
        }

        private string GenerateRelations(IBLInterface ifa)
        {
            string code = "# Beziehungen \n";
            foreach(var r in ifa.Attributes.Where(a => a is RefBLAttribute).Select(a => (RefBLAttribute)a)) {
                code += $"{ifa.ShortName}:{r.Name} -- {r.ReferencedAttribute.ParentInterface.ShortName}:{r.ReferencedAttribute.Name}\n";
            }
            return code;
        }

        private string GenerateInterface(IBLInterface ifa)
        {
            string code = $"{ifa.ShortName}[label=<\n";
            code += "<table border=\"0\" cellborder=\"1\" cellspacing=\"0\">\n";
            code += $"<tr><td><b>{ifa.Name}:{ifa.InterfaceType}</b></td></tr>\n";

            foreach(var attr in ifa.Attributes) {
                code += $"<tr><td port=\"{attr.Name}\">{attr.Name}:{attr.GetSqlDataTypeDefinition()}</td></tr>\n";
            }

            code += "</table>>];\n\n";
            return code;
        }
    }
}