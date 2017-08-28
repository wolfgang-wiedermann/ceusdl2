using System.Collections.Generic;

namespace KDV.CeusDL.Parser.TmpModel 
{
    public class TmpMainLevelObject {        
        private object content; 

        public TmpMainLevelObject(TmpInterface ifa) {            
            content = ifa;
        }

        public TmpMainLevelObject(TmpComment comment) {            
            content = comment;
        }

        public bool IsInterface {
            get {
                return content is TmpInterface;
            }
        }

        public bool IsComment {
            get {
                return content is TmpComment;
            }
        }

        public TmpInterface Interface {
            get {
                if(IsInterface) {
                    return (TmpInterface) content;
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