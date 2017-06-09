using System.Collections.Generic;
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
        }
    }
}