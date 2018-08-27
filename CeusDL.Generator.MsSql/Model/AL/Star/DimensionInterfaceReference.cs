

namespace KDV.CeusDL.Model.AL.Star {
    public class DimensionInterfaceReference {
        public BT.BTInterface ParentBTInterface { get; set; }
        public BT.BTInterface ReferencedBTInterface { get; set; }
        public string ParentJoinAlias { get; set; }
        public string JoinAlias { get; set; }
        public string ParentRefColumnName { get; set; }
        public string ReferencedRefColumnName { get; set; }
    }
}