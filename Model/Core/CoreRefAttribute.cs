using System;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreRefAttribute : CoreAttribute
    {
        public string Alias {get; private set;}
        public CoreBaseAttribute ReferencedAttribute {get; private set;}

        public CoreInterface ReferencedInterface {
            get {
                return ReferencedAttribute.ParentInterface;
            }
        }

        public CoreRefAttribute(TmpInterfaceAttribute tmp, CoreInterface parent, CoreModel model) : base(tmp, parent, model)
        {
            this.Alias = tmp.Alias;
        }

        public override string Name { 
            get {
                if(string.IsNullOrEmpty(Alias)) {
                    return $"{ReferencedInterface.Name}_{ReferencedAttribute.Name}";
                } else {
                    return $"{Alias}_{ReferencedInterface.Name}_{ReferencedAttribute.Name}";
                }
            } 
            protected set => throw new NotImplementedException(); 
        }

        ///
        /// Attributtyp-Spezifische Postprocessing-Schritte anstoßen, 
        /// nachdem alle Attribute und Interfaces angelegt sind.
        ///
        internal override void PostProcess() {        
            ReferencedAttribute = (CoreBaseAttribute)coreModel.GetAttributeByName(BaseData.InterfaceName, BaseData.FieldName);
        }
    }
}