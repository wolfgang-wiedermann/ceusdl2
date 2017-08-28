using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreInterface {

        public string Name {get; private set;}
        public CoreInterfaceType Type {get; private set;}        

        public bool IsMandant {get; private set;}        
        public CoreAttribute HistoryBy {get; private set;}

        public bool IsHistorized {
            get {
                return HistoryBy != null;
            }
        }

        public List<CoreAttribute> Attributes {get; private set;}
        protected CoreModel coreModel;
        protected TmpInterface BaseData {get;set;}

        public CoreInterface(TmpInterface tmp, CoreModel model) {
            coreModel = model;
            BaseData = tmp;

            if(tmp.Parameters != null && tmp.Parameters.Where(a => a.Name == "mandant" && a.Value == "true").Count() > 0) {
                IsMandant = true;
            }

            HistoryBy = null;
            Name = tmp.Name;            
            Type = ToInterfaceType(tmp.Type);

            Attributes = new List<CoreAttribute>();
            foreach(var tmpAttr in tmp.Attributes) {
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
                Attributes.Add(attr);
            }
            // TODO: Attribute übernehmen...
            //       aber wie löse ich die Ref-Attribute auf, wenn ich zu dem Zeitpunkt die
            //       Interfaces noch nicht alle als Core-Interface habe???
            //       Wichtig ist auch, dass die Reihenfolge der Attribute exakt erhalten bleiben muss!
        }

        ///
        /// Mapping des Interface-Typ-Bezeichners auf die Enum-Werte
        ///
        private CoreInterfaceType ToInterfaceType(string typeName) {
            switch(typeName) {
                case "DefTable":
                    return CoreInterfaceType.DEF_TABLE;
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
            if(BaseData.Parameters != null && BaseData.Parameters.Where(a => a.Name == "history").Count() > 0) {
                var history = BaseData.Parameters.Where(a => a.Name == "history").First();
                HistoryBy = coreModel.GetAttributeByName(history.Value);
            }
            foreach(var attr in Attributes) {
                attr.PostProcess();
            }
        }
    }
}