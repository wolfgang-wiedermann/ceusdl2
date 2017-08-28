using System.Collections.Generic;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public enum TmpCommentType {
        LINE_COMMENT, BLOCK_COMMENT
    }

    public class TmpComment {
        public TmpCommentType CommentType { get; set; }
        public string Comment {get; set;}

        ///
        /// Whitespace-Zeichen (Leer, Newline, Tab etc.)
        /// vor und nach dem Kommentar (um exakte Wiedergabe im
        /// generierten ceusdl-Code zur ermöglichen)
        ///
        public string WhitespacesBeforeComment {get; set;}
        public string WhitespacesBehindComment {get; set;}
    }
}