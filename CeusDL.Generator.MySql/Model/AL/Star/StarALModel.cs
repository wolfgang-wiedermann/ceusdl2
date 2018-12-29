

using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.MySql.BT;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.MySql.AL.Star {
    public class StarALModel : ALModel
    {
        public StarALModel(CoreModel parent) : base(new BTModel(parent)) {
            InitializeStarDimensionTables();
        }

        public StarALModel(BTModel parent) : base(parent) {
            InitializeStarDimensionTables();
        }

        public List<StarDimensionTable> StarDimensionTables { get; private set; }

        private void InitializeStarDimensionTables()
        {
            StarDimensionTables = new List<StarDimensionTable>();
            var lst = this.FactInterfaces
                          .SelectMany(f => f.Attributes
                                            .Where(a => a is RefALAttribute)
                                            .Select(a => ((RefALAttribute)a).ReferencedDimension))
                          .Select(d => new StarDimensionTable(d))
                          .ToList();
            
            foreach(var t in lst) {
                if(StarDimensionTables.SingleOrDefault(item => item.Name == t.Name) == null) {
                    StarDimensionTables.Add(t);
                }
            }
        }
    }
}