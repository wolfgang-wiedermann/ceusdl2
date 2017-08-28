using System.Collections.Generic;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public class TmpParserResult {
        public TmpConfig Config {get;set;}
        public List<TmpInterface> Interfaces {get;set;}
    }
}