using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;
using KDV.CeusDL.Model.BL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.BT {

    public class BTInterface {

        internal CoreInterface coreInterface = null;
        internal BL.IBLInterface blInterface = null;

        public BTInterface(IBLInterface ifa, BTModel model) {            
            this.ParentModel = model;
            this.blInterface = ifa;
            this.coreInterface = ifa.GetCoreInterface();
        }

        public BTModel ParentModel { get; private set; }

        public string ShortName { 
            get {
                return coreInterface.Name;
            }
        }

        public string Name {
            get {
                string prefix = "";                
                if(!string.IsNullOrEmpty(ParentModel.Config.Prefix)) {
                    prefix = $"{ParentModel.Config.Prefix}_";
                }
                if(blInterface.InterfaceType == CoreInterfaceType.FACT_TABLE) {
                    return $"{prefix}BT_F_{coreInterface.Name}";
                } else {
                    return $"{prefix}BT_D_{coreInterface.Name}";
                }
            }
        }

        public string FullName {
            get {
                string db = "";
                if(!string.IsNullOrEmpty(ParentModel.Config.BTDatabase)) {
                    db = $"{ParentModel.Config.BTDatabase}.";
                }
                return $"{db}dbo.{this.Name}";
            }
        }
    }

}