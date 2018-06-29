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
        internal BaseBLAttribute otherBlAttribute;
        public RefBTAttribute(RefBLAttribute blAttribute, BTInterface ifa) {
            this.blAttribute = blAttribute;
            this.otherBlAttribute = null;
            this.ParentInterface = ifa;            
            this.IsIdentity = blAttribute.IsIdentity;
            this.IsPartOfUniqueKey = blAttribute.IsPartOfUniqueKey;
            this.HasToUseVerionTable = GetHasToUseVersionTable();

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

        ///
        /// Wichtig: nur in BT zur Abbildung der History-Beziehung nutzen!
        ///
        internal RefBTAttribute(BaseBLAttribute blAttribute, IBLInterface blIfa, BTInterface ifa) {
            this.blAttribute = null;
            this.otherBlAttribute = blAttribute;
            this.ParentInterface = ifa;            
            this.IsIdentity = otherBlAttribute.IsIdentity;
            this.IsPartOfUniqueKey = otherBlAttribute.IsPartOfUniqueKey;
            this.HasToUseVerionTable = false; // TODO: Das ist noch nicht sicher !

            var blModel = blAttribute.ParentInterface.ParentModel;
            this.ReferencedBLInterface = blIfa;
            this.ReferencedBLAttribute = blIfa.Attributes.Single(a => a.IsPartOfUniqueKey && (a is BaseBLAttribute));

            this.IdAttribute = new IdSubAttribute(this);
            this.KnzAttribute = new KnzSubAttribute(this);  
        }

        public BTInterface ParentInterface { get; private set; }

        public ISubAttribute IdAttribute { get; private set; }
        public ISubAttribute KnzAttribute { get; private set; }

        public IBLInterface ReferencedBLInterface { get; private set; }
        public IBLAttribute ReferencedBLAttribute { get; private set; }

        public bool IsIdentity { get;  private set; }

        public bool IsPartOfUniqueKey { get; private set; }
        
        public bool HasToUseVerionTable { get; private set; }            

        public IBLAttribute GetBLAttribute()
        {
            return blAttribute;
        }

        public CoreAttribute GetCoreAttribute()
        {
            return blAttribute.Core;
        }

        private bool GetHasToUseVersionTable() {
            return (blAttribute.ReferencedAttribute.ParentInterface.InterfaceType == CoreInterfaceType.DIM_TABLE
                || blAttribute.ReferencedAttribute.ParentInterface.InterfaceType == CoreInterfaceType.DIM_VIEW)
                && ParentInterface.blInterface.IsHistorized 
                && blAttribute.ReferencedAttribute.ParentInterface.IsHistorized;
        }
    }

}