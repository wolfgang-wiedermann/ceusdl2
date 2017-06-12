using System.Collections.Generic;
using System.Linq;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreModel {
        public CoreConfig Config {get; private set;}
        public List<CoreInterface> Interfaces {get; private set;}

        public CoreModel(TmpParserResult result) {
            Config = new CoreConfig(result.Config);

            Interfaces = new List<CoreInterface>();
            foreach(var ifa in result.Interfaces) {
                Interfaces.Add(new CoreInterface(ifa, this));
            }

            // Post-Processing um Ref-Attribute aufzulösen.
            foreach(var ifa in Interfaces) {
                foreach(var attr in ifa.Attributes) {
                    attr.PostProcess();
                }
            }
        }

        public CoreAttribute GetAttributeByName(string name) {
            var tokens = name.Split('.');

            if(tokens.Count() != 2) {
                throw new InvalidParameterException("Attributreferenzen müssen in der Form INTERFACENAME.ATTRIBUTNAME angegeben werden.");
            }

            return Interfaces.Where(i => i.Name == tokens[0])
                             ?.First().Attributes
                             .Where(a => a is CoreBaseAttribute && a.Name == tokens[1])
                             ?.First();
        } 
    }
}