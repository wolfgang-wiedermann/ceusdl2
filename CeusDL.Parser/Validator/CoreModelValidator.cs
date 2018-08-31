using System;
using System.Collections.Generic;
using System.Linq;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Validator {
    public class CoreModelValidator {
        public static ValidationResultRepository Validate(CoreModel m)
        {
            var repo = ValidationResultRepository.Instance;

            ValidateConfig(m, repo);

            if(!HasJustOneFinestTimeUnit(m)) {
                repo.AddError("Your ceusdl code contains more then one interface with finest_time_attribute=\"true\", that may cause errors", ValidationResult.OT_ELSE);
            }

            var duplicates = FindNonUniqueInterfaceNames(m);
            foreach(var name in duplicates) {
                repo.AddError($"The interface name \"{name}\" is non unique", ValidationResult.OT_INTERFACE);
            }

            foreach(var ifa in m.Interfaces) {
                ValidateInterface(ifa, m, repo);
            }

            return repo;
        }

        private static void ValidateInterface(CoreInterface ifa, CoreModel m, ValidationResultRepository repo)
        {
            CoreInterfaceValidator.Validate(ifa, m, repo);
        }

        private static void ValidateConfig(CoreModel m, ValidationResultRepository repo)
        {
            if (m.Config == null)
            {
                repo.AddError("The ceusdl-Code does not contain a config-Section.", ValidationResult.OT_CONFIG);
            }

            if (string.IsNullOrEmpty(m.Config.Prefix))
            {
                repo.AddInfo("The ceusdl config section does not contain a prefix definition. This is not mandatory but can be helpful.", ValidationResult.OT_CONFIG);
            }

            if (string.IsNullOrEmpty(m.Config.ILDatabase))
            {
                repo.AddInfo("The ceusdl config section does not contain a database name for interface layer.", ValidationResult.OT_CONFIG);
            }
            if (string.IsNullOrEmpty(m.Config.BLDatabase))
            {
                repo.AddInfo("The ceusdl config section does not contain a database name for base layer.", ValidationResult.OT_CONFIG);
            }
            if (string.IsNullOrEmpty(m.Config.BTDatabase))
            {
                repo.AddInfo("The ceusdl config section does not contain a database name for base layer transformation.", ValidationResult.OT_CONFIG);
            }
            if (string.IsNullOrEmpty(m.Config.ALDatabase))
            {
                repo.AddInfo("The ceusdl config section does not contain a database name for analytical layer.", ValidationResult.OT_CONFIG);
            }
        }

        private static bool HasJustOneFinestTimeUnit(CoreModel m) 
        {
            var count = m.Interfaces.Where(i => i.IsFinestTime).Count();
            return count == 0 || count == 1;
        }

        private static ICollection<string> FindNonUniqueInterfaceNames(CoreModel m) {
            var result = new HashSet<string>();
            foreach(var ifa in m.Interfaces) {
                var tmp = m.Interfaces.Where(i => i.Name == ifa.Name);
                if(tmp.Count() > 1) {
                    result.Add(ifa.Name);
                }
            }
            return result;
        }
    }
}