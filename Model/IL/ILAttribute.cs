using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using System;

namespace KDV.CeusDL.Model.IL {
    public class ILAttribute
    {        
        private CoreModel coreModel;

        public CoreAttribute Core {get; private set;}        

        public ILInterface ParentInterface { get; private set; }

        public string Name {get; private set;}
        public string DataType {get; private set;}
        public string DataTypeParameters {get; private set;}
        public bool IsPrimaryKey {get; private set;}

        public ILAttribute(CoreAttribute attr, ILInterface ifa, CoreModel model)
        {
            this.Core = attr;
            this.ParentInterface = ifa;
            this.coreModel = model;
            
            IsPrimaryKey = attr.IsPrimaryKey;

            if(attr is CoreRefAttribute) {                                
                var baseAttr = ((CoreRefAttribute)attr).ReferencedAttribute;
                if(string.IsNullOrEmpty(((CoreRefAttribute)attr).Alias)) {
                    Name = $"{baseAttr.ParentInterface.Name}_{baseAttr.Name}";
                } else {
                    Name = $"{((CoreRefAttribute)attr).Alias}_{baseAttr.ParentInterface.Name}_{baseAttr.Name}";
                }
                PrepareDataTypeAndParams(baseAttr.DataType, baseAttr.Length, baseAttr.Decimals);                
            } else {
                var baseAttr = (CoreBaseAttribute) attr;                
                Name = $"{baseAttr.ParentInterface.Name}_{baseAttr.Name}";
                PrepareDataTypeAndParams(baseAttr.DataType, baseAttr.Length, baseAttr.Decimals);
            }

            if(IsPrimaryKey) {
                DataTypeParameters += " not null";
            }
        }

        public ILAttribute(string name, CoreDataType dataType, int length, int decimals, bool primaryKey) {
            Name = name;
            IsPrimaryKey = primaryKey;
            PrepareDataTypeAndParams(dataType, length, decimals);
            if(IsPrimaryKey) {
                DataTypeParameters += " not null";
            }
        }

        private void PrepareDataTypeAndParams(CoreDataType dataType, int length, int decimals)
        {
            switch(dataType) {
                case CoreDataType.INT:
                    DataType = "int";
                    DataTypeParameters = "";
                    break;
                case CoreDataType.DATE:
                case CoreDataType.TIME:
                case CoreDataType.DATETIME:
                    DataType = "datetime";
                    DataTypeParameters = "";
                    break;
                case CoreDataType.VARCHAR:
                    DataType = "varchar";
                    DataTypeParameters = $"({length})";
                    break;
                case CoreDataType.DECIMAL:
                    DataType = "decimal";
                    DataTypeParameters = $"({length},{decimals})";
                    break;
            }
        }
    }
}