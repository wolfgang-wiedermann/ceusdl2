using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.IL;
using KDV.CeusDL.Model.MySql.BL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.MySql.BT {
    public interface IBTAttribute {
        bool IsIdentity { get; }
        bool IsPartOfUniqueKey { get; }        

        IBLAttribute GetBLAttribute();
        CoreAttribute GetCoreAttribute();
    }
}