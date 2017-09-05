using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public abstract class CoreAttribute : CoreItemLevelObject {
        public abstract string Name { get; protected set; }
        public bool IsPrimaryKey {get; protected set;}
        public CoreInterface ParentInterface {get; protected set;}
        protected TmpInterfaceAttribute BaseData {get; set;}
        public string WhitespaceBefore { get; set; }        
        public bool IsCalculated { get; private set; }
        public string FormerName { get; protected set; }

        protected CoreModel coreModel;

        public CoreAttribute(TmpInterfaceAttribute tmp, CoreInterface parent, CoreModel model) {
            BaseData = tmp;
            ParentInterface = parent;
            coreModel = model;
            WhitespaceBefore = tmp.WhitespaceBefore;
            FormerName = null;

            if(tmp.Parameters != null && tmp.Parameters.Where(a => a.Name == "primary_key" && a.Value == "true").Count() > 0) {
                IsPrimaryKey = true;
            }
            if(tmp.Parameters != null && tmp.Parameters.Where(p => p.Name == "former_name").Count() > 0) {
                FormerName = tmp.Parameters.Where(p => p.Name == "former_name").First().Value;
            }
            if(tmp.Parameters.Where(p => p.Name == "calculated" && p.Value == "true").Count() == 1) {
                IsCalculated = true;
            } else {
                IsCalculated = false;
            }
        }

        ///
        /// Attributtyp-Spezifische Postprocessing-Schritte anstoßen, 
        /// nachdem alle Attribute und Interfaces angelegt sind.
        ///
        internal virtual void PostProcess() {
            // Normal ist da bisher nichts zu tun (außer bei Ref-Attributen)
        }
    }
}