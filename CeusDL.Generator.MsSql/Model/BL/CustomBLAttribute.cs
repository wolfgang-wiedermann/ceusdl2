using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public class CustomBLAttribute : IBLAttribute
    {
        #region HelperFunctions
        ///
        /// Generiert ein neues Mandant-Attribut
        ///
        public static IBLAttribute GetNewMandantAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                Name = "Mandant_KNZ",
                FullName = $"{parentInterface?.Name}.Mandant_KNZ",
                DataType = CoreDataType.VARCHAR,
                Length = 10,
                IsNotNull = true,
                ParentInterface = parentInterface
            };            
        }

        ///
        /// Generiert ein neues ID-Attribut
        ///
        public static IBLAttribute GetNewIDAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                Name = $"{parentInterface.ShortName}_ID",
                FullName = $"{parentInterface.Name}.{parentInterface.ShortName}_ID",
                DataType = CoreDataType.INT,                
                IsPrimaryKey = true,
                IsIdentity = true,
                ParentInterface = parentInterface
            };            
        }
        #endregion
        public CustomBLAttribute()
        {
        }

        public string Name { get; set; }

        public string FullName { get; set; }

        public CoreDataType DataType { get; set; }

        public int Length { get; set; }

        public int Decimals { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsNotNull { get; set; }

        public IBLInterface ParentInterface { get; set; }

        public string GetSqlDataTypeDefinition()
        {
            string result = BaseBLAttribute.GenerateSqlDataTypeDefinition(this);

            if(IsPrimaryKey) {
                result += " primary key";
            }
            if(IsIdentity) {
                result += " identity";
            }
            if(IsNotNull || IsPrimaryKey) {
                result += " not null";
            }

            return result;
        }        
    }
}