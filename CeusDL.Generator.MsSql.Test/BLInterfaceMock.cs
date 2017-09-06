using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;

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

        public List<IBLAttribute> Attributes { get; private set; }

        public CoreInterfaceType InterfaceType {get; set; }

        public bool IsHistorized => throw new NotImplementedException();

        public IBLAttribute HistoryAttribute => throw new NotImplementedException();

        public bool IsMandant => throw new NotImplementedException();

        public List<IBLAttribute> PrimaryKeyAttributes => throw new NotImplementedException();

        public int MaxReferenceDepth => throw new NotImplementedException();

        public BLModel ParentModel { get; private set; }

        #endregion       
        
        public BLInterfaceMock() {
        } 

        public void PostProcess()
        {
            foreach(var attr in Attributes) {
                attr.PostProcess();
            } 
        }        
    }
}