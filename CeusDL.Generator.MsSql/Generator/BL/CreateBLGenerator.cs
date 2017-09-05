using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;

namespace KDV.CeusDL.Generator.BL {
    public class CreateBLGenerator : IGenerator
    {
        private BLModel model;

        public CreateBLGenerator(CoreModel model) {
            this.model = new BLModel(model);
        }

        public List<GeneratorResult> GenerateCode() {
            string code = "--\n-- BaseLayer \n--\n";

            code += "-- Def-Interfaces: \n";
            foreach(var ifa in model.DefInterfaces.OrderBy(i => i.MaxReferenceDepth)) {
                code += "-- Depth: "+ifa.MaxReferenceDepth+"\n";                
                code += $"create table {ifa.FullName} ()\nGO\n\n";
            }

            code += "-- Dim-Interfaces: \n";
            foreach(var ifa in model.DimTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {                
                code += "-- Depth: "+ifa.MaxReferenceDepth+"\n";       
                code += $"create table {ifa.FullName} ()\nGO\n\n";
            }

            code += "-- Fakt-Interfaces: \n";
            foreach(var ifa in model.FactTableInterfaces.OrderBy(i => i.MaxReferenceDepth)) {                
                code += "-- Depth: "+ifa.MaxReferenceDepth+"\n";       
                code += $"create table {ifa.FullName} ()\nGO\n\n";
            }

            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Create.sql", code));
            return result;
        }
    }
}