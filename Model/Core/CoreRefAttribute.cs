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

        public override string Name { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        ///
        /// Attributtyp-Spezifische Postprocessing-Schritte anstoßen, 
        /// nachdem alle Attribute und Interfaces angelegt sind.
        ///
        internal override void PostProcess() {
            var fieldName = BaseData.InterfaceName+"."+BaseData.FieldName;
            Console.WriteLine("FieldName: "+fieldName);
            ReferencedAttribute = (CoreBaseAttribute)coreModel.GetAttributeByName(fieldName);
        }
    }
}