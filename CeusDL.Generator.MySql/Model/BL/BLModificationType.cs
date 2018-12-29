using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.MySql.BL {
    public enum BLModificationType {
        // Unverändert, d.h. dieses Objekt braucht im UpdateBLGenerator nicht berücksichtigt werden
        UNMODIFIED, 
        // Umbenannt, d.h. der Name des Objekts muss mit Alter Table aktualisiert werden
        RENAME, 
        // Zukünftig für Änderungen, die keine komplette Neugenerierung des Objekts erfordern
        // NUR ALS NOTIZ FÜR SPÄTERE VERSIONEN, Wird vorerst mit RECREATE behandelt
        //UPDATE, 

        // Änderungen erfolgen eine Neuerstellung des Objekts (select into -> drop -> create -> insert into select)
        RECREATE,
        // Das Objekt gab es in der vorherigen Version des Schemas noch nicht, es wird einfach
        // neu angelegt.
        NEW
    }
}
