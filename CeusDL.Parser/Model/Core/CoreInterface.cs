using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreInterface : CoreMainLevelObject {

        public string Name {get; private set;}
        public CoreInterfaceType Type {get; private set;}        

        public bool IsMandant {get; private set;} 
        public bool IsFinestTime {get; private set;} 
        public bool IsWithNowTable {get; private set;}      
        public CoreAttribute HistoryBy {get; private set;}
        public string FormerName { get; private set; }
        public bool IsHistorized { get; private set; }
        public bool IsCalculated { get; private set; }

        public List<CoreItemLevelObject> ItemObjects {get; private set;}
        public List<CoreAttribute> Attributes {
            get {
                return ItemObjects.Where(o => o is CoreAttribute)
                                  .Select(o => (CoreAttribute)o)
                                  .ToList<CoreAttribute>();
            }
        }

        public CoreModel CoreModel { 
            get { 
                return this.coreModel; 
            }
        }

        protected CoreModel coreModel;
        protected TmpInterface BaseData {get;set;}
        public string WhitespaceBefore { get; set; }

        public CoreInterface(TmpInterface tmp, CoreModel model) {
            coreModel = model;
            BaseData = tmp;            
            Name = tmp.Name;                        
            FormerName = null;
            Type = ToInterfaceType(tmp.Type);
            WhitespaceBefore = tmp.WhitespaceBefore;
            IsHistorized = false;
            IsWithNowTable = false;            

            // Interface-Parameter ggf. setzen
            if(tmp.Parameters != null && tmp.Parameters.Where(p => p.Name == "mandant" && p.Value == "true").Count() > 0) {
                IsMandant = true;
            }
            if(tmp.Parameters != null && tmp.Parameters.Where(p => p.Name == "former_name").Count() > 0) {
                FormerName = tmp.Parameters.Where(p => p.Name == "former_name").First().Value;
            }
            if(tmp.Parameters != null && tmp.Parameters.Where(p => p.Name == "with_nowtable" && p.Value == "true").Count() > 0) {
                IsWithNowTable = true;
            }
            if(tmp.Parameters != null && tmp.Parameters.Where(p => p.Name == "history" && p.Value == "true").Count() > 0) {
                IsHistorized = true;
            }
            if(tmp.Parameters != null && tmp.Parameters.Where(p => p.Name == "calculated" && p.Value == "true").Count() > 0) {
                IsCalculated = true;
            }

            // Prüfung semantischer Regeln der Sprache CEUSDL (ausfiltern ungültiger Parameterkombinationen)
            if(Type == CoreInterfaceType.TEMPORAL_TABLE && IsMandant) {
                throw new InvalidParameterException($"Fehler in Interface {Name}: Eine TemporalTable kann nicht Mandantabhängig sein");
            }
            if(Type == CoreInterfaceType.TEMPORAL_TABLE && tmp.Parameters.Where(p => p.Name == "history").Count() > 0) {
                throw new InvalidParameterException($"Fehler in Interface {Name}: Eine TemporalTable kann nicht historisiert werden");
            }
            if(Type != CoreInterfaceType.FACT_TABLE && IsWithNowTable) {
                throw new InvalidParameterException($"Fehler in Interface {Name}: Now-Tables sind nur für FactTables zulässig");
            }
            if(IsWithNowTable && !IsHistorized) {
                throw new InvalidParameterException($"Fehler in Interface {Name}: Now-Tables sind nur für historisierte Tabellen zulässig");
            }

            if(tmp.Parameters != null && tmp.Parameters.Where(a => a.Name == "finest_time_attribute" && a.Value == "true").Count() > 0) {
                // Prüfung: InterfaceType muss TemporalTable sein
                if(Type != CoreInterfaceType.TEMPORAL_TABLE) {                    
                    throw new InvalidParameterException($"Fehler in Interface {Name}: Nur TemporalTables können finest_time_attribute sein");
                }                

                // Prüfung: Nur eine Tabelle kann finest_time_attribute sein
                if(model.Interfaces.Where(i => i.IsFinestTime).Count() > 0) {
                    throw new InvalidParameterException($"Fehler in Interface {Name}: Nur ein Interface kann finest_time_attribute sein");
                }

                IsFinestTime = true;                
            }

            ItemObjects = new List<CoreItemLevelObject>();

            foreach(var tmpObj in tmp.ItemObjects) {
                if(tmpObj.IsAttribute) {
                    var tmpAttr = tmpObj.Attribute;                    
                    CoreAttribute attr = null;
                    switch(tmpAttr.AttributeType) {
                        case "base":
                            attr = new CoreBaseAttribute(tmpAttr, this, model);
                            break;
                        case "ref":
                            attr = new CoreRefAttribute(tmpAttr, this, model);
                            break;
                        case "fact":
                            attr = new CoreFactAttribute(tmpAttr, this, model);
                            break;
                        default:
                            throw new InvalidAttributeTypeException($"Ungültiger Attribut-Typ {tmpAttr.AttributeType} in Interface {tmp.Name} gefunden");
                    }                    
                    ItemObjects.Add(attr);
                } else if(tmpObj.IsComment) {
                    ItemObjects.Add(new CoreComment(tmpObj.Comment));
                } else {
                    throw new InvalidDataTypeException();
                }
            }
            // Info: Ref-Attribute werden im Postprocessing aufgelöst.            
        }

        ///
        /// Mapping des Interface-Typ-Bezeichners auf die Enum-Werte
        ///
        private CoreInterfaceType ToInterfaceType(string typeName) {
            switch(typeName) {
                case "DefTable":
                    return CoreInterfaceType.DEF_TABLE;
                case "TemporalTable":
                    return CoreInterfaceType.TEMPORAL_TABLE;
                case "DimTable":
                    return CoreInterfaceType.DIM_TABLE;
                case "DimView":
                    return CoreInterfaceType.DIM_VIEW;
                case "FactTable":
                    return CoreInterfaceType.FACT_TABLE;
                default:
                    throw new InvalidInterfaceTypeException($"Ungültiger Interface-Typ {typeName}");
            }
        }

        internal void PostProcess() {
            foreach(var attr in Attributes) {
                attr.PostProcess();
            }

            // Bei historisierten Fakt-Tabellen wird das Ref-Attribut, das auf das finest_time_attribute zeigt
            // und kein Alias hat als Basis-Attribut für die Historisierung ausgewählt.
            // bei allen anderen bleibt der Wert von HistoryBy == null
            if(IsHistorized && this.Type == CoreInterfaceType.FACT_TABLE) {                
                var historyByInterface = coreModel.Interfaces.Where(i => i.IsFinestTime).First();

                try {
                    HistoryBy = Attributes
                        .Where(a => a is CoreRefAttribute)
                        .Select(a => (CoreRefAttribute)a)
                        .Where(a => a?.ReferencedInterface?.Name == historyByInterface.Name && string.IsNullOrEmpty(a.Alias))
                        .First();
                } catch(Exception ex) {
                    throw new InvalidParameterException("Fehler in Interface {Name}: Historisierung ist nur Möglich, "
                        +"wenn das finest_time_attribute aus dem historisierten Interface referenziert wird (ohne Alias!). "
                        +$"{ex.Message}");
                }

                // Prüfen ob das gefundene Historienattribut auch aus einer TemporalTable stammt!
                if(historyByInterface.Type != CoreInterfaceType.TEMPORAL_TABLE) {
                    throw new InvalidParameterException($"Fehler in Interface {Name}: Historisierung ist nur auf der Basis von TemporalTables möglich");
                }
            }
        }
    }
}