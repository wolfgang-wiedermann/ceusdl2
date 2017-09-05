using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public class BLModel
    {  
        internal CoreModel coreModel;

        public List<BLInterface> Interfaces { get; private set; }

        public List<BLInterface> TemporalInterfaces {
            get {
                return Interfaces.Where(i => i.Type == CoreInterfaceType.TEMPORAL_TABLE).ToList<BLInterface>();
            }
        }

        // Beinhaltet auch TemporalInterfaces, da diese eine Spezialform des DefInterfaces sind...
        public List<BLInterface> DefInterfaces {
            get {
                return Interfaces.Where(i => i.Type == CoreInterfaceType.TEMPORAL_TABLE || i.Type == CoreInterfaceType.DEF_TABLE).ToList<BLInterface>();
            }
        }

        public List<BLInterface> DimTableInterfaces {
            get {
                return Interfaces.Where(i => i.Type == CoreInterfaceType.DIM_TABLE).ToList<BLInterface>();
            }
        }

        public List<BLInterface> FactTableInterfaces {
            get {
                return Interfaces.Where(i => i.Type == CoreInterfaceType.FACT_TABLE).ToList<BLInterface>();
            }
        }

        public BLModel(CoreModel coreModel) {
            this.coreModel = coreModel;

            this.Interfaces = new List<BLInterface>();
            foreach(var ifa in coreModel.Interfaces) {
                this.Interfaces.Add(new BLInterface(ifa, this));
            }

            // Nachdem alle Interfaces initialisiert wurden, Postprocessing aufrufen,
            // um die Ref-Attribute aufzulösen.
            foreach(var ifa in this.Interfaces) {
                ifa.PostProcess();
            }
        }
    }
}