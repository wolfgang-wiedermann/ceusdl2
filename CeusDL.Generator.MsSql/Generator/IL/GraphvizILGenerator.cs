// TODO: BL als GraphViz Graphen abbilden...
// Coole Anleitung siehe https://spin.atomicobject.com/2017/11/15/table-rel-diagrams-graphviz/
// In VisualStudio Code verwende ich https://github.com/EFanZh/Graphviz-Preview
using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;

namespace KDV.CeusDL.Generator.IL {
    public class GraphvizILGenerator : IGenerator
    {
        private ILModel model;

        public GraphvizILGenerator(CoreModel model) {
            this.model = new ILModel(model);
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("IL_Diagram.gv", GenerateDiagram()));            
            return result;
        }

        private string GenerateDiagram() {
            string code = "graph G \n";
            code += "{\n";
            code += "    graph [pad=\"0.5\", nodesep=\"0.5\", ranksep=\"2\"]\n";
            code += "    node [shape=none]\n";
            code += "    rankdir=LR\n";
            code += "    labelloc=t\n"; 
            code += "    label=\"Diagramm der InterfaceLayer\"\n";

            foreach(var ifa in model.Interfaces.Where(i => i.IsILRelevant())) {
                code += GenerateInterface(ifa).Indent("    ");
            }

            foreach(var ifa in model.Interfaces) {
                code += GenerateRelations(ifa).Indent("    ");
            }

            code += "}\n";
            return code;
        }

        private string GenerateRelations(ILInterface ifa)
        {
            string code = "# Beziehungen \n";
            foreach(var r in ifa.Attributes.Where(a => a.Core is CoreRefAttribute)) {
                // TODO: Das hier funktioniert evtl. mit aliases noch nicht richtig!?
                var core = ((CoreRefAttribute)r.Core);
                if(core.ReferencedInterface.Type != CoreInterfaceType.DEF_TABLE 
                    && core.ReferencedInterface.Type != CoreInterfaceType.TEMPORAL_TABLE
                    && core.ReferencedInterface.Type != CoreInterfaceType.DIM_VIEW) {
                    code += $"{ifa.ShortName}:{r.Name} -- {core.ReferencedInterface.Name}:{core.ReferencedAttribute.Name}\n";
                }
            }
            return code;
        }

        private string GenerateInterface(ILInterface ifa)
        {
            string code = $"{ifa.ShortName}[label=<\n";
            code += "<table border=\"0\" cellborder=\"1\" cellspacing=\"0\">\n";
            code += $"<tr><td><b>{ifa.Name}:{ifa.Name}</b></td></tr>\n";

            foreach(var attr in ifa.Attributes) {                                    
                code += $"<tr><td port=\"{attr.Name}\">{attr.Name}:{attr.DataType}</td></tr>\n";
            }

            code += "</table>>];\n\n";
            return code;
        }
    }
}