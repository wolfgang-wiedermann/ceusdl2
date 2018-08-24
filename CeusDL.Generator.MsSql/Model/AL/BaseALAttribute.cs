

using System;
using KDV.CeusDL.Model.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.AL {
    public class BaseALAttribute : IALAttribute
    {
        public BaseALAttribute(IALInterface parentInterface, BaseBTAttribute attr)
        {
            ParentInterface = parentInterface;
            BTAttribute = attr;
            Name = CalculateName();
        }        

        public CoreAttribute Core => BTAttribute.GetCoreAttribute();

        public IALInterface ParentInterface { get; private set; }

        public IBTAttribute BTAttribute { get; private set; }

        public string Name { get; private set; }

        public string SqlType {
            get {
                var attr = (BaseBTAttribute)BTAttribute;
                if(!attr.IsIdentity) {
                    return attr.GetSqlDataTypeDefinition();
                } else if(attr.ParentInterface.InterfaceType == CoreInterfaceType.FACT_TABLE) {
                    return "bigint";
                } else {
                    return "int";
                }
            }
        }

        public bool IsFact => BTAttribute.GetCoreAttribute() is CoreFactAttribute;

        private string CalculateName() {
            if(ParentInterface is FactALInterface) {                
                return ((BaseBTAttribute)BTAttribute).Name;
            } else if (ParentInterface is DimensionALInterface) {
                var pi = (DimensionALInterface)ParentInterface;
                var baseAttr = (BaseBTAttribute)BTAttribute;
                if("Mandant_ID".Equals(baseAttr.Name)) {
                    return "Mandant_ID";
                } else {
                    return $"{pi.RootDimension.ShortName}_{pi.Alias}{((BaseBTAttribute)BTAttribute).Name}";
                }                
            } else {
                throw new NotImplementedException("Unerwarteter InterfaceTyp");
            }
        }
    }
}