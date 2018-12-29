

using System.Collections.Generic;
using KDV.CeusDL.Model.MySql.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.MySql.AL.Snowflake {
    public class SnowflakeALModel : ALModel
    {
        public SnowflakeALModel(CoreModel parent) : base(new BTModel(parent)) {
        }

        public SnowflakeALModel(BTModel parent) : base(parent) {
        }
        
    }
}