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
        public CoreDataType CDataType {get; private set;}
        public int Length {get; private set;}
        public int Decimals {get; private set;}
        public string DataTypeParameters {get; private set;}
        public bool IsPrimaryKey {get; private set;}
        public bool IsCalcualted {get; private set;}

        public ILAttribute(CoreAttribute attr, ILInterface ifa, CoreModel model)
        {
            this.Core = attr;
            this.ParentInterface = ifa;
            this.coreModel = model;
            
            IsPrimaryKey = attr.IsPrimaryKey;
            IsCalcualted = attr.IsCalculated;  

            if(attr is CoreRefAttribute) {                
                var refAttr = (CoreRefAttribute)attr;                              
                var baseAttr = refAttr.ReferencedAttribute;                
                CDataType = baseAttr.DataType;                
                if(string.IsNullOrEmpty(refAttr.Alias)) {
                    Name = $"{baseAttr.ParentInterface.Name}_{baseAttr.Name}";
                } else {
                    Name = $"{refAttr.Alias}_{baseAttr.ParentInterface.Name}_{baseAttr.Name}";
                }
                PrepareDataTypeAndParams(baseAttr.DataType, baseAttr.Length, baseAttr.Decimals);                
            } else {
                var baseAttr = (CoreBaseAttribute) attr;                                
                CDataType = baseAttr.DataType;
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
                    Length = 0;
                    Decimals = 0;
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
                    Length = length;
                    Decimals = 0;
                    break;
                case CoreDataType.DECIMAL:
                    DataType = "decimal";
                    DataTypeParameters = $"({length},{decimals})";
                    Length = length;
                    Decimals = decimals;
                    break;
            }
        }
    }
}