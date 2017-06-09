using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreFactAttribute : CoreBaseAttribute
    {
        public CoreFactAttribute(TmpInterfaceAttribute tmp, CoreInterface parent, CoreModel model) : base(tmp, parent, model)
        {
        }
    }
}