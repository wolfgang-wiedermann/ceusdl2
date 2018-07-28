

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

        public string SqlType => throw new NotImplementedException();

        public string SqlTypeDefinition => throw new System.NotImplementedException();

        private string CalculateName() {
            if(ParentInterface is FactALInterface) {
                return ((BaseBTAttribute)BTAttribute).Name;
            } else if (ParentInterface is DimensionALInterface) {
                var pi = (DimensionALInterface)ParentInterface;
                return $"{pi.Alias}{((BaseBTAttribute)BTAttribute).Name}";
            } else {
                throw new NotImplementedException("Unerwarteter InterfaceTyp");
            }
        }
    }
}