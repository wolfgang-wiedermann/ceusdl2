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
        }

    }
}