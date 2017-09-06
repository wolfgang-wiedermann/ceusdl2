using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;

namespace KDV.CeusDL.Generator.BL {
    public class CreateBLGenerator : IGenerator
    {
        //private BLModel model;

        public CreateBLGenerator(CoreModel model) {
            //this.model = new BLModel(model);
        }

        public List<GeneratorResult> GenerateCode() {
            string code = "--\n-- BaseLayer \n--\n";

            // TODO ...

            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Create.sql", code));
            return result;
        }
    }
}