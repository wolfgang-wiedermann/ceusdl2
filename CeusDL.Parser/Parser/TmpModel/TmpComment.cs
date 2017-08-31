using System.Collections.Generic;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public enum TmpCommentType {
        LINE_COMMENT, BLOCK_COMMENT
    }

    public class TmpComment : ITmpBaseObject {
        public TmpCommentType CommentType { get; set; }
        public string Comment {get; set;}
        public string WhitespaceBefore { get; set; }
    }
}