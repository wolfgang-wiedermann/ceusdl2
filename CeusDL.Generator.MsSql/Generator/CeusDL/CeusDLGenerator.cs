using System;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;

using static KDV.CeusDL.Model.Core.CoreInterfaceType;

namespace KDV.CeusDL.Generator.CeusDL 
{
    public class CeusDLGenerator : IGenerator {

        private CoreModel model = null;

        public CeusDLGenerator(CoreModel model) {
            this.model = model;            
        }

        public List<GeneratorResult> GenerateCode()
        {
            string code = "";
            foreach(var obj in model.Objects) {
                if(obj is CoreInterface) {                    
                    code += GenerateInterface((CoreInterface)obj, model);
                } else if(obj is CoreComment) {                    
                    code += obj.ToString();
                } else if(obj is CoreConfig) {
                    code += GenerateConfig(model.Config);
                }
            }

            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("generated.ceusdl", code));
            return result;
        }

        #region Interface-Generator

        private string GenerateInterface(CoreInterface ifa, CoreModel model)
        {
            string code = $"{ifa.WhitespaceBefore}interface {ifa.Name} : {InterfaceTypeToString(ifa.Type)}";            

            // Interface-Parameter setzen
            if(ifa.IsMandant || ifa.IsHistorized) {
                code += "(";                
                if(ifa.IsMandant) {
                    code += "mandant=\"true\"";
                }
                if(ifa.IsHistorized && ifa.IsMandant) {
                    code += ", ";
                }

                // Historisiert und FinestTime => Beides kann nicht gleichzeitig auftreten!!
                if(ifa.IsHistorized) {
                    code += $"history=\"{ifa.HistoryBy.ParentInterface.Name}.{ifa.HistoryBy.Name}\"";
                }
                code += ")";
            } else if(ifa.IsFinestTime) {
                code += "(finest_time_attribute=\"true\")";
            }

            // Und die Attribute setzen
            code += " {";
            foreach(var item in ifa.ItemObjects) {                                
                if(item is CoreFactAttribute) {
                    //code += "    ";
                    code += GenerateFactAttribute((CoreFactAttribute)item, ifa, model);
                    //code += "\n";
                } else if(item is CoreBaseAttribute) {
                    //code += "    ";
                    code += GenerateBaseAttribute((CoreBaseAttribute)item, ifa, model);
                    //code += "\n";
                } else if(item is CoreRefAttribute) {
                    //code += "    ";
                    code += GenerateRefAttribute((CoreRefAttribute)item, ifa, model);
                    //code += "\n";
                } else if(item is CoreComment) {
                    // TODO: Hier passt noch nix wirklich!!!
                    //  * Einrückung falsch
                    //  * Abstand zwischen Kommentarzeilen falsch etc...                    
                    code += item.ToString();
                }                                
            }
            code += "\n}";
            return code;
        }

        private string GenerateBaseAttribute(CoreBaseAttribute attr, CoreInterface ifa, CoreModel model) {
            return GenerateBaseAttribute("base", attr, ifa, model);
        }

        private string GenerateFactAttribute(CoreBaseAttribute attr, CoreInterface ifa, CoreModel model) {
            return GenerateBaseAttribute("fact", attr, ifa, model);
        }

        private string GenerateBaseAttribute(string type, CoreBaseAttribute attr, CoreInterface ifa, CoreModel model)
        {
            var code = $"{attr.WhitespaceBefore}{type} {attr.Name}:{DataTypeToString(attr.DataType)}";
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
            string code = $"{attr.WhitespaceBefore}ref  {attr.ReferencedInterface.Name}.{attr.ReferencedAttribute.Name}";
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
                case TEMPORAL_TABLE:
                    return "TemporalTable";                     
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
            code += "\n}";
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