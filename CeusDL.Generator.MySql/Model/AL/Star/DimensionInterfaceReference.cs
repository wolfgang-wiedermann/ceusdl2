

namespace KDV.CeusDL.Model.MySql.AL.Star {
    public class DimensionInterfaceReference {
        public MySql.BT.BTInterface ParentBTInterface { get; set; }
        public MySql.BT.BTInterface ReferencedBTInterface { get; set; }
        public string ParentJoinAlias { get; set; }
        public string JoinAlias { get; set; }
        public string ParentRefColumnName { get; set; }
        public string ReferencedRefColumnName { get; set; }
    }
}