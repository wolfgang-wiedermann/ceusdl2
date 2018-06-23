using System;
using System.Collections.Generic;
using System.Linq;
using KDV.CeusDL.Model.BL;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BT
{
    public class IdSubAttribute : ISubAttribute
    {
        private RefBTAttribute refBTAttribute;
        private RefBLAttribute blAttribute;

        public IdSubAttribute(RefBTAttribute refBTAttribute)
        {
            this.refBTAttribute = refBTAttribute;
            this.blAttribute = refBTAttribute.blAttribute;
        }

        public string ShortName => refBTAttribute.ReferencedBLInterface.PrimaryKeyAttributes.First().Name;

        public string Alias => blAttribute.Core.Alias;

        public string Name {
            get {
                var baseName = refBTAttribute.ReferencedBLInterface.PrimaryKeyAttributes.First().Name;
                var alias = blAttribute.Core.Alias;
                if(string.IsNullOrEmpty(alias)) {
                    return baseName;
                } else {
                    return $"{alias}_{baseName}";
                }  
            }
        }        

        public string SqlDataType {
            get {
                if(refBTAttribute.ReferencedBLInterface.InterfaceType == CoreInterfaceType.FACT_TABLE) {
                    return "bigint"; // Bei Fakttabellen ist die ID-Spalte immer bigint
                } else {
                    return "int"; // Bei allen anderen Tabellen ist die ID-Spalte immer int
                }
            }
        }

        public CoreDataType DataType => CoreDataType.INT; // ID-Spalten sind bei uns immer ein int-Datentyp
    }
}
