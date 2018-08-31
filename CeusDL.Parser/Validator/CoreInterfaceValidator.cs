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

            var duplicateAttributes = FindDuplicateAttributes(ifa, m);
            foreach(var duplicate in duplicateAttributes) {
                repo.AddError($"Your code for {ifa.Name} contains more then one definition of {duplicate} as illegal duplicates", ValidationResult.OT_ATTRIBUTE);
            }
        }

        private static List<string> FindDuplicateAttributes(CoreInterface ifa, CoreModel m)
        {
            var result = new HashSet<string>();

            foreach(var attr in ifa.Attributes) {
                if(attr is CoreBaseAttribute || attr is CoreFactAttribute) {
                    var matches = ifa.Attributes
                                     .Where(a => a is CoreBaseAttribute || a is CoreFactAttribute)
                                     .Select(a => (CoreBaseAttribute)a)
                                     .Where(a => a.Name == attr.Name);

                    if(matches.Count() > 1) {
                        result.Add(attr.Name);
                    }
                } else if(attr is CoreRefAttribute) {
                    var refAttr = (CoreRefAttribute)attr;
                    var matches = ifa.Attributes
                                     .Where(a => a is CoreRefAttribute)
                                     .Select(a => (CoreRefAttribute)a)
                                     .Where(a => a.Alias == refAttr.Alias 
                                        && a.ReferencedInterface.Name == refAttr.ReferencedInterface.Name
                                        && a.ReferencedAttribute.Name == refAttr.ReferencedAttribute.Name);

                    if(matches.Count() > 1) {
                        result.Add(attr.Name);
                    }
                }
            }

            return result.ToList();
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