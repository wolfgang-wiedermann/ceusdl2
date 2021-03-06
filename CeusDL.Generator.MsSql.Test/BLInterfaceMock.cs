using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;

namespace KDV.CeusDL.Model.BL {
    public class BLInterfaceMock : IBLInterface
    {   
        #region Public Properties

        public string ShortName { get; set; }

        public string Name { get; set; }

        public string FullName {
            get {
                return $"dbo.{Name}";                
            }
        }

        public string ShortFormerName => throw new NotImplementedException();
        public string FormerName => throw new NotImplementedException();
        public string FullFormerName => throw new NotImplementedException();        
        public string RealFormerName { get; set; }
        public string DatabaseName => throw new NotImplementedException();

        public List<IBLAttribute> Attributes { get; private set; }

        public CoreInterfaceType InterfaceType {get; set; }

        public bool IsHistorized => throw new NotImplementedException();

        public IBLAttribute HistoryAttribute => throw new NotImplementedException();

        public bool IsMandant => throw new NotImplementedException();

        public List<IBLAttribute> PrimaryKeyAttributes => throw new NotImplementedException();
        public List<IBLAttribute> UpdateCheckAttributes => throw new NotImplementedException();

        public int MaxReferenceDepth => throw new NotImplementedException();

        public BLModel ParentModel { get; private set; }

        public List<IBLAttribute> UniqueKeyAttributes => throw new NotImplementedException();

        public string ViewName => throw new NotImplementedException();

        public string FullViewName => throw new NotImplementedException();

        public bool IsCalculated => throw new NotImplementedException();

        #endregion

        public BLInterfaceMock() {
        } 

        public void PostProcess()
        {
            foreach(var attr in Attributes) {
                attr.PostProcess();
            } 
        }

        public ILInterface GetILInterface()
        {
            throw new NotImplementedException();
        }

        public CoreInterface GetCoreInterface() 
        {
            throw new NotImplementedException();
        }
    }
}