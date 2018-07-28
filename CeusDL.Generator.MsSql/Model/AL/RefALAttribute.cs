

using KDV.CeusDL.Model.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.AL {
    public class RefALAttribute : IALAttribute
    {
        public RefALAttribute()
        {
        }

        public CoreAttribute Core => throw new System.NotImplementedException();

        public IALInterface ParentInterface => throw new System.NotImplementedException();

        public IBTAttribute BTAttribute => throw new System.NotImplementedException();

        public string Name => throw new System.NotImplementedException();

        public string SqlType => throw new System.NotImplementedException();

        public string SqlTypeDefinition => throw new System.NotImplementedException();
    }
}