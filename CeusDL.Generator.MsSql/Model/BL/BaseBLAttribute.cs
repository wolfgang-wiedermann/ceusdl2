using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public class BaseBLAttribute : IBLAttribute
    {
        public string Name => throw new NotImplementedException();

        public string FullName => throw new NotImplementedException();

        public CoreDataType DataType => throw new NotImplementedException();

        public int Length => throw new NotImplementedException();

        public int Decimals => throw new NotImplementedException();

        public bool IsPrimaryKey => throw new NotImplementedException();

        public bool IsIdentity => throw new NotImplementedException();

        public IBLInterface ParentInterface => throw new NotImplementedException();

        public string GetSqlDataTypeDefinition()
        {
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