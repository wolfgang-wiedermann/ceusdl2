using System.Collections.Generic;
using System.Linq;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreModel {
        public CoreConfig Config {get; private set;}
        public List<CoreInterface> Interfaces {
            get {
                return Objects.Where(o => o is CoreInterface)
                              .Select(o => (CoreInterface)o)
                              .ToList<CoreInterface>();
            }
        }

        public List<CoreMainLevelObject> Objects {get; private set;}

        public CoreModel(TmpParserResult result) {
            Config = new CoreConfig(result.Config);

            Objects = new List<CoreMainLevelObject>();
            foreach(var obj in result.Objects) {
                if(obj.IsInterface) {
                    Objects.Add(new CoreInterface(obj.Interface, this));
                } else if(obj.IsComment) {
                    Objects.Add(new CoreComment(obj.Comment));
                }
            }

            // Post-Processing um Ref-Attribute aufzulösen.
            foreach(var ifa in Interfaces) {
                ifa.PostProcess();
            }
        }

        public CoreAttribute GetAttributeByName(string name) {
            var tokens = name.Split('.');

            if(tokens.Count() != 2) {
                throw new InvalidParameterException("Attributreferenzen müssen in der Form INTERFACENAME.ATTRIBUTNAME angegeben werden.");
            }

            return GetAttributeByName(tokens[0], tokens[1]);
        }

        public CoreAttribute GetAttributeByName(string ifaName, string attrName) {
            return Interfaces.Where(i => i.Name == ifaName)
                             ?.First().Attributes
                             .Where(a => a is CoreBaseAttribute && a.Name == attrName)
                             ?.First();
        } 
    }
}