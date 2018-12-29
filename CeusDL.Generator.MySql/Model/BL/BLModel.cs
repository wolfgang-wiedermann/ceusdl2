using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.MySql.BL {
    public class BLModel
    {
        public IBLInterface FinestTimeAttribute { get; private set; }
        public List<IBLInterface> Interfaces { get; private set; }

        public List<IBLInterface> DefTableInterfaces { 
            get {
                return Interfaces
                    .Where(i => i.InterfaceType == CoreInterfaceType.DEF_TABLE || i.InterfaceType == CoreInterfaceType.TEMPORAL_TABLE)
                    .ToList<IBLInterface>();
            } 
        }

        public List<IBLInterface> DimTableInterfaces { 
            get {
                return Interfaces
                    .Where(i => i.InterfaceType == CoreInterfaceType.DIM_TABLE)
                    .ToList<IBLInterface>();
            } 
        }

        public List<IBLInterface> DimViewInterfaces { 
            get {
                return Interfaces
                    .Where(i => i.InterfaceType == CoreInterfaceType.DIM_VIEW)
                    .ToList<IBLInterface>();
            } 
        }

        public List<IBLInterface> FactTableInterfaces { 
            get {
                return Interfaces
                    .Where(i => i.InterfaceType == CoreInterfaceType.FACT_TABLE)
                    .ToList<IBLInterface>();
            } 
        }

        public BLConfig Config { get; private set; }
        public CoreModel Core { get; private set; }

        public BLModel(CoreModel coreModel) {
            this.Core = coreModel;
            this.Config = new BLConfig(coreModel.Config);
            this.Interfaces = new List<IBLInterface>();            
            CoreInterface finestTimeCoreAttribute = null;

            // Finest-Time-Attribute ermitteln und setzen
            if(coreModel.Interfaces.Where(i => i.IsFinestTime).Count() > 0) {
                finestTimeCoreAttribute = coreModel.Interfaces.Where(i => i.IsFinestTime).First();
                this.FinestTimeAttribute = new DefaultBLInterface(finestTimeCoreAttribute, this);
            } else {
                this.FinestTimeAttribute = null;
            }

            foreach(var ifa in coreModel.Interfaces) {
                // Fallunterscheidung, da FinestTimeAttribute schon vorher initialisiert wurde.
                if(ifa == finestTimeCoreAttribute) {
                    this.Interfaces.Add(this.FinestTimeAttribute);
                } else {
                    var newIfa = new DefaultBLInterface(ifa, this);
                    this.Interfaces.Add(newIfa);

                    // Für historisierte Dimensionstabellen noch eine Derived-Table anlegen.
                    if(ifa.Type == CoreInterfaceType.DIM_TABLE && ifa.IsHistorized) {                    
                        this.Interfaces.Add(new DerivedBLInterface(newIfa, this));
                    }
                }
            }

            // Postprocessing zum Auflösen der Ref-Attribute etc.
            foreach(var ifa in Interfaces) {
                ifa.PostProcess();
            }
        }

    }
}