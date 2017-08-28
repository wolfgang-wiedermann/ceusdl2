using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreComment : CoreItemLevelObject, CoreMainLevelObject {
        private TmpComment comment;
        public CoreComment(TmpComment comment) {
            this.comment = comment;
        }

        public string Comment {
            get {
                return comment.Comment;
            }
        }

        public override string ToString() {
            if(comment.CommentType == TmpCommentType.BLOCK_COMMENT) {
                return $"{comment.WhitespacesBeforeComment}/*{comment.Comment}*/{comment.WhitespacesBehindComment}";
            } else {
                return $"{comment.WhitespacesBeforeComment}//{comment.Comment}\n{comment.WhitespacesBehindComment}";
            }
        }
    }
}