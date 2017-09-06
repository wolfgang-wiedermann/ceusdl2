using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL
{
    public class BLConfig
    {
        private CoreConfig coreConfig = null;

        public string Prefix { get; private set; }
        public string ILDatabase { get; private set; }
        public string BLDatabase { get; private set;}

        public BLConfig(CoreConfig coreConfig) {
            this.coreConfig = coreConfig;
            Prefix = coreConfig.Prefix;
            ILDatabase = coreConfig.ILDatabase;
            BLDatabase = coreConfig.BLDatabase;
        }
    }
}