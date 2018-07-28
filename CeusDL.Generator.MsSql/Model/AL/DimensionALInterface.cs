using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.AL
{
    public class DimensionALInterface : IALInterface
    {        
        public DimensionALInterface(ALModel model, BT.RefBTAttribute refAttr) {
            this.Model = model;
            this.BTInterface = refAttr.ReferencedBTInterface;
            this.Core = refAttr.ReferencedBTInterface.coreInterface;                        
            this.Depth = 1;            
            this.RootDimension = this;
            this.ShortName = CalculateShortName(model, refAttr);
            this.Name = CalculateName(model, refAttr);
            PrepareAttributes();
        }

        internal DimensionALInterface(ALModel model, BT.RefBTAttribute refAttr, DimensionALInterface rootDimension) {
            this.Model = model;
            this.BTInterface = refAttr.ReferencedBTInterface;
            this.Core = refAttr.ReferencedBTInterface.coreInterface;                        
            this.Depth = rootDimension.Depth + 1;            
            this.RootDimension = rootDimension;
            this.ShortName = CalculateShortName(model, refAttr);
            this.Name = CalculateName(model, refAttr);
            PrepareAttributes();
        }

        private string CalculateName(ALModel model, BT.RefBTAttribute refAttr) {
            string name = "";
            if(string.IsNullOrEmpty(model.Config.Prefix)) {
                name = $"D_";
            } else {
                name = $"{model.Config.Prefix}_D_";
            }

            if(string.IsNullOrEmpty(refAttr?.refBLAttribute?.Core?.Alias)) {                
                name += $"{RootDimension.ShortName}_{Depth}_{refAttr.ReferencedBTInterface.ShortName}";                 
            } else {
                // TODO: Prüfen ob das gut ist so!
                name += $"{RootDimension.ShortName}_{Depth}_{refAttr.refBLAttribute.Core.Alias}_{refAttr.ReferencedBTInterface.ShortName}"; 
            }
            return name;
        }

        private string CalculateShortName(ALModel model, BT.RefBTAttribute refAttr) {            
            if(string.IsNullOrEmpty(refAttr?.refBLAttribute?.Core?.Alias)) {
                return refAttr.ReferencedBTInterface.ShortName;
            } else {
                return $"{refAttr.refBLAttribute.Core.Alias}_{refAttr.ReferencedBTInterface.ShortName}";
            }            
        }

        private void PrepareAttributes()
        {
            Attributes = new List<IALAttribute>();
            foreach(var attr in BTInterface.Attributes.OrderBy(a => a.GetBLAttribute().SortId)) {
                if(attr is BT.BaseBTAttribute) {                    
                    //Attributes.Add(new BaseALAttribute(this, (BT.BaseBTAttribute)attr));
                } else if(attr is BT.RefBTAttribute) {
                    var dim = new DimensionALInterface(Model, (BT.RefBTAttribute)attr, this);                 
                    dim = Model.GetDimensionInterfaceFor(dim);
                    //Attributes.Add(new RefALAttribute());
                } else {
                    throw new NotImplementedException($"DimensionALInterface.PrepareAttributes: Attributtyp {attr.GetType().Name} nicht unterstützt");
                }
            }
        }

        public ALModel Model { get; private set; }

        // Verschachtelungstiefe ab Fakttabelle
        public int Depth { get; private set; }

        // Dimensionsknoten, der direkt an der Fakttabelle anliegt
        public DimensionALInterface RootDimension { get; private set; }

        public CoreInterface Core { get; private set; }

        public BTInterface BTInterface { get; private set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public List<IALAttribute> Attributes { get; private set; }
    }
}