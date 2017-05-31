using System.Collections.Generic;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public class TmpInterface {
        public string Name {get;set;}
        public string Type {get;set;}

        public List<TmpNamedParameter> Parameters {get;set;}
        public List<TmpInterfaceAttribute> Attributes {get;set;}
    }
}