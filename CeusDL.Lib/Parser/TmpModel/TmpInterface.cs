using System.Collections.Generic;
using System.Linq;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public class TmpInterface {
        public string Name {get;set;}
        public string Type {get;set;}

        public List<TmpNamedParameter> Parameters {get;set;}

        public List<TmpItemLevelObject> ItemObjects {get; set;}

        public List<TmpInterfaceAttribute> Attributes {
            get {
                return ItemObjects.Where(i => i.IsAttribute).Select(i => i.Attribute).ToList();
            }
        }

        public void AddAttribute(TmpInterfaceAttribute attr) {
            ItemObjects.Add(new TmpItemLevelObject(attr));
        }

        public void AddComment(TmpComment comment) {
            ItemObjects.Add(new TmpItemLevelObject(comment));
        }
    }
}