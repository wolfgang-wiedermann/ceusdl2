using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;

namespace KDV.CeusDL.Model.BT
{
    public class KnzSubAttribute : ISubAttribute
    {
        private RefBTAttribute refBTAttribute;
        private IBLAttribute blAttribute;
        private RefBLAttribute refBLAttribute;

        public KnzSubAttribute(RefBTAttribute refBTAttribute)
        {
            this.refBTAttribute = refBTAttribute;
            this.refBLAttribute = refBTAttribute.refBLAttribute;
            this.blAttribute = refBTAttribute.blAttribute;            

            if(refBTAttribute.HasToUseVerionTable) {
                var core = refBTAttribute.ReferencedBLInterface.GetCoreInterface();
                var knz = core.Attributes.Where(a => a.IsPrimaryKey).First();
                this.ShortName = $"{core.Name}_VERSION_{knz.Name}";
                this.Alias = refBLAttribute.Core.Alias;
            } else if(refBLAttribute == null) {
                this.ShortName = this.blAttribute.Name;
                this.Alias = null;
            } else {
                this.ShortName = this.refBLAttribute.ReferencedAttribute.Name;
                this.Alias = refBLAttribute.Core.Alias;
            }
        }

        public string ShortName { get; private set; }

        public string Alias { get; private set; }

        public string Name {
            get {
                if(string.IsNullOrEmpty(Alias)) {
                    return ShortName;
                } else {
                    return $"{Alias}_{ShortName}";
                }
            }
        }        

        public string SqlDataType => blAttribute.GetSqlDataTypeDefinition(); // Ob das so schon gut ist?

        public CoreDataType DataType => blAttribute.DataType;
    }
}
