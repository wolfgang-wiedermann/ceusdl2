using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreFactAttribute : CoreBaseAttribute
    {
        public string DefaultValue { get; private set; }
        public CoreFactAttribute(TmpInterfaceAttribute tmp, CoreInterface parent, CoreModel model) : base(tmp, parent, model)
        {
            if(tmp.Parameters != null) {
                var x = tmp.Parameters.Where(p => p.Name.Equals("default"));
                if(x.Count() > 0) {
                    DefaultValue = x.First().Value;
                } else {
                    DefaultValue = null;
                }
            }
        }
    }
}