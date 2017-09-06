using System;
using System.Collections.Generic;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public class DefaultBLInterface : IBLInterface
    {
        public string ShortName { get; set; }

        public string Name { get; set; }

        public string FullName => throw new NotImplementedException();

        public List<IBLAttribute> Attributes => throw new NotImplementedException();

        public CoreInterfaceType InterfaceType {get; set; }

        public bool IsHistorized => throw new NotImplementedException();

        public IBLAttribute HistoryAttribute => throw new NotImplementedException();

        public bool IsMandant => throw new NotImplementedException();

        public List<IBLAttribute> PrimaryKeyAttributes => throw new NotImplementedException();

        public int MaxReferenceDepth => throw new NotImplementedException();
    }
}