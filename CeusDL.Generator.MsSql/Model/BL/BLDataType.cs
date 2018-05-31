using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public class BLDataType
    {
        public CoreDataType Core { get; private set; }

        public BLDataType(CoreDataType core) {
            this.Core = core;
        }

        public string GetSqlDataType() {
            return BLDataType.GetSqlDataType(this.Core);
        }

        public static string GetSqlDataType(CoreDataType dataType) {
            switch(dataType) {
                case CoreDataType.DATE:                    
                    return "date";                    
                case CoreDataType.DATETIME:
                    return "datetime";                    
                case CoreDataType.DECIMAL:
                    return "decimal";                    
                case CoreDataType.INT:                    
                    // Achtung: Sonderfall: ID in Faktentabllen ist BigInt !!!
                    return "int";                    
                case CoreDataType.VARCHAR:
                    return "varchar";                    
                default:
                    throw new InvalidDataTypeException($"Ungültiger Datentyp: {dataType.ToString()}");
            } 
        }

        public static string GetSqlDataType(IBLAttribute attr) {
            string code = "";

            switch(attr.DataType) {
                case CoreDataType.DATE:                    
                    return "date";                    
                case CoreDataType.DATETIME:
                    return "datetime";                    
                case CoreDataType.DECIMAL:
                    return "decimal";  
                case CoreDataType.INT:
                    // Sonderfall: ID in Fakttabelle als bigint berücksichtigen...
                    if(attr?.ParentInterface?.InterfaceType == CoreInterfaceType.FACT_TABLE && attr.IsIdentity) {
                        return "bigint";
                    } else {
                        return "int";
                    }                                        
                case CoreDataType.VARCHAR:
                    return "varchar";                    
                default:
                    throw new InvalidDataTypeException($"Ungültiger Datentyp: Die SqlDataTypeDefinition für das Attribut {attr.FullName} konnte nicht generiert werden.");
            }         

            return code;
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