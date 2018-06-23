using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;
using KDV.CeusDL.Model.BL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.BT {

    public class BaseBTAttribute : IBTAttribute {
        internal IBLAttribute blAttribute;
        public BaseBTAttribute(BaseBLAttribute blAttribute, BTInterface ifa) {
            this.blAttribute = blAttribute;
            this.ParentInterface = ifa;
        }

        public BaseBTAttribute(CustomBLAttribute blAttribute, BTInterface ifa) {
            this.blAttribute = blAttribute;
            this.ParentInterface = ifa;
        }

        public BTInterface ParentInterface { get; private set; }

        public string Name {
            get {
                return blAttribute.Name;
            }
        }

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

        public CoreDataType DataType => blAttribute.DataType;

        public IBLAttribute GetBLAttribute() {
            return this.blAttribute;
        }

        public CoreAttribute GetCoreAttribute() {
            return blAttribute.GetILAttribute()?.Core;
        }

        public string GetSqlDataTypeDefinition() {
            return blAttribute.GetSqlDataTypeDefinition();
        }
    }

}