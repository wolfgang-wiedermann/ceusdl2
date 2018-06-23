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
        private RefBLAttribute blAttribute;

        public KnzSubAttribute(RefBTAttribute refBTAttribute)
        {
            this.refBTAttribute = refBTAttribute;
            this.blAttribute = refBTAttribute.blAttribute;

            if(refBTAttribute.HasToUseVerionTable) {
                var core = refBTAttribute.ReferencedBLInterface.GetCoreInterface();
                var knz = core.Attributes.Where(a => a.IsPrimaryKey).First();
                this.ShortName = $"{core.Name}_VERSION_{knz.Name}";
            } else {
                this.ShortName = this.blAttribute.ReferencedAttribute.Name;
            }
        }

        public string ShortName { get; private set; }

        public string Alias => blAttribute.Core.Alias;

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
