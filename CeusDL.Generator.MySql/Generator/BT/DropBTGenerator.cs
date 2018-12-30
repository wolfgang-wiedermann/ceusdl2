using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BT;
using System.Text;

namespace KDV.CeusDL.Generator.MySql.BT {
    public class DropBTGenerator : IGenerator
    {
        private BTModel model;

        public DropBTGenerator(CoreModel model) {
            this.model = new BTModel(model);
        }

        public DropBTGenerator(BTModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode()
        {            
            List<GeneratorResult> result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BT_Drop.sql", GenerateDropTables()));
            return result;
        }

        private string GenerateDropTables() {
            StringBuilder sb = new StringBuilder();
            sb.Append("--\n-- Tabellen für BaseLayer Transformation (BT) anlegen\n--\n\n");
            CreateUsing(sb);
            foreach(var ifa in model.Interfaces) {
                CreateDropTable(sb, ifa);
            }
            return sb.ToString();
        }

        private void CreateUsing(StringBuilder sb) {
            if(!string.IsNullOrEmpty(model.Config.BTDatabase)) {
                sb.Append($"use {model.Config.BTDatabase};\n\n");
            }
        }
        
        private void CreateDropTable(StringBuilder sb, BTInterface ifa)
        {
            sb.Append($"drop table if exists {ifa.FullName};\n");
        }
    }
}