using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;
using KDV.CeusDL.Model.BL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.BT {

    public class RefBTAttribute : IBTAttribute {
        internal IBLAttribute blAttribute;
        internal RefBLAttribute refBLAttribute;
        internal BaseBLAttribute otherBlAttribute;
        public RefBTAttribute(RefBLAttribute blAttribute, BTInterface ifa) {
            this.blAttribute = blAttribute;
            this.refBLAttribute = blAttribute;
            this.otherBlAttribute = null;
            this.ParentInterface = ifa;            
            this.IsIdentity = blAttribute.IsIdentity;
            this.IsPartOfUniqueKey = blAttribute.IsPartOfUniqueKey;
            this.HasToUseVerionTable = GetHasToUseVersionTable();

            if(HasToUseVerionTable) {
                // Wenn das aktuelle Interface keine FactTable ist und beide eine Historie haben, 
                // dann wird die Beziehung über die Historientabellen aufgelöst
                var blModel = blAttribute.ParentInterface.ParentModel;
                this.ReferencedBLInterface = blModel.Interfaces.Single(i => i.Name == blAttribute.ReferencedAttribute.ParentInterface.Name+"_VERSION");
                this.ReferencedBLAttribute = this.ReferencedBLInterface.Attributes.Single(a => a.IsPartOfUniqueKey && !a.IsTechnicalAttribute && a.Name != "Mandant_KNZ");
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
            this.blAttribute = blAttribute;
            this.refBLAttribute = null;
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

        public BTInterface ReferencedBTInterface { get; internal set; }
        public IBTAttribute ReferencedBTAttribute { get; internal set; }

        public bool IsIdentity { get;  private set; }

        public bool IsPartOfUniqueKey { get; private set; }
        
        public bool HasToUseVerionTable { get; private set; }            

        public string JoinAlias { get; internal set; }

        public IBLAttribute GetBLAttribute()
        {
            return blAttribute;
        }

        public CoreAttribute GetCoreAttribute()
        {
            if(refBLAttribute != null) return refBLAttribute.Core;
            if(otherBlAttribute != null) return otherBlAttribute.GetILAttribute().Core;
            else return null;
        }

        private bool GetHasToUseVersionTable() {
            // TODO: Prüfen, ob die Bedingung nicht vereinfacht werden kann!
            return (refBLAttribute.ReferencedAttribute.ParentInterface.InterfaceType == CoreInterfaceType.DIM_TABLE
                    || refBLAttribute.ReferencedAttribute.ParentInterface.InterfaceType == CoreInterfaceType.DIM_VIEW)
                && ParentInterface.blInterface.IsHistorized 
                && ParentInterface.blInterface is DerivedBLInterface
                && refBLAttribute.ReferencedAttribute.ParentInterface.IsHistorized
                ;                
        }
    }

}