

namespace KDV.CeusDL.Model.AL {
    public interface IALAttribute {
        Core.CoreAttribute Core { get; }
        IALInterface ParentInterface { get; }
        BT.IBTAttribute BTAttribute { get; }
        string Name { get; }       
        string SqlType { get; } 
        string SqlTypeDefinition { get; }
    }
}