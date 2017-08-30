using System;
using System.Collections.Generic;
using System.Linq;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public class TmpInterfaceAttribute {    

        public TmpInterfaceAttribute() {
            this.Alias = "";
        }

        /// base, ref, fact        
        public string AttributeType {get;set;}
        // nur bei base und fact
        public string Name {
            get {
                return name;
            }
            set {
                if((new string[]{"base", "fact"}).Contains(this.AttributeType)) {
                    this.name = value;
                } else {
                    throw new InvalidOperationException("Attribute Name is just valid for base and fact");
                }
            }
        }
        // nur bei ref
        public string InterfaceName {
            get {
                return interfaceName;
            }
            set {
                if("ref".Equals(this.AttributeType)) {
                    this.interfaceName = value;
                } else {
                    throw new InvalidOperationException("Attribute InterfaceName is just valid for ref");
                }
            }
        }
        // nur bei ref
        public string FieldName {
            get {
                return fieldName;
            }
            set {
                if("ref".Equals(this.AttributeType)) {
                    this.fieldName = value;
                } else {
                    throw new InvalidOperationException("Attribute FieldName is just valid for ref");
                }
            }
        }
        // varchar, int, decimal, date, time, datetime (oder so?)
        public string DataType {get;set;}
        public string Alias {get;set;}
        public List<TmpNamedParameter> Parameters {get;set;}

        #region private
        private string name;
        private string interfaceName;
        private string fieldName;
        #endregion
    }
}