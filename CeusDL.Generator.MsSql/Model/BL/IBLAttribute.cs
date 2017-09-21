using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public interface IBLAttribute {
        #region Properties

        string ShortName { get; } // Attributname in ceusdl-Syntax z. B. KNZ
        string Name { get; } // Attributname in BL-Syntax z. B. Semester_KNZ      
        string FullName { get; } // Attributname incl. Tabellenname getrennt durch Punkt
        CoreDataType DataType { get; }
        int Length { get; }
        int Decimals { get; }
        bool IsPrimaryKey { get; }
        bool IsIdentity { get; }        
        bool IsPartOfUniqueKey { get; } // Markiert den Original-PK aus dem Quellsystem!
        bool IsTechnicalAttribute { get; }
        IBLInterface ParentInterface { get; }
        int SortId { get; } // Sortier-ID z. B. für den UniqueKey        

        #endregion Properties
        #region Methods

        // Erstellt den SQL-Code für die Datentyp-Definition.
        string GetSqlDataTypeDefinition(); 

        // TODO: Ist das mit dem Get hier ein gutes Konzept???
        KDV.CeusDL.Model.IL.ILAttribute GetILAttribute();

        void PostProcess(); 

        #endregion Methods
    }
}