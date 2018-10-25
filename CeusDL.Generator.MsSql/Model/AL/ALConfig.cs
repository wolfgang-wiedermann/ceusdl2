using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.AL
{
    public class ALConfig
    {
        private CoreConfig coreConfig = null;

        public string Prefix { get; private set; }
        public string BTDatabase { get; private set; }
        public string ALDatabase { get; private set;}
        public string EtlDbServer { get; private set; }

        public ALConfig(CoreConfig coreConfig) {
            this.coreConfig = coreConfig;
            Prefix = coreConfig.Prefix;
            BTDatabase = coreConfig.BTDatabase;
            ALDatabase = coreConfig.ALDatabase;
            EtlDbServer = coreConfig.EtlDbServer;
        }
    }
}