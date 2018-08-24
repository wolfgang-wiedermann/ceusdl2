

using KDV.CeusDL.Model.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.AL {
    public class RefALAttribute : IALAttribute
    {
        public RefALAttribute(IALInterface parentInterface, DimensionALInterface referencedDim, BT.RefBTAttribute btAttribute)
        {
            this.ParentInterface = parentInterface;
            this.ReferencedDimension = referencedDim;
            this.BTAttribute = btAttribute;         
        }

        public CoreAttribute Core => throw new System.NotImplementedException();

        public IALInterface ParentInterface { get; private set; }

        public DimensionALInterface ReferencedDimension { get; private set; }
        
        public IBTAttribute BTAttribute { get; private set; }

        public string Name => ReferencedDimension.IdColumn.Name;

        public string SqlType => ((RefBTAttribute)BTAttribute).IdAttribute.SqlDataType;

        public bool IsFact => false;
    }
}