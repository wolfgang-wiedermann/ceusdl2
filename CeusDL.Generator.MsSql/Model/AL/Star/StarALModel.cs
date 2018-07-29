

using System.Collections.Generic;
using KDV.CeusDL.Model.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.AL.Star {
    public class StarALModel : ALModel
    {
        public StarALModel(CoreModel parent) : base(new BT.BTModel(parent)) {
        }

        public StarALModel(BTModel parent) : base(parent) {
        }

        public List<StarDimensionTable> StarDimensionTables { get; private set; }
    }
}