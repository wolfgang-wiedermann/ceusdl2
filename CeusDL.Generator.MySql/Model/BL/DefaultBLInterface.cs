using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.IL;

namespace KDV.CeusDL.Model.MySql.BL {
    public class DefaultBLInterface : IBLInterface
    {   
        #region Private Attributes
        internal CoreInterface coreInterface = null;
        internal ILInterface ilInterface = null;

        #endregion
        #region Public Properties

        public string ShortName { get; set; }

        public string Name { get; set; }

        public string FullName {
            get {
                if(ParentModel?.Config?.BLDatabase != null) {
                    return $"{ParentModel.Config.BLDatabase}.{Name}";
                } else {
                    return $"{Name}";
                }
            }
        }

        public string ShortFormerName => this.coreInterface.FormerName;

        public string FormerName { get; set; }

        public string FullFormerName { 
            get {
                if(FormerName == null) return null;
                if(ParentModel?.Config?.BLDatabase != null) {
                    return $"{ParentModel.Config.BLDatabase}.{FormerName}";
                } else {
                    return $"{FormerName}";
                }
            }
        }

        public string RealFormerName { get; set; }

        public string DatabaseName => ParentModel.Config.BLDatabase;

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
                // Immer noch fraglich ob das so passt
                if(IsHistorized && coreInterface.Type == CoreInterfaceType.FACT_TABLE) {                    
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

        // Liste der Attribute, für die auf Update-Geprüft werden soll
        // (für Dimensionstabellen mit und ohne Historisierung)
        public List<IBLAttribute> UpdateCheckAttributes { 
            get {
                return this.Attributes
                    .Where(a => !a.IsPrimaryKey 
                           && !a.IsIdentity 
                           && !a.IsPartOfUniqueKey 
                           && !a.IsTechnicalAttribute)
                    .OrderBy(a => a.SortId)
                    .ToList();
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

        public string ViewName => Name+"_VW";

        public string FullViewName => FullName+"_VW";

        #endregion

        public DefaultBLInterface(CoreInterface coreInterface, BLModel model) {
            this.coreInterface = coreInterface;
            this.ParentModel = model;
            this.ShortName = coreInterface.Name;
            this.Name = ConvertName(coreInterface, model?.Config);

            if(string.IsNullOrEmpty(coreInterface.FormerName)) {
                this.FormerName = null;
            } else {
                this.FormerName = ConvertName(coreInterface.FormerName, coreInterface, model?.Config);
            }

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
            return ConvertName(coreInterface.Name, coreInterface, config);
        }

        private string ConvertName(string name, CoreInterface coreInterface, BLConfig config) {
            string prefix = "";
            if(!string.IsNullOrEmpty(config?.Prefix)) {
                prefix = $"{config.Prefix}_";
            }
            if(coreInterface.Type == CoreInterfaceType.DEF_TABLE
                || coreInterface.Type == CoreInterfaceType.TEMPORAL_TABLE) {                
                return $"{prefix}def_{name}";
            } else if(coreInterface.Type == CoreInterfaceType.DIM_TABLE) {
                return $"{prefix}BL_D_{name}";
            } else if(coreInterface.Type == CoreInterfaceType.DIM_VIEW) {
                return $"{prefix}BL_D_{name}";
            } else if(coreInterface.Type == CoreInterfaceType.FACT_TABLE) {
                return $"{prefix}BL_F_{name}";
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

        public CoreInterface GetCoreInterface() 
        {
            return this.coreInterface;
        }

        #endregion
    }
}