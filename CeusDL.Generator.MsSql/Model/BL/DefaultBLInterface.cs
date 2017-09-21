using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;

namespace KDV.CeusDL.Model.BL {
    public class DefaultBLInterface : IBLInterface
    {   
        #region Private Attributes
        internal CoreInterface coreInterface = null;
        internal IL.ILInterface ilInterface = null;

        #endregion
        #region Public Properties

        public string ShortName { get; set; }

        public string Name { get; set; }

        public string FullName {
            get {
                if(ParentModel?.Config?.BLDatabase != null) {
                    return $"{ParentModel.Config.BLDatabase}.dbo.{Name}";
                } else {
                    return $"dbo.{Name}";
                }
            }
        }

        public List<IBLAttribute> Attributes { get; private set; }

        public CoreInterfaceType InterfaceType { 
            get {
                return this.coreInterface.Type;
            } 
        }

        public bool IsHistorized {
            get {
                return this.coreInterface.IsHistorized;
            }
        }

        public IBLAttribute HistoryAttribute {
            get {
                if(IsHistorized) {
                    // Fraglich, ob das schon funktioniert!
                    return Attributes.Where(a => a.ShortName == this.coreInterface.HistoryBy.Name).First();
                } else {
                    return null;
                }
            }
        }

        public bool IsMandant {
            get {
                return this.coreInterface.IsMandant;
            }
        }

        public List<IBLAttribute> PrimaryKeyAttributes {
            get {
                return this.Attributes.Where(a => a.IsPrimaryKey).ToList<IBLAttribute>();
            }
        }

        public List<IBLAttribute> UniqueKeyAttributes {
            get {
                return this.Attributes
                    .Where(a => a.IsPartOfUniqueKey)
                    .OrderBy(a => a.SortId)
                    .ToList<IBLAttribute>();
            }
        }

        public int MaxReferenceDepth {
            get {
                IBLInterface ifa = this;
                if(ifa.Attributes.Where(a => a is RefBLAttribute).Count() > 0) {
                    // TODO: Achtung, das ist schöner Code, berücksichtigt aber nicht
                    //       das Problem einer möglichen Endlos-Rekursion durch zyklische Referenzen
                    return ifa.Attributes
                        .Where(a => a is RefBLAttribute)
                        .Select(a => ((RefBLAttribute)a).ReferencedAttribute.ParentInterface)
                        .Max(a => a.MaxReferenceDepth)+1;
                } else {
                    return 0;
                }
            }
        }

        public BLModel ParentModel { get; private set; }

        #endregion       

        public DefaultBLInterface(CoreInterface coreInterface, BLModel model) {
            this.coreInterface = coreInterface;
            this.ParentModel = model;
            this.ShortName = coreInterface.Name;
            this.Name = ConvertName(coreInterface, model?.Config);
            this.Attributes = new List<IBLAttribute>();

            // ID-Attribut hinzufügen
            this.Attributes.Add(CustomBLAttribute.GetNewIDAttribute(this));

            foreach(var attr in coreInterface.Attributes) {
                if(attr is CoreRefAttribute) {
                    this.Attributes.Add(new RefBLAttribute((CoreRefAttribute) attr, this));
                } else {
                    this.Attributes.Add(new BaseBLAttribute((CoreBaseAttribute) attr, this));
                }
            }

            // Ggf. Mandant-Attribut hinzufügen
            if(coreInterface.IsMandant) {
                this.Attributes.Add(CustomBLAttribute.GetNewMandantAttribute(this));
            }

            // Technische Hilfsattribute hinzufügen
            AddTechnicalAttributes();
        }

        #region Methoden

        private string ConvertName(CoreInterface coreInterface, BLConfig config)
        {
            string prefix = "";
            if(!string.IsNullOrEmpty(config?.Prefix)) {
                prefix = $"{config.Prefix}_";
            }
            if(coreInterface.Type == CoreInterfaceType.DEF_TABLE
                || coreInterface.Type == CoreInterfaceType.TEMPORAL_TABLE) {                
                return $"{prefix}def_{coreInterface.Name}";
            } else if(coreInterface.Type == CoreInterfaceType.DIM_TABLE) {
                return $"{prefix}BL_D_{coreInterface.Name}";
            } else if(coreInterface.Type == CoreInterfaceType.DIM_VIEW) {
                return $"{prefix}BL_D_{coreInterface.Name}";
            } else if(coreInterface.Type == CoreInterfaceType.FACT_TABLE) {
                return $"{prefix}BL_F_{coreInterface.Name}";
            } else {
                throw new InvalidInterfaceTypeException("Ungültiger Interface-Typ in ConvertName");
            }
        }

        private void AddTechnicalAttributes()
        {
            if(coreInterface.Type == CoreInterfaceType.DEF_TABLE
                || coreInterface.Type == CoreInterfaceType.TEMPORAL_TABLE) {
                this.Attributes.Add(CustomBLAttribute.GetNewTBenutzerAttribute(this));                
                this.Attributes.Add(CustomBLAttribute.GetNewTSystemAttribute(this));
                this.Attributes.Add(CustomBLAttribute.GetNewTErstDatAttribute(this));
                this.Attributes.Add(CustomBLAttribute.GetNewTAendDatAttribute(this));
            } else if(coreInterface.Type == CoreInterfaceType.DIM_TABLE
                || coreInterface.Type == CoreInterfaceType.DIM_VIEW
                || coreInterface.Type == CoreInterfaceType.FACT_TABLE) {
                this.Attributes.Add(CustomBLAttribute.GetNewTModifikationAttribute(this));
                this.Attributes.Add(CustomBLAttribute.GetNewTBemerkungAttribute(this));
                this.Attributes.Add(CustomBLAttribute.GetNewTBenutzerAttribute(this));                
                this.Attributes.Add(CustomBLAttribute.GetNewTSystemAttribute(this));
                this.Attributes.Add(CustomBLAttribute.GetNewTErstDatAttribute(this));
                this.Attributes.Add(CustomBLAttribute.GetNewTAendDatAttribute(this));
                this.Attributes.Add(CustomBLAttribute.GetNewTLadelaufNRAttribute(this));                            
            }
        }

        public void PostProcess()
        {
            foreach(var attr in Attributes) {
                attr.PostProcess();
            } 
        }

        public ILInterface GetILInterface()
        {
            if(ilInterface == null) {
                ilInterface = new IL.ILInterface(this.coreInterface, this.coreInterface.CoreModel);
            }
            return ilInterface;
        }

        #endregion
    }
}