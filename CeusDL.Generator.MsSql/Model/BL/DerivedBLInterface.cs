using System;
using System.Collections.Generic;
using System.Linq;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;

namespace KDV.CeusDL.Model.BL
{
    public class DerivedBLInterface : IBLInterface
    {
        private CoreInterface coreInterface;               

        public DerivedBLInterface(DefaultBLInterface defaultInterface, BLModel parentModel)
        {
            this.DefaultInterface = defaultInterface;
            this.ParentModel = parentModel;
            this.coreInterface = defaultInterface.coreInterface;

            this.ShortName = $"{coreInterface.Name}_VERSION";
            this.Name = ConvertName(coreInterface, parentModel.Config);
            ConvertAttributes(); // Laden von this.Attributes 
        }

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

        public DefaultBLInterface DefaultInterface { get; private set; }

        public CoreInterfaceType InterfaceType {
            get {
                return DefaultInterface.InterfaceType;
            }
        }

        public bool IsHistorized {
            get {
                return DefaultInterface.IsHistorized;
            }
        }

        public IBLAttribute HistoryAttribute { get; private set; }

        public bool IsMandant {
            get {
                return this.DefaultInterface.IsMandant;
            }
        }

        public List<IBLAttribute> PrimaryKeyAttributes { get; private set; }

        public List<IBLAttribute> UniqueKeyAttributes {           
            get {
                return this.Attributes
                    .Where(a => a.IsPartOfUniqueKey)
                    .OrderBy(a => a.SortId)
                    .ToList<IBLAttribute>();
            } 
        }        
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
                return this.DefaultInterface.MaxReferenceDepth;
            }
        }

        public BLModel ParentModel { get; private set; }

        public string ViewName => Name+"_VW";

        public string FullViewName => FullName+"_VW";

        #endregion

        public void PostProcess()
        {            
            foreach(var attr in this.Attributes) {
                attr.PostProcess();                
            }
        }

        #region Private Methods

        private string ConvertName(CoreInterface coreInterface, BLConfig config)
        {
            string prefix = "";
            if(!string.IsNullOrEmpty(config?.Prefix)) {
                prefix = $"{config.Prefix}_";
            }

            if(coreInterface.Type == CoreInterfaceType.DIM_TABLE) {
                return $"{prefix}BL_D_{coreInterface.Name}_VERSION";            
            } else {
                throw new InvalidInterfaceTypeException("Ungültiger Interface-Typ in ConvertName");
            }
        }

        private void ConvertAttributes() {            
            this.Attributes = new List<IBLAttribute>();
            this.PrimaryKeyAttributes = new List<IBLAttribute>();

            // ID-Attribut hinzufügen
            var pk = CustomBLAttribute.GetNewIDAttribute(this);
            this.Attributes.Add(pk);
            this.PrimaryKeyAttributes.Add(pk);


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
            this.Attributes.Add(CustomBLAttribute.GetNewTModifikationAttribute(this));
            this.Attributes.Add(CustomBLAttribute.GetNewTBemerkungAttribute(this));
            this.Attributes.Add(CustomBLAttribute.GetNewTBenutzerAttribute(this));                
            this.Attributes.Add(CustomBLAttribute.GetNewTSystemAttribute(this));                    
            this.HistoryAttribute = CustomBLAttribute.GetNewTGueltigBisAttribute(this, this.ParentModel.FinestTimeAttribute);
            this.Attributes.Add(this.HistoryAttribute);            
            this.Attributes.Add(CustomBLAttribute.GetNewTErstDatAttribute(this));
            this.Attributes.Add(CustomBLAttribute.GetNewTAendDatAttribute(this));
            this.Attributes.Add(CustomBLAttribute.GetNewTLadelaufNRAttribute(this));   
        }

        public ILInterface GetILInterface()
        {
            return DefaultInterface.GetILInterface();
        }

        #endregion
    }
}