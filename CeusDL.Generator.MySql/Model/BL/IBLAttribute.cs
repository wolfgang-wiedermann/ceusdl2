using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.MySql.BL {
    public interface IBLAttribute {
        #region Properties

        string ShortName { get; } // Attributname in ceusdl-Syntax z. B. KNZ
        string Name { get; } // Attributname in BL-Syntax z. B. Semester_KNZ      
        string FullName { get; } // Attributname incl. Tabellenname getrennt durch Punkt
        string ShortFormerName { get; } // Früherer Attributname in ceusdl-Syntax lt. former_name="K" => K
        string FormerName { get; } // Früherer Attributname in BL-Syntax lt. former_name="K" => Semester_K
        string FullFormerName { get; } // Attributname incl. ggf. früheren Tabellennamen getrennt durch Punkt
        string RealFormerName { get; set; } // Attributname lt. Datenbankabgleich - benötigt für Insert into Select
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
        KDV.CeusDL.Model.MySql.IL.ILAttribute GetILAttribute();

        void PostProcess(); 

        #endregion Methods
    }
}