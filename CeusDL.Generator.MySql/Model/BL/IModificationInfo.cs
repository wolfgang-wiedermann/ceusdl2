using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.MySql.BL {
    public interface IModificationInfo {
        BLModificationType ModificationType { get; set; }
    }
}
