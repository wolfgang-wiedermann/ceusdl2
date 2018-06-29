using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;
using KDV.CeusDL.Model.BL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.BT {

    public class BTInterface {

        internal CoreInterface coreInterface = null;
        internal BL.IBLInterface blInterface = null;

        public BTInterface(IBLInterface ifa, BTModel model) {            
            this.ParentModel = model;
            this.blInterface = ifa;
            this.coreInterface = ifa.GetCoreInterface();
            this.InterfaceType = coreInterface.Type;

            // TODO: evtl. ist es auch vorteilhaft, wenn die beiden
            //       Interfaces _VERSION und das Original eine Referenz aufeinander haben.
            if(ifa is DerivedBLInterface) {
                this.IsHistoryTable = true;
            } else {
                this.IsHistoryTable = false;
            }

            this.Attributes = new List<IBTAttribute>();
            if(IsHistoryTable)
            {
                // History-Table-Fall                
                PrepareHistoryTable(ifa);
            }
            else {
                // Standardfall
                foreach(var blAttr in ifa.Attributes.Where(a => !a.IsTechnicalAttribute)) {
                   var btAttr = ConvertAttribute(blAttr);
                   if(blAttr.IsIdentity) {
                       this.Identity = btAttr;
                   }
                   this.Attributes.Add(btAttr);
                }
            }             
        }

        public BTModel ParentModel { get; private set; }

        public IBTAttribute Identity { get; private set; }

        public string ShortName { 
            get {
                return coreInterface.Name;
            }
        }

        public string Name {
            get {
                string prefix = "";                
                if(!string.IsNullOrEmpty(ParentModel.Config.Prefix)) {
                    prefix = $"{ParentModel.Config.Prefix}_";
                }
                if(blInterface.InterfaceType == CoreInterfaceType.FACT_TABLE) {
                    return $"{prefix}BT_F_{coreInterface.Name}";
                } else if(this.IsHistoryTable) {
                    return $"{prefix}BT_D_{coreInterface.Name}_VERSION";
                } else {
                    return $"{prefix}BT_D_{coreInterface.Name}";
                }
            }
        }

        public string FullName {
            get {
                string db = "";
                if(!string.IsNullOrEmpty(ParentModel.Config.BTDatabase)) {
                    db = $"{ParentModel.Config.BTDatabase}.";
                }
                return $"{db}dbo.{this.Name}";
            }
        }

        public List<IBTAttribute> Attributes { get; private set; }
        public CoreInterfaceType InterfaceType { get; private set; }

        // Markiert, ob es sich bei der Tabelle um eine technisch generierte Versionstabelle
        // für den Fall history="true" handelt.
        public bool IsHistoryTable { get; private set; } 

        private IBTAttribute ConvertAttribute(IBLAttribute attr) {
            if(attr is BaseBLAttribute) {
                return new BaseBTAttribute((BaseBLAttribute)attr, this);
            } else if(attr is RefBLAttribute) {
                return new RefBTAttribute((RefBLAttribute)attr, this);
            } else if(attr is CustomBLAttribute && attr.IsIdentity) {                
                return new BaseBTAttribute((CustomBLAttribute)attr, this);
            } else if(attr is CustomBLAttribute && attr.Name == "Mandant_KNZ") {                
                return new BaseBTAttribute((CustomBLAttribute)attr, this);                   
            } else {
                throw new InvalidAttributeTypeException($"in new BTInterface.ConvertAttribute() for {attr.Name} in {attr.ParentInterface.Name}");
            }
        }

        ///
        /// Baut die Dinge zusammen, die speziell für eine History-Table erforderlich sind
        ///
        private void PrepareHistoryTable(IBLInterface iifa)
        {
            if(!(iifa is DerivedBLInterface))
                throw new InvalidInterfaceTypeException("An dieser Stelle dürfen nur DerivedBLInterfaces ankommen!");
            
            // Dadurch bekommen wir Zugriff aufs Default-Interface!
            var ifa = (DerivedBLInterface)iifa;            
            var identity = ifa.Attributes.Single(a => a.IsIdentity);            
            var pk = (BaseBLAttribute)ifa.Attributes.Single(a => a.IsPartOfUniqueKey && a is BaseBLAttribute);            

            this.Attributes.Add(ConvertAttribute(identity));
            this.Attributes.Add(CreateVersionKNZAttribute(pk));
            this.Attributes.Add(CreateNonVersionedRefAttribute(pk, ifa));

            // Restliche Standardattribute
            var dataAttributes = ifa.Attributes
                                    .Where(a => !(a.IsTechnicalAttribute
                                        || a.IsIdentity
                                        || (a.IsPartOfUniqueKey && a is BaseBLAttribute)
                                        ));
            foreach (var attr in dataAttributes)
            {
                this.Attributes.Add(ConvertAttribute(attr));
            }
        }

        private IBTAttribute CreateNonVersionedRefAttribute(BaseBLAttribute pk, DerivedBLInterface ifa)
        {
            return new RefBTAttribute(pk, ifa.DefaultInterface, this);            
        }

        private IBTAttribute CreateVersionKNZAttribute(BaseBLAttribute pk)
        {
            string ifaName = pk.GetILAttribute().Core.ParentInterface.Name;
            string fieldName = pk.GetILAttribute().Core.Name;
            return new BaseBTAttribute($"{ifaName}_VERSION_{fieldName}", pk, this);
        }
    }

}