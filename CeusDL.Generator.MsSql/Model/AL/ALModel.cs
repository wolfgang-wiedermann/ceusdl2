using System;
using System.Collections.Generic;
using System.Linq;
using KDV.CeusDL.Model.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.AL {
    public class ALModel
    {
        private BTModel parent;         
        public ALModel(BTModel parent)
        {    
            this.parent = parent;
            this.Config = new ALConfig(parent.ParentModel.Core.Config);
            PrepareFactInterfaces();            
        }

        public ALConfig Config { get; private set; }
        public List<FactALInterface> FactInterfaces { get; private set; }
        public List<DimensionALInterface> DimensionInterfaces { get; private set; }

        private void PrepareFactInterfaces() {
            FactInterfaces = new List<FactALInterface>();
            DimensionInterfaces = new List<DimensionALInterface>();
            
            var factBT = parent.Interfaces
                               .Where(i => i.InterfaceType == Core.CoreInterfaceType.FACT_TABLE)
                               .OrderByDescending(i => i.blInterface.MaxReferenceDepth)
                               ;
            
            foreach(var f in factBT) {
                FactInterfaces.Add(new FactALInterface(f, this));
            }            
        }

        internal DimensionALInterface GetDimensionInterfaceFor(DimensionALInterface dimIn) {
            var dimOut = GetDimensionInterfaceByName(dimIn.Name);
            if(dimOut == null) {
                DimensionInterfaces.Add(dimIn);
                dimOut = dimIn;                
            }
            return dimOut;
        }

        internal DimensionALInterface GetDimensionInterfaceByName(string name) {
            return DimensionInterfaces.SingleOrDefault(i => i.Name == name);            
        }
    }
}
