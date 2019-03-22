using System.Linq;
using System.Collections.Generic;

namespace KDV.CeusDL.Model.AL.Star
{
    public class StarDimensionTable
    {
        private DimensionALInterface dim;
        private int joinIdx;
        public StarDimensionTable(DimensionALInterface dim) {
            this.dim = dim;
            joinIdx = 0;
            var currentJoinAlias = $"t{joinIdx}";
            
            InterfaceReferences = new List<DimensionInterfaceReference>();
            Config = dim.Model.Config;

            Attributes = new List<IALAttribute>();
            // Basis-Attribute            
            var baseAttrs = dim.Attributes.Where(a => a is BaseALAttribute);
            foreach(var baseAttr in baseAttrs) {
                baseAttr.JoinAlias = currentJoinAlias;
            }            
            Attributes.AddRange(baseAttrs);

            // Referenz-Attribute            
            var refAttrs = dim.Attributes.Where(a => a is RefALAttribute)
                                         .Select(a => (RefALAttribute)a);                                          
            foreach(var refAttr in refAttrs) {
                joinIdx += 1;

                var temp = RecurseDimensions(refAttr);
                var joinAlias = temp.First().JoinAlias;

                InterfaceReferences.Add(new DimensionInterfaceReference() {
                    ParentBTInterface = refAttr.ParentInterface.BTInterface,
                    ReferencedBTInterface = ((BT.RefBTAttribute)refAttr.BTAttribute).ReferencedBTInterface,                    
                    JoinAlias = joinAlias,
                    ParentJoinAlias = currentJoinAlias,
                    ParentRefColumnName = ((BT.RefBTAttribute)refAttr.BTAttribute).IdAttribute.Name,
                    ReferencedRefColumnName = ((BT.BaseBTAttribute)((BT.RefBTAttribute)refAttr.BTAttribute).ReferencedBTAttribute).Name
                });

                Attributes.AddRange(temp);
            }

            // InterfaceReferences nach Alias sortieren!
            InterfaceReferences = InterfaceReferences.OrderBy(i => $"{i.JoinAlias.Length}{i.JoinAlias}").ToList();
        }

        public ALConfig Config { get; private set; }

        public List<IALAttribute> Attributes { get; private set; }

        public List<DimensionInterfaceReference> InterfaceReferences { get; private set; }

        public string Name => dim.Name;
        public IALAttribute IdColumn => dim.IdColumn;

        public BT.BTInterface MainBTInterface => dim.BTInterface;

        public bool ConstainsDimInterface(IALInterface ifa) {
            return this.dim.Name == ifa.Name 
                || this.Attributes
                       .Select(a => a.ParentInterface)
                       .Distinct()
                       .Where(i => i == ifa)
                       .Count() > 0;
        }

        private List<IALAttribute> RecurseDimensions(RefALAttribute r) {
            var result = new List<IALAttribute>();
            var currentJoinAlias = $"t{joinIdx}";

            // Base-Attribute behandeln
            var baseAttributes = r.ReferencedDimension.Attributes.Where(a => a is BaseALAttribute && a.Name != "Mandant_ID");
            foreach(var baseAttr in baseAttributes) {
                baseAttr.JoinAlias = currentJoinAlias;
            }
            result.AddRange(baseAttributes);

            // Referenz-Attribute behandeln
            var temp = r.ReferencedDimension
                        .Attributes.Where(a => a is RefALAttribute)
                                   .Select(a => (RefALAttribute)a.Clone(dim)).ToList();
            
            foreach(var tempR in temp) {
                joinIdx += 1;
                var joinAlias = $"t{joinIdx}";
                InterfaceReferences.Add(new DimensionInterfaceReference() {
                    ParentBTInterface = tempR.ParentInterface.BTInterface,
                    ReferencedBTInterface = ((BT.RefBTAttribute)tempR.BTAttribute).ReferencedBTInterface,                    
                    JoinAlias = joinAlias,
                    ParentJoinAlias = currentJoinAlias,
                    ParentRefColumnName = ((BT.RefBTAttribute)tempR.BTAttribute).IdAttribute.Name,
                    ReferencedRefColumnName = ((BT.BaseBTAttribute)((BT.RefBTAttribute)tempR.BTAttribute).ReferencedBTAttribute).Name
                });
                                
                result.AddRange(RecurseDimensions(tempR));
            }

            return result;
        }
    }
}