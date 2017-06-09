using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreBaseAttribute : CoreAttribute
    {
        private string name;

        public override string Name { get => name; protected set => name = value; }
        public CoreDataType DataType {get; private set;}
        public int Length {get; private set;}
        public int Decimals {get; private set;}
        public string Unit {get; private set;}

        public CoreBaseAttribute(TmpInterfaceAttribute tmp, CoreInterface parent, CoreModel model) : base(tmp, parent, model)
        {
            Name = tmp.Name;
            DataType = ToDataType(tmp.DataType);

            Length = 0;
            Decimals = 0;
            if(DataType == CoreDataType.DECIMAL || DataType == CoreDataType.VARCHAR) {
                // Dann Länge ermitteln
                var rs = tmp.Parameters.Where(p => p.Name == "len");
                if(rs.Count() != 1) {
                    throw new InvalidParameterException($"Für Attribute des Datentypen {tmp.DataType} ist die Angabe des len-Parameters erforderlich");
                }
                var lenStr = rs.First().Value;
                if(DataType == CoreDataType.VARCHAR) {
                    Length = Int32.Parse(lenStr);
                } else {
                    var split = lenStr.Split('.');
                    if(split.Length != 2) {
                        throw new InvalidParameterException($"Für Attribute des Datentypen {tmp.DataType} ist die Angabe der Vor- und Nachkommastellen im len-Parameter erforderlich");
                    }
                    Length = Int32.Parse(split[0]);
                    Decimals = Int32.Parse(split[1]);
                }
            } 
            
            if(tmp.Parameters.Where(p => p.Name == "unit").Count() != 0) {
                Unit = tmp.Parameters.Where(p => p.Name == "unit").First().Value;
            }
        }

        private CoreDataType ToDataType(string dataType)
        {
            switch(dataType) {
                case "int":
                    return CoreDataType.INT;
                case "decimal":
                    return CoreDataType.DECIMAL;
                case "varchar":
                    return CoreDataType.VARCHAR;
                case "date":
                    return CoreDataType.DATE;
                case "time":
                    return CoreDataType.TIME;
                case "datetime":
                    return CoreDataType.DATETIME;
                default:
                    throw new InvalidDataTypeException($"Ungültiger Datentyp im Attribut {this.name} im Interface {this.ParentInterface.Name} gefunden!");
            }
        }
    }
}