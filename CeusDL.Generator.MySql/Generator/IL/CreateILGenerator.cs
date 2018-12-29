using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.IL;

namespace KDV.CeusDL.Generator.MySql.IL {
    public class CreateILGenerator : IGenerator
    {
        private ILModel model;

        public CreateILGenerator(CoreModel model) {
            this.model = new ILModel(model);
        }

        public List<GeneratorResult> GenerateCode()
        {
            string code = "--\n-- InterfaceLayer (MySQL) \n--\n\n";
            code += $"create database if not exists {model.Database};\n\n";
            code += $"use {model.Database};\n\n";

            foreach(var ifa in model.Interfaces.Where(i => i.IsILRelevant())) {
                code += GenerateIfaCode(ifa);
            }
            
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("IL_Create.sql", code));
            return result;
        }

        private string GenerateIfaCode(ILInterface ifa)
        {
            string code = $"create table {ifa.Name} (\n";
            foreach(var attr in ifa.Attributes) {
                code += $"    {attr.Name} {attr.DataType}{attr.DataTypeParameters}";
                if(!ifa.Attributes.Last().Equals(attr)) {
                    code += ",";
                }
                code += "\n";
            }
            code += ");\n\n";
            return code;
        }
    }
}