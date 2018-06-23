using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;
using KDV.CeusDL.Model.BL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.BT {

    public class RefBTAttribute : IBTAttribute {
        internal RefBLAttribute blAttribute;
        public RefBTAttribute(RefBLAttribute blAttribute, BTInterface ifa) {
            this.blAttribute = blAttribute;
            this.ParentInterface = ifa;
            this.IdAttribute = new IdSubAttribute(this);
            this.KnzAttribute = new KnzSubAttribute(this);
        }

        public BTInterface ParentInterface { get; private set; }

        public ISubAttribute IdAttribute { get; private set; }
        public ISubAttribute KnzAttribute { get; private set; }

// TODO: Das wird ersetzt

        public string KnzName { 
            get {
                return blAttribute.Name;
            }
        }
// ENDE von Das wird ersetzt
        public bool IsIdentity => blAttribute.IsIdentity;

        public bool IsPartOfUniqueKey => blAttribute.IsPartOfUniqueKey;
        

        public IBLAttribute GetBLAttribute()
        {
            return blAttribute;
        }

        public CoreAttribute GetCoreAttribute()
        {
            return blAttribute.Core;
        }
    }

}