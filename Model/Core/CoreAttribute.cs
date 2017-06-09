using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public abstract class CoreAttribute {
        public abstract string Name { get; protected set; }
        public bool IsPrimaryKey {get; protected set;}
        public CoreInterface ParentInterface {get; protected set;}
        protected TmpInterfaceAttribute BaseData {get; set;}
        protected CoreModel coreModel;

        public CoreAttribute(TmpInterfaceAttribute tmp, CoreInterface parent, CoreModel model) {
            BaseData = tmp;
            ParentInterface = parent;
            coreModel = model;

            if(tmp.Parameters != null && tmp.Parameters.Where(a => a.Name == "primary_key" && a.Value == "true").Count() > 0) {
                IsPrimaryKey = true;
            }
        }
    }
}