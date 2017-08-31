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

        protected CoreModel coreModel;

        public CoreAttribute(TmpInterfaceAttribute tmp, CoreInterface parent, CoreModel model) {
            BaseData = tmp;
            ParentInterface = parent;
            coreModel = model;
            WhitespaceBefore = tmp.WhitespaceBefore;

            if(tmp.Parameters != null && tmp.Parameters.Where(a => a.Name == "primary_key" && a.Value == "true").Count() > 0) {
                IsPrimaryKey = true;
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