using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;
using System.Text;

namespace KDV.CeusDL.Generator.BL {
    public class UpdateBLGenerator : IGenerator
    {
        private BLModel model;

        public UpdateBLGenerator(CoreModel model) {
            this.model = new BLModel(model);
        }

        public UpdateBLGenerator(BLModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Update.sql", GenerateUpdateTables()));            
            return result;
        }

        public string GenerateUpdateTables() {
            return "";
        }
    }
}