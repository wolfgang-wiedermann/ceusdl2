using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BL {
    public class RefBLAttribute : IBLAttribute
    {
        public string Name => throw new NotImplementedException();

        public string FullName => throw new NotImplementedException();

        public CoreDataType DataType => throw new NotImplementedException();

        public int Length => throw new NotImplementedException();

        public int Decimals => throw new NotImplementedException();

        public bool IsPrimaryKey => throw new NotImplementedException();

        public bool IsIdentity => throw new NotImplementedException();

        public IBLInterface ParentInterface => throw new NotImplementedException();

        public bool IsPartOfUniqueKey => throw new NotImplementedException();

        public string GetSqlDataTypeDefinition()
        {
            throw new NotImplementedException();
        }
    }
}