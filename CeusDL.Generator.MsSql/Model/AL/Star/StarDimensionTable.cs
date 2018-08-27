using System.Linq;
using System.Collections.Generic;

namespace KDV.CeusDL.Model.AL.Star
{
    public class StarDimensionTable
    {
        private DimensionALInterface dim;
        public StarDimensionTable(DimensionALInterface dim) {
            this.dim = dim;

            Attributes = new List<IALAttribute>();
            Attributes.AddRange(dim.Attributes.Where(a => a is BaseALAttribute));
            Attributes.AddRange(dim.Attributes.Where(a => a is RefALAttribute)
                                              .Select(a => (RefALAttribute)a)                                              
                                              .SelectMany(r => RecurseDimensions(r)));                
        }

        public List<IALAttribute> Attributes { get; private set; }

        public string Name => dim.Name;
        public IALAttribute IdColumn => dim.IdColumn;

        private List<IALAttribute> RecurseDimensions(RefALAttribute r) {
            var result = new List<IALAttribute>();
            result.AddRange(r.ReferencedDimension.Attributes.Where(a => a is BaseALAttribute && a.Name != "Mandant_ID"));
            var temp = r.ReferencedDimension
                        .Attributes.Where(a => a is RefALAttribute)
                                   .Select(a => (RefALAttribute)a.Clone(dim)).ToList();
            
            foreach(var tempR in temp) {
                result.AddRange(RecurseDimensions(tempR));
            }

            return result;
        }
    }
}