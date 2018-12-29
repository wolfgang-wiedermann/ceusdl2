

namespace KDV.CeusDL.Model.MySql.AL {
    public class FactInterfaceReference {
        public MySql.BT.BTInterface BTInterface { get; set; }
        public string JoinAlias { get; set; }
        public string RefColumnName { get; set; }
    }
}