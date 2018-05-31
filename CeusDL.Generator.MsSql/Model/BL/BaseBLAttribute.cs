using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;
using KDV.CeusDL.Model.Exceptions;

namespace KDV.CeusDL.Model.BL {
    public class BaseBLAttribute : IBLAttribute
    {
        #region Private Attributes
        private CoreBaseAttribute coreAttribute = null;
        #endregion

        #region Public Properties

        public string ShortName {
            get {
                return coreAttribute.Name;
            }            
        }

        public string Name { 
            get {
                return $"{this.coreAttribute.ParentInterface.Name}_{this.coreAttribute.Name}";
            }
        }

        public string FullName {
            get {
                if(ParentInterface != null) {
                    return $"{ParentInterface.Name}.{Name}";
                } else {
                    return $"UNKNOWN_TABLE.{Name}";
                }
            }
        }

        public string ShortFormerName => coreAttribute.FormerName;
        public string FormerName {
            get {
                if(this.coreAttribute.FormerName != null) {
                    if(coreAttribute.ParentInterface.FormerName != null) {
                        return $"{this.coreAttribute.ParentInterface.FormerName}_{this.coreAttribute.FormerName}";
                    } else {
                        return $"{this.coreAttribute.ParentInterface.Name}_{this.coreAttribute.FormerName}";
                    }
                } else if(coreAttribute.ParentInterface.FormerName != null) {
                    return $"{this.coreAttribute.ParentInterface.FormerName}_{this.coreAttribute.Name}";
                } else {
                    return null;
                }
            }
        }
        public string FullFormerName { 
            get {
                if(FormerName != null && ParentInterface.FormerName != null) {
                    return $"{ParentInterface.FormerName}.{FormerName}";
                } else if(FormerName == null && ParentInterface.FormerName != null) {
                    return $"{ParentInterface.FormerName}.{Name}";
                } else if(FormerName != null) {
                    return $"{ParentInterface.Name}.{FormerName}";                    
                } else {
                    return null;
                }
            }
        }

        public CoreDataType DataType {
            get {
                return this.coreAttribute.DataType;
            }
        }

        public int Length {
            get {
                return this.coreAttribute.Length;
            }
        }

        public int Decimals {
            get {
                return this.coreAttribute.Decimals;
            }
        }

        public bool IsPrimaryKey {
            get {
                // In der BL sind Attribute aus dem ceusdl-File nie PK
                return false;
            }
        }

        public bool IsPartOfUniqueKey {
            get {
                // In der BL wird der Primärschlüssel aus dem Quellsystem (ggf. ergänzt um Mandant und Historienattribut)
                // in einen Unique-Key umgewandelt.
                return this.coreAttribute.IsPrimaryKey;
            }
        }

        public bool IsIdentity {
            get {
                // In der BL sind Attribute aus dem ceusdl-File nie Identity
                return false;
            }
        }

        public IBLInterface ParentInterface { get; private set; }

        public int SortId {
            get {
                if(this.coreAttribute.IsPrimaryKey) {
                    return 1;
                } else {
                    return 50;
                }
            }
        }

        public bool IsTechnicalAttribute => false;

        #endregion

        public BaseBLAttribute(CoreBaseAttribute coreAttribute, IBLInterface parentInterface) {                        
            this.coreAttribute = coreAttribute;            
            this.ParentInterface = parentInterface;
        }

        public string GetSqlDataTypeDefinition()
        {
            // TODO: das hier ist noch nicht alles !!!
            return BLDataType.GenerateSqlDataTypeDefinition(this);
        }    

        public void PostProcess()
        {
            // Beim BaseBLAttribute gibts im PostProcessing nix zu tun.
        }

        public ILAttribute GetILAttribute()
        {
            // Zu technischen Attributen gibt es in der IL sowieso keine Entsprechung
            if(IsTechnicalAttribute) 
                return null;

            var ilInterface = this.ParentInterface.GetILInterface();
            var result = ilInterface.Attributes.Where(i => i.Core == this.coreAttribute);

            if(result.Count() > 1) 
                throw new InvalidCountException();

            if(result.Count() == 1) {
                return result.First();
            } else {
                return null;
            }
        }
    }
}