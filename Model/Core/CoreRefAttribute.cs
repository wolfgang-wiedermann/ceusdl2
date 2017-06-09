using System;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreRefAttribute : CoreAttribute
    {
        public CoreRefAttribute(TmpInterfaceAttribute tmp, CoreInterface parent, CoreModel model) : base(tmp, parent, model)
        {
        }

        public override string Name { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
    }
}