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

            foreach(var illegal in ContainsIllegalAttributeName(ifa)) {
                repo.AddError($"The interface {ifa.Name} contains the illegal attribute name {illegal}, which "
                    + "is preserved for the ceusdl generation process", ValidationResult.OT_ATTRIBUTE);
            }

            foreach(var illegal in ContainsIllegalAttributeType(ifa)) {
                repo.AddError($"The interface {ifa.Name} contains an attribute of type {illegal.GetType().Name}, which is illegal for a {ifa.Type} Interface", ValidationResult.OT_ATTRIBUTE);
            }

            foreach(var illegal in ContainsIllegalReference(ifa)) {
                repo.AddError($"The interface {ifa.Name} ({ifa.Type}) contains an illegal reference "
                    + $"to {illegal.ReferencedInterface.Name}.{illegal.ReferencedAttribute.Name} ({illegal.ReferencedInterface.Type})", ValidationResult.OT_ATTRIBUTE);
            }

            foreach(var duplicate in FindDuplicateAttributes(ifa, m)) {
                repo.AddError($"Your code for {ifa.Name} contains more then one definition of {duplicate} as illegal duplicates", ValidationResult.OT_ATTRIBUTE);
            }

            CheckFactsWithInvalidDataType(ifa, repo);
        }

        private static List<CoreRefAttribute> ContainsIllegalReference(CoreInterface ifa)
        { 
            var result = new List<CoreRefAttribute>();

            // Abbruch, wenn keine Referenzen enthalten sind:
            var refAttributes = ifa.Attributes.Where(a => a is CoreRefAttribute).Select(a => (CoreRefAttribute)a);
            if(refAttributes.Count() == 0) {
                return result;
            }

            switch(ifa.Type) {
                case CoreInterfaceType.FACT_TABLE:
                    break;
                case CoreInterfaceType.DIM_TABLE:
                case CoreInterfaceType.DIM_VIEW:
                    result.AddRange(refAttributes.Where(a => a.ReferencedInterface.Type == CoreInterfaceType.FACT_TABLE).ToList());
                    break;
                case CoreInterfaceType.DEF_TABLE:
                case CoreInterfaceType.TEMPORAL_TABLE:
                    result.AddRange(refAttributes.Where(a => a.ReferencedInterface.Type == CoreInterfaceType.FACT_TABLE).ToList());
                    result.AddRange(refAttributes.Where(a => a.ReferencedInterface.Type == CoreInterfaceType.DIM_TABLE).ToList());
                    result.AddRange(refAttributes.Where(a => a.ReferencedInterface.Type == CoreInterfaceType.DIM_VIEW).ToList());
                    break;
            }
            return result;
        }

        private static List<CoreAttribute> ContainsIllegalAttributeType(CoreInterface ifa) {
            var result = new List<CoreAttribute>();
            if(ifa.Type != CoreInterfaceType.FACT_TABLE) {
                result.AddRange(ifa.Attributes.Where(a => a is CoreFactAttribute).ToList());
            }
            return result;
        }

        private static List<string> ContainsIllegalAttributeName(CoreInterface ifa)
        {
            var illegals = new string[]{"ID"};
            return ifa.Attributes
                      .Where(a => illegals.Contains(a.Name?.ToUpper()))
                      .Select(a => a.Name)
                      .Distinct()
                      .ToList();
        }

        private static void CheckFactsWithInvalidDataType(CoreInterface ifa, ValidationResultRepository repo)
        {
            foreach(var attr in ifa.Attributes.Where(a => a is CoreFactAttribute).Select(a => (CoreFactAttribute)a)) {
                if(attr.DataType != CoreDataType.INT && attr.DataType != CoreDataType.DECIMAL) {
                    repo.AddError($"The Fact {attr.Name} in {ifa.Name} has the DataType {attr.DataType} which is non numerical", ValidationResult.OT_ATTRIBUTE);
                }
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