using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public class BLModel
    {
        public List<IBLInterface> Interfaces { get; private set; }
        public BLConfig Config { get; private set; }

        public BLModel(CoreModel coreModel) {
            this.Config = new BLConfig(coreModel.Config);
            this.Interfaces = new List<IBLInterface>();

            foreach(var ifa in coreModel.Interfaces) {
                var newIfa = new DefaultBLInterface(ifa, this);
                this.Interfaces.Add(newIfa);
                if(ifa.IsHistorized) {
                    // TODO: dann noch ein DerivedInterface für newIfa anlegen und einfügen!
                }
            }

            // Postprocessing zum Auflösen der Ref-Attribute
            foreach(var ifa in Interfaces) {
                ifa.PostProcess();
            }
        }

    }
}