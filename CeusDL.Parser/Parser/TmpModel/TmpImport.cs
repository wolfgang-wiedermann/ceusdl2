using System.Collections.Generic;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public class TmpImport : ITmpBaseObject {
        public string Path { get; set; }
        public TmpParserResult Content { get; set; }
        public string WhitespaceBefore { get; set; }
    }
}