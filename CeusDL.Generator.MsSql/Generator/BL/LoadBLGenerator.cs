using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;
using System.Text;

namespace KDV.CeusDL.Generator.BL {
    public class LoadBLGenerator : IGenerator
    {
        private BLModel model;

        public LoadBLGenerator(CoreModel model) {
            this.model = new BLModel(model);
        }

        public LoadBLGenerator(BLModel model) {
            this.model = model;
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("BL_Load.sql", GenerateLoadTables()));            
            return result;
        }

        private string GenerateLoadTables()
        {
            var sb = new StringBuilder();

            // Dimensionen
            foreach(var ifa in model.DimTableInterfaces) {
                sb.Append(GenerateDimTableUpdate(ifa));
                sb.Append(GenerateDimTableInsert(ifa));                
            }            

            // Fakten
            foreach(var ifa in model.FactTableInterfaces) {
                //sb.Append(GenerateFactTableDelete(ifa));
                //sb.Append(GenerateFactTableInsert(ifa));
            }

            return sb.ToString();
        }

        private string GenerateDimTableUpdate(IBLInterface ifa)
        {
            if(ifa.IsHistorized && ifa is DerivedBLInterface) {
                return GenerateDimTableUpdateHist(ifa);
            } else {
                return GenerateDimTableUpdateNoHist(ifa);
            }            
        }

        private string GenerateDimTableUpdateNoHist(IBLInterface ifa)
        {
            var idAttribute = ifa.Attributes.Where(a => a.IsIdentity).First();

            StringBuilder sb = new StringBuilder();
            sb.Append($"-- Update for non historized table: {ifa.FullName}\n");
            sb.Append("update t set\n");            
            foreach(var attr in ifa.UpdateCheckAttributes) {
                sb.Append($"t.{attr.Name} = v.{attr.Name},\n".Indent("    "));
            }
            sb.Append("t.T_Modifikation = 'U',\nt.T_Aend_Dat = GETDATE(),\nt.T_Benutzer = SYSTEM_USER\n".Indent("    "));
            sb.Append($"from {ifa.FullName} t\ninner join {ifa.FullViewName} v\n");
            sb.Append($"on t.{idAttribute.Name} = v.{idAttribute.Name}\n    and v.T_Modifikation = 'U'\n\n");
            return sb.ToString();
        }

        private string GenerateDimTableUpdateHist(IBLInterface ifa)
        {
            var idAttribute = ifa.Attributes.Where(a => a.IsIdentity).First();
            StringBuilder sb = new StringBuilder();
            sb.Append($"-- Update historized table: {ifa.FullName}\n");
            sb.Append("update t set t.T_Gueltig_Bis_Dat = dbo.GetCurrentTimeForHistory()\n");
            sb.Append($"from {ifa.FullName} t\n");
            sb.Append($"inner join {ifa.FullViewName} v\n");
            sb.Append($"on t.{idAttribute.Name} = v.{idAttribute.Name} and v.T_Modifikation = 'U'\n\n");
            return sb.ToString();
        }

        // 
        // Fügt alle mit I markierten Sätze in die entsprechenden Tabellen ein ...
        //
        private string GenerateDimTableInsert(IBLInterface ifa) {
            string fieldList = "";
            var relevantAttributes = ifa.Attributes.Where(a => (!a.IsTechnicalAttribute || a.Name.Equals("T_Modifikation")) && !a.IsIdentity);

            foreach(var attr in relevantAttributes) {
                fieldList += attr.Name;                
                fieldList += ", \n";                
            }

            var upperFieldList = fieldList + "T_Ladelauf_NR, \nT_Benutzer, \nT_System, \nT_Erst_Dat, \nT_Aend_Dat";
            var lowerFieldList = fieldList + "0, \nSYSTEM_USER, \n'H', \nGETDATE(), \nGETDATE()";

            string code = $"-- Insert {ifa.FullName}\n";
            code += $"insert into {ifa.FullName} (\n{upperFieldList.Indent("    ")}\n)\n";
            code += $"select \n{lowerFieldList.Indent("    ")} \nfrom {ifa.FullViewName}\n";
            code += "where T_Modifikation = 'I'\n";
            code += "\n";
            return code;
        }
    }
}