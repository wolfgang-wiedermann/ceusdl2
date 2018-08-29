using System;
using System.Collections.Generic;
using System.Linq;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Validator {
    public class CoreInterfaceValidator
    {
        public static void Validate(CoreInterface ifa, CoreModel m, ValidationResultRepository repo)
        {
            if(ifa.IsHistorized && !HasFinestTimeUnit(m)) {
                repo.AddError($"Your ceusdl code contains historized interfaces ({ifa.Name}) but no TemporalTable which is the finest_time_attribute", ValidationResult.OT_INTERFACE);
            }

            if(HasNoPrimaryKey(ifa)) {
                repo.AddError($"Your ceusdl code for {ifa.Name} does not have a primary key definition", ValidationResult.OT_INTERFACE);
            }

            if(DimHasNonSingleAttributePrimaryKey(ifa)) {
                repo.AddError($"Your ceusdl code for {ifa.Name} must have a one attribute primary key", ValidationResult.OT_INTERFACE);
            }
        }

        private static bool DimHasNonSingleAttributePrimaryKey(CoreInterface ifa)
        {
            var types = new List<CoreInterfaceType>() {
                CoreInterfaceType.DEF_TABLE, CoreInterfaceType.DIM_TABLE, CoreInterfaceType.DIM_VIEW, CoreInterfaceType.TEMPORAL_TABLE
            };

            return types.Contains(ifa.Type) && ifa.Attributes.Where(a => a.IsPrimaryKey).Count() > 1;
        }

        private static bool HasNoPrimaryKey(CoreInterface ifa)
        {
            return ifa.Attributes.Where(a => a.IsPrimaryKey).Count() == 0;
        }

        private static bool HasFinestTimeUnit(CoreModel m)
        {
            return m.Interfaces.Where(i => i.IsFinestTime).Count() > 0;
        }
        
    }
}