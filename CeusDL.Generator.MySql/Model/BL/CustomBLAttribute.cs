using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.IL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.MySql.BL {
    // TODO: Name ist nicht gut, Custom??? eigentlich wäre Technical wohl besser???
    public class CustomBLAttribute : IBLAttribute
    {
        #region HelperFunctions
        ///
        /// Generiert ein neues Mandant-Attribut
        ///
        public static IBLAttribute GetNewMandantAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                ShortName = null,
                Name = "Mandant_KNZ",
                FullName = $"{parentInterface?.Name}.Mandant_KNZ",
                DataType = CoreDataType.VARCHAR,
                Length = 10,
                IsNotNull = true,
                IsPartOfUniqueKey = true,
                ParentInterface = parentInterface,
                IsTechnicalAttribute = false,
                SortId = 0
            };            
        }

        ///
        /// Generiert ein neues ID-Attribut
        ///
        public static IBLAttribute GetNewIDAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                ShortName = "ID",
                Name = $"{parentInterface.ShortName}_ID",
                FullName = $"{parentInterface.Name}.{parentInterface.ShortName}_ID",
                DataType = CoreDataType.INT,                
                IsPrimaryKey = true,
                IsIdentity = true,
                ParentInterface = parentInterface,
                IsTechnicalAttribute = false,
                SortId = 1
            };            
        }    

        ///
        /// Generiert ein neues T_Modifikation-Attribut
        ///
        public static IBLAttribute GetNewTModifikationAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                ShortName = null,
                Name = "T_Modifikation",
                FullName = $"{parentInterface.Name}.T_Modifikation",
                DataType = CoreDataType.VARCHAR,                
                Length = 10,
                IsNotNull = true,
                ParentInterface = parentInterface,
                IsTechnicalAttribute = true,
                SortId = 100
            };            
        }

        ///
        /// Generiert ein neues T_Bemerkung-Attribut
        ///
        public static IBLAttribute GetNewTBemerkungAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                ShortName = null,
                Name = "T_Bemerkung",
                FullName = $"{parentInterface.Name}.T_Bemerkung",
                DataType = CoreDataType.VARCHAR,                
                Length = 100,                
                ParentInterface = parentInterface,
                IsTechnicalAttribute = true,
                SortId = 101
            };            
        }

        ///
        /// Generiert ein neues T_Benutzer-Attribut
        ///
        public static IBLAttribute GetNewTBenutzerAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                ShortName = null,
                Name = "T_Benutzer",
                FullName = $"{parentInterface.Name}.T_Benutzer",
                DataType = CoreDataType.VARCHAR,                
                Length = 100,
                IsNotNull = true,
                ParentInterface = parentInterface,
                IsTechnicalAttribute = true,
                SortId = 102
            };            
        }

        ///
        /// Generiert ein neues T_System-Attribut
        ///
        public static IBLAttribute GetNewTSystemAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                ShortName = null,
                Name = "T_System",
                FullName = $"{parentInterface.Name}.T_System",
                DataType = CoreDataType.VARCHAR,                
                Length = 10,
                IsNotNull = true,
                ParentInterface = parentInterface,
                IsTechnicalAttribute = true,
                SortId = 103
            };            
        }  

        ///
        /// Generiert ein neues T_Erst_Dat-Attribut
        ///
        public static IBLAttribute GetNewTErstDatAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                ShortName = null,
                Name = "T_Erst_Dat",
                FullName = $"{parentInterface.Name}.T_Erst_Dat",
                DataType = CoreDataType.DATETIME,                
                IsNotNull = true,
                ParentInterface = parentInterface,
                IsTechnicalAttribute = true,
                SortId = 104
            };            
        }    

        ///
        /// Generiert ein neues T_Aend_Dat-Attribut
        ///
        public static IBLAttribute GetNewTAendDatAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                ShortName = null,
                Name = "T_Aend_Dat",
                FullName = $"{parentInterface.Name}.T_Aend_Dat",
                DataType = CoreDataType.DATETIME,                
                IsNotNull = true,
                ParentInterface = parentInterface,
                IsTechnicalAttribute = true,
                SortId = 105
            };            
        }

        ///
        /// Generiert ein neues T_Gueltig_Bis-Attribut
        ///
        public static IBLAttribute GetNewTGueltigBisAttribute(IBLInterface parentInterface, IBLInterface timeInterface) {
            if(timeInterface == null)
                throw new MissingFinestTimeAttributeException("Wenn Sie history=\"true\" verwenden müssen Sie ein finest_time_attribute angeben");

            // 1. Das Primärschlüssel-Attribut (z. B. KNZ) aus dem FinestTimeInterface ermitteln.
            var timeAttribute = timeInterface.UniqueKeyAttributes.Where(a => a.Name != "Mandant_KNZ").First();
            // 2. Custom-Attribut bauen.
            return new CustomBLAttribute() {
                ShortName = null,
                Name = "T_Gueltig_Bis_Dat",
                FullName = $"{parentInterface.Name}.T_Gueltig_Bis_Dat",
                DataType = timeAttribute.DataType,
                Length = timeAttribute.Length,
                Decimals = timeAttribute.Decimals,
                IsPartOfUniqueKey = true,              
                ParentInterface = parentInterface,
                IsTechnicalAttribute = true,
                SortId = 106
            };            
        }

        ///
        /// Generiert ein neues T_Ladelauf_NR-Attribut
        ///
        public static IBLAttribute GetNewTLadelaufNRAttribute(IBLInterface parentInterface) {
            return new CustomBLAttribute() {
                ShortName = null,
                Name = "T_Ladelauf_NR",
                FullName = $"{parentInterface.Name}.T_Ladelauf_NR",
                DataType = CoreDataType.INT,                
                IsNotNull = true,
                ParentInterface = parentInterface,
                IsTechnicalAttribute = true,
                SortId = 107
            };            
        }                  
        #endregion

        public string ShortName { get ; private set; }
        public string Name { get; set; }

        public string FullName { get; set; }

        public string ShortFormerName => ShortName;
        public string FormerName => ParentInterface.FormerName!=null&&ShortName!=null?$"{ParentInterface.ShortFormerName}_{ShortName}":null;
        public string FullFormerName => ParentInterface.FormerName!=null&&ShortName!=null?$"{ParentInterface.FormerName}.dbo.{FormerName}":null;
        public string RealFormerName { get; set; }

        public CoreDataType DataType { get; set; }

        public int Length { get; set; }

        public int Decimals { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsIdentity { get; set; }

        public bool IsPartOfUniqueKey { get; set; }

        public bool IsNotNull { get; set; }

        public IBLInterface ParentInterface { get; set; }

        public int SortId { get; set; }

        public bool IsTechnicalAttribute { get; set; }

        public string GetSqlDataTypeDefinition()
        {
            string result = BLDataType.GenerateSqlDataTypeDefinition(this);

            if(IsPrimaryKey) {
                result += " primary key";
            }
            if(IsIdentity && ParentInterface.GetCoreInterface().Type != Core.CoreInterfaceType.DEF_TABLE) {
                result += " auto_increment";
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

        public ILAttribute GetILAttribute()
        {
            return null;
        }
    }
}