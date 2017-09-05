using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {

    // TODO: hier fehlt noch der gesamte Mandant-Zeugs!!!
    //       also die Reaktion auf mandant="true"... (vgl. ILInterface ...)
    // TODO: hier fehlt auch noch, dass ab BL immer eine ID-Spalte enthalten sein muss!
    public class BLInterface
    {  
        internal CoreInterface coreInterface {get; private set;}
        internal CoreModel coreModel {get; private set;}
        internal BLModel blModel {get; private set;}

        /// Tabellenname ohne Datenbank und Schema
        public string Name {
            get {
                string ttype = "BL";

                if(coreInterface.Type == CoreInterfaceType.DEF_TABLE
                    || coreInterface.Type == CoreInterfaceType.TEMPORAL_TABLE) {                    
                    ttype = "def";
                }

                if(String.IsNullOrEmpty(coreModel.Config.Prefix)) {
                    return $"{ttype}_{coreInterface.Name}";
                } else {
                    return $"{coreModel.Config.Prefix}_{ttype}_{coreInterface.Name}";
                }                
            }
        }

        // Tabellenname mit Datenbank und Schema
        public string FullName {
            get {
                if(String.IsNullOrEmpty(coreModel.Config.BLDatabase)) {
                    return $"dbo.{Name}";
                } else {
                    return $"{coreModel.Config.BLDatabase}.dbo.{Name}";
                }                
            }            
        }

        public CoreInterfaceType Type {
            get {
                return coreInterface.Type;
            }
        }

        public List<BLAttribute> Attributes { get; private set; }
        
        // Die ReferenceDepth ist eine Metrik die die Anzahl der rekursiv referenzierten
        // Interfaces beinhaltet. Sie wird als Sortierkriterium verwendet, um die die create-
        // Statements für die Tabellen mit der geringsten Abhängigkeitstiefe nach vorn zu
        // sortieren um beim Anlegen Probleme durch nicht vorhandene benötigte Tabellen
        // zu vermeiden.
        internal int GetMaxReferenceDepth(int depth) {            
            return Attributes.Select(a => a.GetReferenceDepth(depth)).Max();
        }

        public int MaxReferenceDepth {
            get { 
                return this.GetMaxReferenceDepth(0);
            }
        }

        public BLInterface(CoreInterface coreInterface, BLModel blModel) {
            this.coreInterface = coreInterface;    
            this.coreModel = blModel.coreModel;
            this.blModel = blModel;

            this.Attributes = new List<BLAttribute>();

            // Mandant-Spalte hinzufügen
            if(coreInterface.IsMandant) {
                 this.Attributes.Add(new BLAttribute("Mandant_ID", CoreDataType.INT, null, null, this));  // Evtl. muss das hier auch noch Mandant_KNZ sein!
            }

            // ID-Spalte hinzufügen
            this.Attributes.Add(new BLAttribute($"{coreInterface.Name}_ID", CoreDataType.INT, null, null, this)); 
            
            foreach(var attr in coreInterface.Attributes) {
                this.Attributes.Add(new BLAttribute(attr, this));
            }

            // TODO: Technisch erforderliche Spalten wie T_MODIFIKATION etc. hinzufügen
        }

        public void PostProcess() {
            foreach(var attr in this.Attributes) {
                attr.PostProcess();
            }
        }
    }
}