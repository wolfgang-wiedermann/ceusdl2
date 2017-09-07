using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public class RefBLAttribute : IBLAttribute
    {
        private CoreRefAttribute coreAttribute = null;

        #region Public Properties
        public string ShortName {
            get {
                if(!string.IsNullOrEmpty(coreAttribute.Alias)) {
                    return coreAttribute.Alias;
                } else {
                    return $"{coreAttribute.ReferencedInterface.Name}.{coreAttribute.ReferencedAttribute.Name}";
                }
            }            
        }

        public string Name {
            get {                                
                if(string.IsNullOrEmpty(coreAttribute.Alias)) {
                    return ReferencedAttribute.Name;
                } else {
                    return $"{coreAttribute.Alias}_{ReferencedAttribute.Name}";
                } 
            }
        }

        public string FullName {
            get {
                return $"{ParentInterface.Name}.{Name}";
            }
        }

        public CoreDataType DataType {
            get {
                return ReferencedAttribute.DataType;
            }
        }

        public int Length {
            get {
                return ReferencedAttribute.Length;
            }
        }

        public int Decimals {
            get {
                return ReferencedAttribute.Decimals;
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

        // WICHTIG: Wird erst im PostProcessing gesetzt !!!
        public IBLAttribute ReferencedAttribute { get; private set; }

        public int SortId {
            get {
                if(this.coreAttribute.IsPrimaryKey) {
                    return 1;
                } else {
                    return 50;
                }
            }
        }

        #endregion

        public RefBLAttribute(CoreRefAttribute coreAttribute, IBLInterface parentInterface) {
            this.coreAttribute = coreAttribute;
            this.ParentInterface = parentInterface;
        }

        public string GetSqlDataTypeDefinition()
        {
            return ReferencedAttribute.GetSqlDataTypeDefinition();
        }

        public void PostProcess()
        {
            this.ReferencedAttribute = ParentInterface.ParentModel.Interfaces
                                            .Where(i => i.ShortName == coreAttribute.ReferencedInterface.Name)
                                            .First()
                                            .Attributes.Where(a => a.ShortName == coreAttribute.ReferencedAttribute.Name)
                                                       .First();
        }
    }
}