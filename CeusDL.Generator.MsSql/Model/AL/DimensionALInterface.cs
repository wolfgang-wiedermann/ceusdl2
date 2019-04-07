using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.BT;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.Exceptions;

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
            this.Alias = CalculateAlias(model, refAttr);
            this.ShortName = CalculateShortName(model, refAttr);
            this.Name = CalculateName(model, refAttr);
            PrepareAttributes();
        }

        internal DimensionALInterface(ALModel model, BT.RefBTAttribute refAttr, DimensionALInterface rootDimension, int depth) {
            if(rootDimension == null) {
                rootDimension = this;
            }
            this.Model = model;
            this.BTInterface = refAttr.ReferencedBTInterface;
            this.Core = refAttr.ReferencedBTInterface.coreInterface;                        
            this.Depth = depth;            
            this.RootDimension = rootDimension;
            this.Alias = CalculateAlias(model, refAttr);
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
            name += $"{RootDimension.ShortName}_{Depth}_{this.ShortName}";                 
            return name;
        }

        private string CalculateShortName(ALModel model, BT.RefBTAttribute refAttr) {

            if(string.IsNullOrEmpty(refAttr?.refBLAttribute?.Core?.Alias) && refAttr.ReferencedBTInterface.IsHistoryTable) {
                return $"{refAttr.ReferencedBTInterface.ShortName}_VERSION";
            } else if(string.IsNullOrEmpty(refAttr?.refBLAttribute?.Core?.Alias)) {
                return refAttr.ReferencedBTInterface.ShortName;
            } else if(refAttr.ReferencedBTInterface.IsHistoryTable) {
                return $"{refAttr.refBLAttribute.Core.Alias}_{refAttr.ReferencedBTInterface.ShortName}_VERSION";
            } else {
                return $"{refAttr.refBLAttribute.Core.Alias}_{refAttr.ReferencedBTInterface.ShortName}";
            }            
        }

        private string CalculateAlias(ALModel model, BT.RefBTAttribute refAttr) {            
            if(string.IsNullOrEmpty(refAttr?.refBLAttribute?.Core?.Alias)) {
                return "";
            } else {
                return $"{refAttr.refBLAttribute.Core.Alias}_";
            }            
        }

        private void PrepareAttributes()
        {
            Attributes = new List<IALAttribute>();
            foreach(var attr in BTInterface.Attributes.OrderBy(a => a.GetBLAttribute().SortId)) {
                if(attr is BT.BaseBTAttribute) {                    
                    var baseAttr = new BaseALAttribute(this, (BT.BaseBTAttribute)attr);
                    Attributes.Add(baseAttr);
                    if(attr.IsIdentity && IdColumn == null) {
                        IdColumn = baseAttr;
                    } else if(attr.IsIdentity) {
                        throw new InvalidStateException($"Die Dimension {this.Name} hat mehr als eine Identity-Spalte");
                    }
                } else if(attr is BT.RefBTAttribute) {
                    var refAttr = (BT.RefBTAttribute)attr;
                    DimensionALInterface dim = null; 
                    // Prüfen, ob es sich um die Current-State-Table zur aktuellen Tabelle handelt
                    if(this.BTInterface.IsHistoryTable && refAttr.ReferencedBTInterface.IsCurrentStateTable 
                        && refAttr.ReferencedBTInterface.coreInterface == this.BTInterface.coreInterface) {
                            dim = new DimensionALInterface(Model, refAttr, null, this.Depth+1);
                    } else {
                        dim = new DimensionALInterface(Model, refAttr, this.RootDimension, this.Depth+1);                 
                    }
                    dim = Model.GetDimensionInterfaceFor(dim);
                    Attributes.Add(new RefALAttribute(this, dim, refAttr));
                } else {
                    throw new NotImplementedException($"DimensionALInterface.PrepareAttributes: Attributtyp {attr.GetType().Name} nicht unterstützt");
                }
            }
        }

        public BaseALAttribute IdColumn { get; private set; }
        public ALModel Model { get; private set; }

        // Verschachtelungstiefe ab Fakttabelle
        public int Depth { get; private set; }

        // Dimensionsknoten, der direkt an der Fakttabelle anliegt
        public DimensionALInterface RootDimension { get; private set; }

        public CoreInterface Core { get; private set; }

        public BTInterface BTInterface { get; private set; }

        public string ShortName { get; set; }
        public string Name { get; set; }        
        public string FullName {
            get {
                if(!string.IsNullOrEmpty(Model.Config.ALDatabase)) {
                    return $"{Model.Config.ALDatabase}.dbo.{Name}";
                } else {
                    return $"dbo.{Name}";
                }
            }
        }
        public string Alias { get; set; }

        public bool IsWithNowTable => false;

        public List<IALAttribute> Attributes { get; private set; }
    }
}