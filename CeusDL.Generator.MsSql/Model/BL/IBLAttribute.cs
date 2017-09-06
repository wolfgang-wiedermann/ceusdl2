using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public interface IBLAttribute {
        #region Properties

        string Name { get; } // Attributname in BL-Syntax        
        string FullName { get; } // Attributname incl. Tabellenname getrennt durch Punkt
        CoreDataType DataType { get; }
        int Length { get; }
        int Decimals { get; }
        bool IsPrimaryKey { get; }
        bool IsIdentity { get; }
        bool IsPartOfUniqueKey { get; } // Markiert den Original-PK aus dem Quellsystem!
        IBLInterface ParentInterface { get; }

        #endregion Properties
        #region Methods

        // Erstellt den SQL-Code für die Datentyp-Definition.
        string GetSqlDataTypeDefinition(); 

        #endregion Methods
    }
}