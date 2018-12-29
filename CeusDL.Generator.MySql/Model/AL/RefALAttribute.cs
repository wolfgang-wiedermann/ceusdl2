

using KDV.CeusDL.Model.MySql.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.MySql.AL {
    public class RefALAttribute : IALAttribute
    {
        public RefALAttribute(IALInterface parentInterface, DimensionALInterface referencedDim, MySql.BT.RefBTAttribute btAttribute)
        {
            this.ParentInterface = parentInterface;
            this.ReferencedDimension = referencedDim;
            this.BTAttribute = btAttribute;
            this.Name = ReferencedDimension.IdColumn.Name;    
        }

        public CoreAttribute Core => throw new System.NotImplementedException();

        public IALInterface ParentInterface { get; private set; }

        public DimensionALInterface ReferencedDimension { get; private set; }
        
        public IBTAttribute BTAttribute { get; private set; }

        public string Name { get; private set; }

        public string SqlType => ((RefBTAttribute)BTAttribute).IdAttribute.SqlDataType;

        public bool IsFact => false;

        public string JoinAlias { get; set; }

        public IALAttribute Clone(IALInterface newParent)
        {
            var clone = new RefALAttribute(newParent, ReferencedDimension, (RefBTAttribute)BTAttribute);
            clone.Name = this.Name;
            return clone;
        }
    }
}