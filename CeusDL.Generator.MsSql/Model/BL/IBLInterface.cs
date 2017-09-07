using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public interface IBLInterface {
        // Kurzname: Wie in CEUSDL-Spezifiziert (z. B. Antrag)
        string ShortName { get; }
        // Vollstäniger Tabellenname in BL (z. B. AP_BL_F_Antrag)
        string Name { get; }
        // Voll qualifizierter Tabellennambe in BL (z. B. FH_AP_BaseLayer.dbo.AP_BL_F_Antrag)
        string FullName { get; }        
        List<IBLAttribute> Attributes { get; }
        CoreInterfaceType InterfaceType { get; }
        bool IsHistorized { get; }
        IBLAttribute HistoryAttribute { get; }
        bool IsMandant { get; }
        List<IBLAttribute> PrimaryKeyAttributes { get; }
        List<IBLAttribute> UniqueKeyAttributes { get; }
        int MaxReferenceDepth { get; }
        BLModel ParentModel { get; }

        void PostProcess();
    }
}