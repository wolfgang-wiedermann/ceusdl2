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
                IsPartOfUniqueKey = true,
                ParentInterface = parentInterface,
                SortId = 0
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
                ParentInterface = parentInterface,
                SortId = 1
            };            
        }

        ///
        /// Generiert ein neues T_Modifikation-Attribut
        ///
        public static IBLAttribute GetNewTModifikationAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                Name = "T_Modifikation",
                FullName = $"{parentInterface.Name}.T_Modifikation",
                DataType = CoreDataType.VARCHAR,                
                Length = 10,
                IsNotNull = true,
                ParentInterface = parentInterface,
                SortId = 100
            };            
        }

        ///
        /// Generiert ein neues T_Bemerkung-Attribut
        ///
        public static IBLAttribute GetNewTBemerkungAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                Name = "T_Bemerkung",
                FullName = $"{parentInterface.Name}.T_Bemerkung",
                DataType = CoreDataType.VARCHAR,                
                Length = 100,                
                ParentInterface = parentInterface,
                SortId = 101
            };            
        }

        ///
        /// Generiert ein neues T_Benutzer-Attribut
        ///
        public static IBLAttribute GetNewTBenutzerAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                Name = "T_Benutzer",
                FullName = $"{parentInterface.Name}.T_Benutzer",
                DataType = CoreDataType.VARCHAR,                
                Length = 100,
                IsNotNull = true,
                ParentInterface = parentInterface,
                SortId = 102
            };            
        }

        ///
        /// Generiert ein neues T_System-Attribut
        ///
        public static IBLAttribute GetNewTSystemAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                Name = "T_System",
                FullName = $"{parentInterface.Name}.T_System",
                DataType = CoreDataType.VARCHAR,                
                Length = 10,
                IsNotNull = true,
                ParentInterface = parentInterface,
                SortId = 103
            };            
        }  

        ///
        /// Generiert ein neues T_Erst_Dat-Attribut
        ///
        public static IBLAttribute GetNewTErstDatAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                Name = "T_Erst_Dat",
                FullName = $"{parentInterface.Name}.T_Erst_Dat",
                DataType = CoreDataType.DATETIME,                
                IsNotNull = true,
                ParentInterface = parentInterface,
                SortId = 104
            };            
        }    

        ///
        /// Generiert ein neues T_Aend_Dat-Attribut
        ///
        public static IBLAttribute GetNewTAendDatAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                Name = "T_Aend_Dat",
                FullName = $"{parentInterface.Name}.T_Aend_Dat",
                DataType = CoreDataType.DATETIME,                
                IsNotNull = true,
                ParentInterface = parentInterface,
                SortId = 105
            };            
        }

        ///
        /// Generiert ein neues T_Ladelauf_NR-Attribut
        ///
        public static IBLAttribute GetNewTLadelaufNRAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                Name = "T_Ladelauf_NR",
                FullName = $"{parentInterface.Name}.T_Ladelauf_NR",
                DataType = CoreDataType.INT,                
                IsNotNull = true,
                ParentInterface = parentInterface,
                SortId = 106
            };            
        }                  
        #endregion

        public string ShortName {
            get {
                return Name;
            }            
        }
        public string Name { get; set; }

        public string FullName { get; set; }

        public CoreDataType DataType { get; set; }

        public int Length { get; set; }

        public int Decimals { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsPartOfUniqueKey { get; set; }

        public bool IsNotNull { get; set; }

        public IBLInterface ParentInterface { get; set; }

        public int SortId { get; set; }

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

        public void PostProcess()
        {
            // Beim CustomBLAttribute gibts im PostProcessing nix zu tun.
        }
    }
}