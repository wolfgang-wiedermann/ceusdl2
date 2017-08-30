using System.Collections.Generic;
using System.Linq;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public class TmpParserResult {

        public List<TmpMainLevelObject> Objects { get; set; }
        public TmpConfig Config {get;set;}
        public List<TmpInterface> Interfaces {
            get {
                return Objects.Where(o => o.IsInterface).Select(o => o.Interface).ToList();
            }
        }

        public void AddInterface(TmpInterface ifa) {
            Objects.Add(new TmpMainLevelObject(ifa));
        }   

        public void AddComment(TmpComment comment) {
            Objects.Add(new TmpMainLevelObject(comment));
        }      
    }
}