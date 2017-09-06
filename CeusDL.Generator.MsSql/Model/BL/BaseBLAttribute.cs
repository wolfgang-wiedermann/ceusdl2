using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public class BaseBLAttribute : IBLAttribute
    {
        #region Private Attributes
        private CoreBaseAttribute coreAttribute = null;
        #endregion

        #region Public Properties
        public string Name { 
            get {
                return $"{this.coreAttribute.ParentInterface.Name}_{this.coreAttribute.Name}";
            }
        }

        public string FullName {
            get {
                if(ParentInterface != null) {
                    return $"{ParentInterface.Name}.{Name}";
                } else {
                    return $"UNKNOWN_TABLE.{Name}";
                }
            }
        }

        public CoreDataType DataType {
            get {
                return this.coreAttribute.DataType;
            }
        }

        public int Length {
            get {
                return this.coreAttribute.Length;
            }
        }

        public int Decimals {
            get {
                return this.coreAttribute.Decimals;
            }
        }

        public bool IsPrimaryKey {
            get {
                // In der BL sind Attribute aus dem ceusdl-File nie PK
                return false;
            }
        }

        public bool IsPartOfUniqueKey {
            get {
                // In der BL wird der Primärschlüssel aus dem Quellsystem (ggf. ergänzt um Mandant und Historienattribut)
                // in einen Unique-Key umgewandelt.
                return this.coreAttribute.IsPrimaryKey;
            }
        }

        public bool IsIdentity {
            get {
                // In der BL sind Attribute aus dem ceusdl-File nie Identity
                return false;
            }
        }

        public IBLInterface ParentInterface { get; private set; }
        
        #endregion

        public BaseBLAttribute(CoreBaseAttribute coreAttribute, IBLInterface parentInterface) {                        
            this.coreAttribute = coreAttribute;            
            this.ParentInterface = parentInterface;
        }

        public string GetSqlDataTypeDefinition()
        {
            // TODO: das hier ist noch nicht alles !!!
            return BaseBLAttribute.GenerateSqlDataTypeDefinition(this);
        }

        public static string GenerateSqlDataTypeDefinition(IBLAttribute attr) {
            string code = "";

            switch(attr.DataType) {
                case CoreDataType.DATE:                    
                    code = "date";
                    break;
                case CoreDataType.DATETIME:
                    code = "datetime";
                    break;
                case CoreDataType.DECIMAL:
                    code = $"decimal({attr.Length}, {attr.Decimals})";
                    break;
                case CoreDataType.INT:
                    // Sonderfall: ID in Fakttabelle als bigint berücksichtigen...
                    if(attr?.ParentInterface?.InterfaceType == CoreInterfaceType.FACT_TABLE && attr.IsIdentity) {
                        code = "bigint";
                    } else {
                        code = "int";
                    }                    
                    break;
                case CoreDataType.VARCHAR:
                    code = $"varchar({attr.Length})";
                    break;
                default:
                    throw new InvalidDataTypeException($"Ungültiger Datentyp: Die SqlDataTypeDefinition für das Attribut {attr.FullName} konnte nicht generiert werden.");
            }         

            return code;
        }
    }
}