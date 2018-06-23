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

            if(HasToUseVerionTable) {
                // Wenn das aktuelle Interface keine FactTable ist und beide eine Historie haben, 
                // dann wird die Beziehung über die Historientabellen aufgelöst
                var blModel = blAttribute.ParentInterface.ParentModel;
                this.ReferencedBLInterface = blModel.Interfaces.Where(i => i.Name == blAttribute.ReferencedAttribute.ParentInterface.Name+"_VERSION").First();
                this.ReferencedBLAttribute = this.ReferencedBLInterface.Attributes.Where(a => a.IsIdentity).First();
            } else {
                // ansonsten wird die Tabelle ohne Historie verwendet.
                this.ReferencedBLAttribute = blAttribute.ReferencedAttribute;
                this.ReferencedBLInterface = blAttribute.ReferencedAttribute.ParentInterface;
            }

            this.IdAttribute = new IdSubAttribute(this);
            this.KnzAttribute = new KnzSubAttribute(this);
        }

        public BTInterface ParentInterface { get; private set; }

        public ISubAttribute IdAttribute { get; private set; }
        public ISubAttribute KnzAttribute { get; private set; }

        public IBLInterface ReferencedBLInterface { get; private set; }
        public IBLAttribute ReferencedBLAttribute { get; private set; }

        public bool IsIdentity => blAttribute.IsIdentity;

        public bool IsPartOfUniqueKey => blAttribute.IsPartOfUniqueKey;
        
        public bool HasToUseVerionTable { 
            get {
                return (blAttribute.ReferencedAttribute.ParentInterface.InterfaceType == CoreInterfaceType.DIM_TABLE
                || blAttribute.ReferencedAttribute.ParentInterface.InterfaceType == CoreInterfaceType.DIM_VIEW)
                && ParentInterface.blInterface.IsHistorized 
                && blAttribute.ReferencedAttribute.ParentInterface.IsHistorized;
            }
        }

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