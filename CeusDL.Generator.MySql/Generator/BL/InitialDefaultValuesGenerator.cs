using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BL;

namespace KDV.CeusDL.Generator.MySql.BL {
    public class InitialDefaultValuesGenerator : IGenerator
    {
        private BLModel model;

        public InitialDefaultValuesGenerator(CoreModel model) {
            this.model = new BLModel(model);
        }

        public InitialDefaultValuesGenerator(BLModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_InitialDefaultValues.sql", GenerateInitialDefaults()));            
            return result;
        }

        private string GenerateInitialDefaults() {
            string code = "--\n-- BaseLayer initial default values \n--\n";
            
            if(!string.IsNullOrEmpty(model.Config.BLDatabase)) {
                code += $"\nuse {model.Config.BLDatabase};\n\n";
            }

            foreach(var ifa in model.DefTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {
                code += GenerateUnbekannt(ifa);                
            }

            foreach(var ifa in model.DimTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {                
                code += GenerateUnbekannt(ifa);                  
            }
            
            return code;
        }

        private string GenerateUnbekannt(IBLInterface ifa) {
            string code = $"";
            code += $"insert into {ifa.FullName} ";            
            string attributeList = "";
            string valuesList = "";
            foreach(var attr in ifa.Attributes) {
                attributeList += attr.Name;
                if(attr.IsIdentity) {
                    valuesList += "-1";
                } else if(attr.IsPartOfUniqueKey) { // KNZ und ggf. Mandant_KNZ
                    valuesList += "'-1'";
                } else if(attr.DataType == CoreDataType.VARCHAR) { // Alle anderen textuellen Felder
                    valuesList += "'UNBEKANNT'";
                } else if(attr.DataType == CoreDataType.DATE || attr.DataType == CoreDataType.DATETIME) { // Datumsfelder auf aktuelles Datum
                    valuesList += "NOW()";
                } else if(attr.Name == "T_Ladelauf_NR") { // Ladelauf 0
                    valuesList += "0";
                } else { // Ansonsten versuchen wir es mal mit NULL ;-) evtl geht das aber nicht immer?
                    valuesList += "NULL";
                }
                if(attr != ifa.Attributes.Last()) {
                    attributeList += ",\n";
                    valuesList += ",\n";
                }
            }
            code += $"(\n{attributeList.Indent(1)}\n) values (\n{valuesList.Indent(1)}\n);\n";
            code += $"\n";
            return code;
        }
    }
}