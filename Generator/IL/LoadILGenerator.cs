using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;

namespace KDV.CeusDL.Generator.IL {
    public class LoadILGenerator : IGenerator
    {
        private ILModel model;

        public LoadILGenerator(CoreModel model) {
            this.model = new ILModel(model);
        }

        public List<GeneratorResult> GenerateCode() {
            var result = new List<GeneratorResult>();
            result.Add(new GeneratorResult("IInterfaceLayerLoader.cs", GenerateInterface()));
            result.AddRange(GenerateLoadCsvClasses());
            return result;
        }

        private string GenerateInterface()
        {
            string code = "using System.Data.Common;\n\n";
            
            code += "namespace Kdv.Loader {\n";
            code += "    public interface IInterfacelayerLoader {\n";
            code += "        void Execute(DbConnection con);\n";
            code += "    }\n";
            code += "}\n";

            return code;
        }

        private IEnumerable<GeneratorResult> GenerateLoadCsvClasses()
        {
            foreach(var ifa in model.Interfaces) {
                yield return GenerateLoadCsvClasses(ifa);
            }
        }

        private GeneratorResult GenerateLoadCsvClasses(ILInterface ifa)
        {
            string code= "using System;\n";
            code += "using System.Collections.Generic;\n";
            code += "using System.IO;\n";
            code += "using System.Data.Common;\n";
            if(!model.Namespace.Equals("Kdv.Loader")) {
                code += "using Kdv.Loader;\n";
            }
            code += "\n";
            code += $"namespace {model.Namespace} {{\n";

            code += GenerateCsvEnum(ifa);
            code += GenerateSubstateEnum(ifa);
            code += GenerateDataClass(ifa);
            code += GenerateParserClass(ifa);

            code += "}";
            return new GeneratorResult($"{ifa.ShortName}Loader.cs", code);
        }

        private string GenerateDataClass(ILInterface ifa)
        {            
            string code = $"    public class {ifa.ShortName}Line {{\n";
            foreach(var attr in ifa.Attributes) {
                code += $"        public string {attr.Name} {{ get; set; }}\n";
            }
            code += "    }\n\n";
            return code;
        }

        private string GenerateParserClass(ILInterface ifa)
        {
            string code = $"    public class {ifa.ShortName}Loader : IInterfacelayerLoader {{\n";
            // 
            // Properties
            //
            code += "        public string Folder { get; set; }\n";
            code += "        public string FileName { get; set; }\n\n";
            //
            // Konstruktor
            //
            code += $"        public {ifa.ShortName}Loader(string folder) {{\n";
            code += $"            this.FileName = \"{ifa.ShortName}.csv\";\n";
            code += "            this.Folder = folder;\n";
            code += "        }\n\n";
            //
            // Methoden
            //
            code += GenerateExecuteMethod(ifa);
            code += GenerateLoadMethod(ifa);
            code += GenerateParseLineMethod(ifa);
            code += GenerateDeleteMethod(ifa);
            code += GenerateInsertSQLMethod(ifa);
            // 
            // Schluss
            //
            code += "    }\n\n";
            return code;
        }

        ///
        /// Line to SQL Funktion
        ///
        private string GenerateInsertSQLMethod(ILInterface ifa)
        {
            string code = "";
            code +=$"        public string GetInsertSQL({ifa.ShortName}Line line) {{\n";
            code +=$"            string sql = \"insert into {ifa.FullName} (\";\n";

            code += "            sql += \"";
            foreach(var attr in ifa.Attributes) {
                code += attr.Name;
                if(!attr.Equals(ifa.Attributes.Last())) {
                    code +=", ";
                }                
            }
            code += "\";\n";

            code += "            sql += \") values (\";\n";
            
            foreach(var attr in ifa.Attributes) {
                code +=$"            if(!string.IsNullOrEmpty(line.{attr.Name})) {{\n";
                code +=$"                sql += \"\'\" + line.{attr.Name}{GetSubstringIfNeeded(attr, ifa)} + \"\'";                
                if(attr.Equals(ifa.Attributes.Last())) {
                    code += ")\\n\";\n"; 
                } else {
                    code += ", \";\n"; 
                }
                code += "            } else {\n";
                code += "                sql += \"\'\'";
                if(attr.Equals(ifa.Attributes.Last())) {
                    code += ")\\n\";\n"; 
                } else {
                    code += ", \";\n"; 
                }
                code += "            }\n";
            }

            code += "            return sql;\n";
            code += "        }\n\n";
            return code;
        }

        private string GetSubstringIfNeeded(ILAttribute attr, ILInterface ifa) {
            string code = "";
            switch(attr.CDataType) {
                case CoreDataType.DATE:
                    return ".Substring(0, 10)";
                case CoreDataType.DATETIME:
                    return ".Substring(0, 20)";
                case CoreDataType.DECIMAL:                    
                    return $".Substring(0, {attr.Length})";
                case CoreDataType.VARCHAR:
                    return $".Substring(0, {attr.Length})";
                case CoreDataType.INT:
                    return ".Substring(0, 10)";
            }
            return code;
        }

        ///
        /// Delete-Funktion
        /// 
        private string GenerateDeleteMethod(ILInterface ifa)
        {   
            string code = "";
            code += $"        public void DeleteFromTable(DbConnection con) {{\n";
            code += "            using(var cmd = con.CreateCommand()) {\n";
            code +=$"                cmd.CommandText = \"truncate table {ifa.FullName}\";\n";
            code += "                cmd.ExecuteNonQuery();\n";
            code += "            }\n";
            code += "        }\n\n";
            return code;
        }

        ///
        /// Load-Funktion
        /// 
        private string GenerateLoadMethod(ILInterface ifa)
        {
            string code = "";
            code +=$"        public IEnumerable<{ifa.ShortName}Line> Load(string filename) {{\n";
            code += "            if(!File.Exists(filename)) {\n";
            code += "                throw new FileNotFoundException(filename);\n";
            code += "            }\n\n";
            code += "            using(var fs = new StreamReader(new FileStream(filename, FileMode.Open))) {\n";
            code += "                string line = \"\";\n\n";
            code += "                while((line = fs.ReadLine()) != null) {\n";
            code += "                    yield return ParseLine(line);\n";
            code += "                }\n";
            code += "            }\n";
            code += "        }\n\n";
            return code;
        }

        ///
        /// Load-Funktion
        /// 
        private string GenerateParseLineMethod(ILInterface ifa)
        {
            string code = "";
            code +=$"        public {ifa.ShortName}Line ParseLine(string line) {{\n";
            code += "            throw new NotImplementedException();\n";
            code += "        }\n\n";
            return code;
        }

        ///
        /// Execute-Funktion
        ///
        private string GenerateExecuteMethod(ILInterface ifa)
        {
            string code = "";     
            code += "        public void Execute(DbConnection con) {\n";
            code += "            string filename = Path.Combine(this.Folder, this.FileName);\n";
            code += "            if(con == null) {\n";
            code += "                // Ausgabe zwecks Debugging\n";
            code += "                foreach(var line in Load(filename)) {\n";
            code += "                    Console.WriteLine(GetInsertSQL(line));\n";
            code += "                }\n";
            code += "            } else {\n";
            code += "                // Tabelleninhalt löschen\n";
            code += "                DeleteFromTable(con);\n";
            code += "                int i = 0;\n";
            // TODO: Kopfzeile sollte später auf Korrektheit geprüft werden!
            code += "                // Tatsächliche Verarbeitung: 1. Zeile wird als Kopfzeile ausgelassen\n";
            code += "                using(var cmd = con.CreateCommand()) {\n";
            code += "                    foreach(var line in Load(filename)) {\n";
            code += "                        cmd.CommandText = GetInsertSQL(line);\n";
            code += "                        if(i > 0) {\n";
            code += "                            cmd.ExecuteNonQuery();\n";
            code += "                        }\n";
            code += "                    }\n";
            code += "                }\n";
            code += "            }\n";
            code += "        }\n\n";
            return code;
        }

        private string GenerateCsvEnum(ILInterface ifa)
        {
            string code = $"    public enum {ifa.ShortName}ParserState {{\n        ";

            foreach(var attr in ifa.Attributes) {
                code += $"IN_{attr.Name.ToUpper()}";
                if(ifa.Attributes.Last() != attr) {
                    code += ", ";
                } else {
                    code += "\n";
                }
            }

            code += "    }\n\n";
            return code;
        }

        private string GenerateSubstateEnum(ILInterface ifa)
        {
            string code = $"    public enum {ifa.ShortName}ParserSubstate {{\n";
            code += "        INITIAL, IN_STRING\n";
            code += "    }\n\n";
            return code;
        }
    }
}