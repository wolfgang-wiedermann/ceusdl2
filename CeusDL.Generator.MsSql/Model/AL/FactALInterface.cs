using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BT;

namespace KDV.CeusDL.Model.AL
{
    public class FactALInterface : IALInterface
    {                
        public FactALInterface(BTInterface ifa, ALModel alModel)
        {
            this.BTInterface = ifa;
            this.Model = alModel;

            PrepareAttributes();
        }

        public ALModel Model { get; private set; }

        public CoreInterface Core => BTInterface.coreInterface;

        public BT.BTInterface BTInterface { get; private set; }

        public string ShortName => BTInterface.ShortName;

        public string Name {
            get {
                string prefix = "";
                if(!string.IsNullOrEmpty(Model.Config.Prefix)) {
                    prefix = $"{Model.Config.Prefix}_";
                }
                return $"{prefix}AP_{ShortName}";
            }
        } 

        public List<IALAttribute> Attributes { get; private set; }

        // TODO: Was ist mit dem Einschachteln der Attribute übergeordneter Fakttabellen??!!
        private void PrepareAttributes()
        {
            Attributes = new List<IALAttribute>();
            foreach(var attr in BTInterface.Attributes.OrderBy(a => a.GetBLAttribute().SortId)) {
                if(attr is BT.BaseBTAttribute) {                    
                    Attributes.Add(new BaseALAttribute(this, (BT.BaseBTAttribute)attr));
                } else if(attr is BT.RefBTAttribute) {
                    // TODO: diesen elseif-Block evtl. auslagern in Methode
                    var refAttr = (BT.RefBTAttribute)attr;
                    if(refAttr.ReferencedBTInterface.InterfaceType == CoreInterfaceType.FACT_TABLE) {
                        // TODO: Integration der Attribute einer referenzierten Fakttabelle
                        //       ausprogrammieren
                    } else {
                        var dim = new DimensionALInterface(Model, refAttr);                 
                        dim = Model.GetDimensionInterfaceFor(dim);
                        Attributes.Add(new RefALAttribute(this, dim, refAttr));
                    }
                } else {
                    throw new NotImplementedException($"FactALInterface.PrepareAttributes: Attributtyp {attr.GetType().Name} nicht unterstützt");
                }
            }
        }
    }
}