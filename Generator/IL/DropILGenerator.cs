using System;
using System.Linq;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;

namespace KDV.CeusDL.Generator.IL {
    public class DropILGenerator : IGenerator
    {
        private ILModel model;

        public DropILGenerator(CoreModel model) {
            this.model = new ILModel(model);
        }

        public string GenerateCode()
        {
            string code = "--\n-- InterfaceLayer-Tabellen aus der Datenbank entfernen\n--\n\n";
            if(!string.IsNullOrEmpty(model.Database)) {
                code += $"use [{model.Database}]\nGO\n\n";
            }

            foreach(var ifa in model.Interfaces) {
                code += $"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{ifa.Name}]') AND type in (N'U'))\n";
                code += $"DROP TABLE [dbo].[{ifa.Name}]\n";
                code += "GO\n\n";
            }

            return code;
        }
    }
}