using System;
using KDV.CeusDL.Model.Core;

using static KDV.CeusDL.Model.Core.CoreInterfaceType;

namespace KDV.CeusDL.Generator.CeusDL 
{
    public class CeusDLGenerator : IGenerator {

        private CoreModel model = null;

        public CeusDLGenerator(CoreModel model) {
            this.model = model;            
        }

        public string GenerateCode()
        {
            string code = GenerateConfig(model.Config);
            foreach(var ifa in model.Interfaces) {
                code += GenerateInterface(ifa, model);
            }

            return code;
        }

        #region Interface-Generator

        private string GenerateInterface(CoreInterface ifa, CoreModel model)
        {
            string code = $"interface {ifa.Name} : {InterfaceTypeToString(ifa.Type)}";            

            // Interface-Parameter setzen
            if(ifa.IsMandant || ifa.IsHistorized) {
                code += "(";
                if(ifa.IsMandant) {
                    code += "mandant=\"true\"";
                }
                if(ifa.IsHistorized && ifa.IsMandant) {
                    code += ", ";
                }
                if(ifa.IsHistorized) {
                    code += $"history=\"{ifa.HistoryBy.ParentInterface.Name}.{ifa.HistoryBy.Name}\"";
                }
                code += ")";
            }

            // Und die Attribute setzen
            code += " {\n";
            foreach(var attr in ifa.Attributes) {
                code += "   ";
                if(attr is CoreBaseAttribute) {
                    code += GenerateBaseAttribute((CoreBaseAttribute)attr, ifa, model);
                } else if(attr is CoreRefAttribute) {
                    code += GenerateRefAttribute((CoreRefAttribute)attr, ifa, model);
                }
                code += "\n";
            }
            code += "}\n\n";
            return code;
        }

        private string GenerateBaseAttribute(CoreBaseAttribute attr, CoreInterface ifa, CoreModel model)
        {
            var code = $"base {attr.Name}:{DataTypeToString(attr.DataType)}";
            if(attr.IsPrimaryKey || attr.DataType == CoreDataType.DECIMAL 
                || attr.DataType == CoreDataType.VARCHAR || !string.IsNullOrEmpty(attr.Unit)) 
            {                
                bool dirty = false;
                code += "(";
                if(attr.IsPrimaryKey) {
                    dirty = true;
                    code += "primary_key=\"true\"";
                }
                if(attr.DataType == CoreDataType.DECIMAL) {
                    if(dirty) code +=", ";
                    dirty = true;
                    code += $"len=\"{attr.Length},{attr.Decimals}\"";
                }
                if(attr.DataType == CoreDataType.VARCHAR) {
                    if(dirty) code +=", ";
                    dirty = true;
                    code += $"len=\"{attr.Length}\"";
                }
                if(!string.IsNullOrEmpty(attr.Unit)) {
                    if(dirty) code +=", ";
                    dirty = true;
                    code += $"unit=\"{attr.Unit}\"";
                }
                code += ")";
            }
            code += ";";
            return code;
        }

        private string GenerateRefAttribute(CoreRefAttribute attr, CoreInterface ifa, CoreModel model)
        {
            string code = $"ref  {attr.ReferencedInterface.Name}.{attr.ReferencedAttribute.Name}";
            if(attr.IsPrimaryKey) 
            {                                
                code += "(primary_key=\"true\")";
            }
            if(!string.IsNullOrEmpty(attr.Alias)) {
                code += $" as {attr.Alias}";
            }
            code += ";";
            return code;
        }

        private object DataTypeToString(CoreDataType dataType)
        {
            switch(dataType) {
                case CoreDataType.INT:
                    return "int";
                case CoreDataType.DECIMAL:
                    return "decimal";
                case CoreDataType.VARCHAR:
                    return "varchar";
                case CoreDataType.DATE:
                    return "date";
                case CoreDataType.DATETIME:
                    return "datetime";
                case CoreDataType.TIME:
                    return "time";
                default:
                    throw new InvalidDataTypeException();
            }
        }

        private string InterfaceTypeToString(CoreInterfaceType type) {
            switch(type) {
                case DEF_TABLE:
                    return "DefTable";                    
                case DIM_TABLE:
                    return "DimTable";
                case DIM_VIEW:
                    return "DimView";
                case FACT_TABLE:
                    return "FactTable";
                default:
                    return "DimTable";                    
            }
        }

        #endregion
        #region Config-Generator

        private string GenerateConfig(CoreConfig config) {
            string code = "config {";
            code += GenerateConfigParameter("prefix", config.Prefix);
            code += GenerateConfigParameter("al_database", config.ALDatabase);
            code += GenerateConfigParameter("bt_database", config.BTDatabase);
            code += GenerateConfigParameter("bl_database", config.BLDatabase);
            code += GenerateConfigParameter("il_database", config.ILDatabase);            
            code += GenerateConfigParameter("etl_db_server", config.EtlDbServer);
            code += "\n}\n\n";
            return code;
        }

        private string GenerateConfigParameter(string parameterName, string parameterValue) {
            if(!string.IsNullOrEmpty(parameterValue)) {
                return $"\n    {parameterName}=\"{parameterValue}\";";
            } else {
                return "";
            }            
        } 
        #endregion
    }
}