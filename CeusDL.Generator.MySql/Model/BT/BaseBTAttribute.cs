using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.IL;
using KDV.CeusDL.Model.MySql.BL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.MySql.BT {

    public class BaseBTAttribute : IBTAttribute {
        internal IBLAttribute blAttribute;
        public BaseBTAttribute(BaseBLAttribute blAttribute, BTInterface ifa) {
            this.blAttribute = blAttribute;
            this.ParentInterface = ifa;
            SetName();
        }

        public BaseBTAttribute(string name, BaseBLAttribute blAttribute, BTInterface ifa) {
            this.blAttribute = blAttribute;
            this.ParentInterface = ifa;
            Name = name;
        }

        public BaseBTAttribute(CustomBLAttribute blAttribute, BTInterface ifa) {
            this.blAttribute = blAttribute;
            this.ParentInterface = ifa;
            SetName();
        }

        private void SetName() {            
            if(blAttribute.Name.Equals("Mandant_KNZ")) {
                Name = "Mandant_ID";
            } else {
                Name = blAttribute.Name;
            }            
        }

        public BTInterface ParentInterface { get; private set; }

        public string Name { get; set; }

        public string FullName {
            get {
                var db = "";
                if(!string.IsNullOrEmpty(ParentInterface.ParentModel.Config.BTDatabase)) {
                    db = $"{ParentInterface.ParentModel.Config.BTDatabase}.";
                }
                return $"{db}dbo.{this.Name}";
            }
        }

        public bool IsIdentity => blAttribute.IsIdentity;
        public bool IsPartOfUniqueKey => blAttribute.IsPartOfUniqueKey;

        public CoreDataType DataType {
            get {
                if(blAttribute.Name == "Mandant_KNZ") {
                    return CoreDataType.INT;
                } else {
                    return blAttribute.DataType;
                }
            }
        } 

        public IBLAttribute GetBLAttribute() {
            return this.blAttribute;
        }

        public CoreAttribute GetCoreAttribute() {
            return blAttribute.GetILAttribute()?.Core;
        }

        public string GetSqlDataTypeDefinition() {
            if(blAttribute is CustomBLAttribute) {
                if(blAttribute.Name == "Mandant_KNZ") {
                    return "int not null"; // Mandant_ID
                } else if(blAttribute.ParentInterface.InterfaceType == CoreInterfaceType.FACT_TABLE) {
                    return "bigint primary key not null"; // *_ID Spalte in Faktentabelle
                } else if(blAttribute.IsIdentity) {
                    return "int primary key not null"; // *_ID Spalte in Dimensionstabelle
                } else {
                    throw new InvalidDataTypeException();
                }
            } else {
                return blAttribute.GetSqlDataTypeDefinition();
            }
        }
    }

}