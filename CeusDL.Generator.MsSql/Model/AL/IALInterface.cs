

using System.Collections.Generic;

namespace KDV.CeusDL.Model.AL {
    public interface IALInterface {
        Core.CoreInterface Core { get; }
        
        BT.BTInterface BTInterface { get; }
        string Name { get; }       
        List<IALAttribute> Attributes { get; }
    }
}