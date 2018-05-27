using System;
using System.Data.Common;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;

namespace KDV.CeusDL.Utilities.BL {
    public class ModificationAnalyzer {
        private BLModel model = null;

        public ModificationAnalyzer(BLModel model, DbConnection con) {
            this.model = model;
        }

        // TODO: Hier die vergleichende Analyse des BL-Inhalts lt. Datenbankschema (information_schema) und
        //       des gewünschten Zustands lt. CEUSDL (BLModel) durchführen...
    }
}