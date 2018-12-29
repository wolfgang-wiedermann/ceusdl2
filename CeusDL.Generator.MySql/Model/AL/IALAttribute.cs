

namespace KDV.CeusDL.Model.MySql.AL {
    public interface IALAttribute {
        Core.CoreAttribute Core { get; }
        IALInterface ParentInterface { get; }
        MySql.BT.IBTAttribute BTAttribute { get; }
        string Name { get; }       
        string SqlType { get; }
        bool IsFact { get; }
        string JoinAlias { get; set; }

        IALAttribute Clone(IALInterface newParent);
    }
}