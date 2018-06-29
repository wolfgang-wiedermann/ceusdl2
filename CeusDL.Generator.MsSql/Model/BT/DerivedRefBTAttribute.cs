using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;
using KDV.CeusDL.Model.BL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.BT {

    public class DerivedRefBTAttribute : IBTAttribute
    {
        public bool IsIdentity => throw new NotImplementedException();

        public bool IsPartOfUniqueKey => throw new NotImplementedException();

        public IBLAttribute GetBLAttribute()
        {
            throw new NotImplementedException();
        }

        public CoreAttribute GetCoreAttribute()
        {
            throw new NotImplementedException();
        }
    }
}