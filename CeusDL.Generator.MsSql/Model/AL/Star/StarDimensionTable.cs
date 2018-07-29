using System.Linq;
using System.Collections.Generic;

namespace KDV.CeusDL.Model.AL.Star
{
    public class StarDimensionTable
    {
        private DimensionALInterface dim;
        public StarDimensionTable(DimensionALInterface dim) {
            this.dim = dim;
        }

        public List<IALAttribute> Attributes {
            get {
                var result = new List<IALAttribute>();
                result.AddRange(dim.Attributes.Where(a => a is BaseALAttribute));
                result.AddRange(dim.Attributes.Where(a => a is RefALAttribute)
                                              .Select(a => (RefALAttribute)a)                                              
                                              .SelectMany(r => RecurseDimensions(r)));
                return result;
            }
        }

        private List<IALAttribute> RecurseDimensions(RefALAttribute r) {
            var result = new List<IALAttribute>();
            result.AddRange(r.ReferencedDimension.Attributes.Where(a => a is BaseALAttribute));
            var temp = r.ReferencedDimension
                        .Attributes.Where(a => a is RefALAttribute)
                                   .Select(a => (RefALAttribute)a).ToList();
            
            foreach(var tempR in temp) {
                result.AddRange(RecurseDimensions(tempR));
            }

            return result;
        }
    }
}