

using System.Collections.Generic;

namespace KDV.CeusDL.Model.AL {
    public interface IALInterface {
        Core.CoreInterface Core { get; }
        
        BT.BTInterface BTInterface { get; }
        string ShortName { get; }
        string Name { get; }
        string FullName { get; }
        bool IsWithNowTable { get; }
        List<IALAttribute> Attributes { get; }
    }
}