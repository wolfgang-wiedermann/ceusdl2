using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.IL;

namespace KDV.CeusDL.Generator.MySql.IL {
    public class DropILGenerator : IGenerator
    {
        private ILModel model;

        public DropILGenerator(CoreModel model) {
            this.model = new ILModel(model);
        }

        public List<GeneratorResult> GenerateCode()
        {
            string code = "--\n-- InterfaceLayer-Tabellen aus der MySQL-Datenbank entfernen\n--\n\n";
            if(!string.IsNullOrEmpty(model.Database)) {
                code += $"use {model.Database};\n\n";
            }

            foreach(var ifa in model.Interfaces.Where(i => i.IsILRelevant())) {
                code += $"DROP TABLE IF EXISTS {ifa.FullName};\n";
            }
            
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("IL_Drop.sql", code));
            return result;
        }
    }
}