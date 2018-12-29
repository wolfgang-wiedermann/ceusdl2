using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.IL;
using KDV.CeusDL.Model.MySql.BL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.MySql.BT {

    public class BTModel {

        public BTModel(CoreModel m) : this(new BLModel(m)) {
        }

        public BTModel(BLModel m) {
            this.ParentModel = m;
            this.Config = new BTConfig(m.Core.Config);
            this.Interfaces = new List<BTInterface>();            
            foreach(var ifa in m.Interfaces) {
                this.Interfaces.Add(new BTInterface(ifa, this));
            }
            
            foreach(var ifa in Interfaces) {
                ifa.PostProcess();
            }
        }        

        public BTConfig Config { get; private set; }
        public List<BTInterface> Interfaces { get; private set; }
        public BLModel ParentModel { get; private set; }
    }

}