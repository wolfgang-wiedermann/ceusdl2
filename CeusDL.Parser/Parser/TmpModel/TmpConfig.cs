using System.Collections.Generic;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public class TmpConfig : ITmpBaseObject {
        public List<TmpNamedParameter> Parameters {get;set;}
        public string WhitespaceBefore { get; set; }
    }
}