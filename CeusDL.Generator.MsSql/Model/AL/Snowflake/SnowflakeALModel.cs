

using System.Collections.Generic;
using KDV.CeusDL.Model.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.AL.Snowflake {
    public class SnowflakeALModel : ALModel
    {
        public SnowflakeALModel(CoreModel parent) : base(new BT.BTModel(parent)) {
        }

        public SnowflakeALModel(BTModel parent) : base(parent) {
        }
        
    }
}