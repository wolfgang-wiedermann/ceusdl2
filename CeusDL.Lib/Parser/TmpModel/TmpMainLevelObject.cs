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

        public TmpMainLevelObject(TmpConfig config) {            
            content = config;
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

        public bool IsConfig {
            get {
                return content is TmpConfig;
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

        public TmpConfig Config {
            get {
                if(IsConfig) {
                    return (TmpConfig) content;
                } else {
                    return null;
                }
            }
        }
    }
}