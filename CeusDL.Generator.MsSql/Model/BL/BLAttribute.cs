using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    // TODO: 
    // Die Klasse BLAttribute ist so einfach noch Mist. Die schmeiß ich morgen nochmal weg und schreib sie neu...
    // Haupt-Defizit ist, dass man nicht sauber technisch bedingte Attribute im BLInterface anlegen kann
    // IDEE:
    // IBLAttribute -> BLBaseAttribute, BLRefAttribute, BLManualAttribute (letzteres für nachträglich hinzugefügte)
    public class BLAttribute
    {
        private string name = null;
        internal CoreAttribute coreAttribute { get; private set; }
        internal BLModel blModel { get; private set; }         

        public string Name {
            get {
                if(this.coreAttribute == null) {
                    return this.name;
                } else if(this.coreAttribute is CoreRefAttribute) {
                    var refAttr = (CoreRefAttribute)this.coreAttribute;
                    if(string.IsNullOrEmpty(refAttr.Alias)) {
                        return ReferencedAttribute.Name;
                    } else {
                        return $"{refAttr.Alias}_{ReferencedAttribute.Name}";
                    }                    
                } else {
                    return $"{this.coreAttribute.ParentInterface.Name}_{this.coreAttribute.Name}";
                }
            }
        }

        // Die ReferenceDepth ist eine Metrik die die Anzahl der rekursiv referenzierten
        // Interfaces beinhaltet. Sie wird als Sortierkriterium verwendet, um die die create-
        // Statements für die Tabellen mit der geringsten Abhängigkeitstiefe nach vorn zu
        // sortieren um beim Anlegen Probleme durch nicht vorhandene benötigte Tabellen
        // zu vermeiden.
        public int GetReferenceDepth(int depth) {
            if(coreAttribute is CoreRefAttribute) {
                if(depth > 100)
                    return 100;
                return ReferencedAttribute.ParentInterface.GetMaxReferenceDepth(depth+1)+1;                                    
            } else {
                return 0;
            }            
        }        

        public BLAttribute ReferencedAttribute { get; set; }

        public BLInterface ParentInterface { get; private set; }

        public BLAttribute(CoreAttribute coreAttribute, BLInterface parent) {
            this.coreAttribute = coreAttribute;
            this.blModel = parent.blModel;
            this.ParentInterface = parent;
        }

        public BLAttribute(string name, CoreDataType dataType, int? length, int? decimals, BLInterface parent) {
            this.ParentInterface = parent;
            this.name = name;
            this.coreAttribute = null;
            // TODO: etc.
        }

        public void PostProcess() {
            // Postprocessing betrifft nur Ref-Attribute!
            if(coreAttribute is CoreRefAttribute) {
                var refAttr = (CoreRefAttribute)coreAttribute;
                var refIfa = this.blModel.Interfaces.Where(i => i.coreInterface.Name == refAttr.ReferencedInterface.Name).First();
                this.ReferencedAttribute = refIfa.Attributes.Where(a => a?.coreAttribute?.Name == refAttr.ReferencedAttribute.Name).First();
            }
        }

        internal string GetTypeDefinition()
        {
            return "varchar(123)"; // TODO: noch richtig ausprogrammieren
        }
    }
}