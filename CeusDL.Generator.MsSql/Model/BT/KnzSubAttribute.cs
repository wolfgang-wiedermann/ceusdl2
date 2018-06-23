using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;

namespace KDV.CeusDL.Model.BT
{
    public class KnzSubAttribute : ISubAttribute
    {
        private RefBTAttribute refBTAttribute;
        private RefBLAttribute blAttribute;

        public KnzSubAttribute(RefBTAttribute refBTAttribute)
        {
            this.refBTAttribute = refBTAttribute;
            this.blAttribute = refBTAttribute.blAttribute;
        }

        public string ShortName => blAttribute.ReferencedAttribute.Name;

        public string Alias => blAttribute.Core.Alias;

        public string Name => blAttribute.Name;

        public string FullName => throw new NotImplementedException();

        public string SqlDataType => blAttribute.GetSqlDataTypeDefinition(); // Ob das so schon gut ist?

        public CoreDataType DataType => blAttribute.DataType;
    }
}
