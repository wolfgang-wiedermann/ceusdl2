using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BT;

namespace KDV.CeusDL.Model.MySql.AL
{
    public class FactALInterface : IALInterface
    {           
        private int joinIdx;
        public FactALInterface(BTInterface ifa, ALModel alModel)
        {
            this.joinIdx = 0;
            this.BTInterface = ifa;
            this.Model = alModel;
            this.HistoryAttribute = null;
            this.FactInterfaceReferences = new List<FactInterfaceReference>();

            PrepareAttributes();
        }

        public ALModel Model { get; private set; }

        public CoreInterface Core => BTInterface.coreInterface;

        public BT.BTInterface BTInterface { get; private set; }

        public IALAttribute IdColumn { get; private set; }

        public string ShortName => BTInterface.ShortName;

        public string Name {
            get {
                string prefix = "";
                if(!string.IsNullOrEmpty(Model.Config.Prefix)) {
                    prefix = $"{Model.Config.Prefix}_";
                }
                return $"{prefix}F_{ShortName}";
            }
        }

        public string FullName {
            get {
                if(!string.IsNullOrEmpty(Model.Config.ALDatabase)) {
                    return $"{Model.Config.ALDatabase}.dbo.{Name}";
                } else {
                    return $"dbo.{Name}";
                }
            }
        }

        public List<IALAttribute> Attributes { get; private set; }

        public IALAttribute HistoryAttribute { get; private set; }

        public List<FactInterfaceReference> FactInterfaceReferences { get; private set; }

        public bool IsWithNowTable => this.Core != null?this.Core.IsWithNowTable:false;

        private void PrepareAttributes()
        {
            Attributes = new List<IALAttribute>();
            foreach(var attr in BTInterface.Attributes.OrderBy(a => a.GetBLAttribute().SortId)) {
                if(attr is BT.BaseBTAttribute) {
                    PrepareBaseAttribute(attr);
                } else if(attr is BT.RefBTAttribute) {
                    PrepareRefAttribute((BT.RefBTAttribute)attr);
                } else {
                    throw new NotImplementedException($"FactALInterface.PrepareAttributes: Attributtyp {attr.GetType().Name} nicht unterstützt");
                }
            }
        }

        private void PrepareBaseAttribute(IBTAttribute attr)
        {
            var baseAttr = new BaseALAttribute(this, (BT.BaseBTAttribute)attr);
            baseAttr.JoinAlias = "t0";
            Attributes.Add(baseAttr);
            if (attr.IsIdentity)
            {
                IdColumn = baseAttr;
            }
        }

        private void PrepareRefAttribute(BT.RefBTAttribute refAttr)
        {            
            if (refAttr.ReferencedBTInterface.InterfaceType == CoreInterfaceType.FACT_TABLE)
            {                
                var child = new FactALInterface(refAttr.ReferencedBTInterface, Model);
                child = Model.GetFactInterfaceFor(child);
                joinIdx += 1;
                var joinAlias = $"t{joinIdx}";

                this.FactInterfaceReferences.Add(new FactInterfaceReference() {
                    BTInterface = refAttr.ReferencedBTInterface,
                    JoinAlias = joinAlias,
                    RefColumnName = refAttr.IdAttribute.Name
                });

                // Alle Attribute der Kind-Fakttabelle übernehmen
                // (ohne Fakten, Historisierungsattribut und Mandant-Spalte)
                foreach(var attr in child.Attributes.Where(a => !a.IsFact && a.Name != "Mandant_ID" && a != child.HistoryAttribute)) {
                    var clone = attr.Clone(this);
                    clone.JoinAlias = joinAlias;
                    Attributes.Add(clone); 
                }
            }
            else
            {
                var dim = new DimensionALInterface(Model, refAttr);
                dim = Model.GetDimensionInterfaceFor(dim);
                var refAlAttr = new RefALAttribute(this, dim, refAttr);
                refAlAttr.JoinAlias = "t0";
                Attributes.Add(refAlAttr);

                // Wenn es sich hier um das Historienattribut handelt, merken
                if(refAttr?.ParentInterface?.blInterface?.HistoryAttribute != null 
                    && refAttr?.ParentInterface?.blInterface?.HistoryAttribute == refAttr.blAttribute) {
                        HistoryAttribute = refAlAttr;
                }
            }
        }
    }
}