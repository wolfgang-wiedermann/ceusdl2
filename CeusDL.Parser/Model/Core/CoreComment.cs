using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreComment : CoreItemLevelObject, CoreMainLevelObject {
        private TmpComment comment;
        public CoreComment(TmpComment comment) {
            this.WhitespaceBefore = comment.WhitespaceBefore;
            this.comment = comment;
        }

        public string Comment {
            get {
                return comment.Comment;
            }
        }

        public string WhitespaceBefore { get; set; }

        public override string ToString() {
            if(comment.CommentType == TmpCommentType.BLOCK_COMMENT) {
                return $"{WhitespaceBefore}/*{comment.Comment}*/";
            } else {
                return $"{WhitespaceBefore}//{comment.Comment}\n";
            }
        }
    }
}