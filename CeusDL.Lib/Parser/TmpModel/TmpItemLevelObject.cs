using System.Collections.Generic;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public class TmpItemLevelObject {        
        private object content; 

        public TmpItemLevelObject(TmpInterfaceAttribute attr) {            
            content = attr;
        }

        public TmpItemLevelObject(TmpComment comment) {            
            content = comment;
        }

        public bool IsAttribute {
            get {
                return content is TmpInterfaceAttribute;
            }
        }

        public bool IsComment {
            get {
                return content is TmpComment;
            }
        }

        public TmpInterfaceAttribute Attribute {
            get {
                if(IsAttribute) {
                    return (TmpInterfaceAttribute) content;
                } else {
                    return null;
                }
            }
        }

        public TmpComment Comment {
            get {
                if(IsComment) {
                    return (TmpComment) content;
                } else {
                    return null;
                }
            }
        }
    }
}