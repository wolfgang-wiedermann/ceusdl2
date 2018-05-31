using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public interface IBLInterface {
        // Kurzname: Wie in CEUSDL-Spezifiziert (z. B. Antrag)
        string ShortName { get; }
        // Vollstäniger Tabellenname in BL (z. B. AP_BL_F_Antrag)
        string Name { get; }
        // Voll qualifizierter Tabellenname in BL (z. B. FH_AP_BaseLayer.dbo.AP_BL_F_Antrag)
        string FullName { get; }        
        // Früherer CEUSDL-Interfacename (aus former_name="") vgl. ShortName
        string ShortFormerName { get; }  
        // Früherer Vollständiger Tabellenname in BL (berechnet aus former_name="") 
        string FormerName { get; }
        // Früherer voll qualifizierter Tabellenname in BL (berechnet aus former_name="")
        string FullFormerName { get; }
        // Vollständiger ViewName in BL (z. B. AP_BL_F_Antrag_VW)
        string ViewName { get; }
        // Voll qualifizierter ViewName in BL (z. B.FH_AP_BaseLayer.dbo.AP_BL_F_Antrag_VW)
        string FullViewName { get; }
        List<IBLAttribute> Attributes { get; }
        CoreInterfaceType InterfaceType { get; }
        bool IsHistorized { get; }
        IBLAttribute HistoryAttribute { get; }
        bool IsMandant { get; }
        List<IBLAttribute> PrimaryKeyAttributes { get; }
        List<IBLAttribute> UniqueKeyAttributes { get; }
        
        // Liste der Attribute, für die auf Update-Geprüft werden soll
        // (für Dimensionstabellen mit und ohne Historisierung)
        List<IBLAttribute> UpdateCheckAttributes { get; }
        int MaxReferenceDepth { get; }
        BLModel ParentModel { get; }

        // TODO: Ist das mit dem Get hier ein gutes Konzept???
        KDV.CeusDL.Model.IL.ILInterface GetILInterface();

        void PostProcess();        
    }
}