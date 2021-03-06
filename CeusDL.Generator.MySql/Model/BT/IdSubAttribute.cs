using System;
using System.Collections.Generic;
using System.Linq;
using KDV.CeusDL.Model.MySql.BL;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.MySql.BT
{
    public class IdSubAttribute : ISubAttribute
    {
        private RefBTAttribute refBTAttribute;
        private RefBLAttribute blAttribute;

        public IdSubAttribute(RefBTAttribute refBTAttribute)
        {
            this.refBTAttribute = refBTAttribute;
            if(refBTAttribute.blAttribute is RefBLAttribute) {
                this.blAttribute = (RefBLAttribute)refBTAttribute.blAttribute;
            }                   
        }

        public string ShortName => refBTAttribute.ReferencedBLInterface.PrimaryKeyAttributes.First().Name; // TODO: Prüfen ob das für alle Fälle so gut ist!

        public string Alias => blAttribute?.Core?.Alias;

        public string Name {
            get {
                var baseName = refBTAttribute.ReferencedBLInterface.PrimaryKeyAttributes.First().Name; // TODO: Prüfen ob das für alle Fälle so gut ist!
                var alias = blAttribute?.Core?.Alias;
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
