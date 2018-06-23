using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BT
{
    public class BTConfig
    {
        private CoreConfig coreConfig = null;

        public string Prefix { get; private set; }
        public string BLDatabase { get; private set; }
        public string BTDatabase { get; private set;}

        public BTConfig(CoreConfig coreConfig) {
            this.coreConfig = coreConfig;
            Prefix = coreConfig.Prefix;
            BLDatabase = coreConfig.BLDatabase;
            BTDatabase = coreConfig.BTDatabase;
        }
    }
}