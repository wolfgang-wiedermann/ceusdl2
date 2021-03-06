using System;
using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.IL;

namespace KDV.CeusDL.Generator.MySql.IL {
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
            result.Add(new GeneratorResult("InterfaceLayerLoaderCollection.cs", GenerateLoaderCollection()));
            return result;
        }

        private string GenerateLoaderCollection()
        {
            string code = "using System;\n";
            code += "using System.Collections.Generic;\n";

            code += $"using {model.Namespace};\n\n";

            code += "namespace Kdv.Loader {\n";
            code += "public class InterfaceLayerLoaderCollection {\n".Indent(1);
            code += "    public static List<IInterfaceLayerLoader> GetLoaders(string directory) {\n".Indent(1);
            code += "        var lst = new List<IInterfaceLayerLoader>();\n".Indent(1);
            foreach(var ifa in model.Interfaces.Where(i => i.IsILRelevant())) {
                code += $"lst.Add(new {ifa.ShortName}Loader(directory));\n".Indent(3);
            }
            code += "        return lst;\n".Indent(1);
            code += "    }\n".Indent(1);
            code += "}\n".Indent(1);
            code += "}";

            return code;
        }

        private string GenerateInterface()
        {
            string code = "using System.Data.Common;\n\n";
            
            code += "namespace Kdv.Loader {\n";
            code += "    public interface IInterfaceLayerLoader {\n";
            code += "        void Execute(DbConnection con);\n";
            code += "    }\n";
            code += "}\n";

            return code;
        }

        private IEnumerable<GeneratorResult> GenerateLoadCsvClasses()
        {
            // TODO: Die Loader brauchen wir eigentlich blos für DimTable und FactTable !!!
            foreach(var ifa in model.Interfaces.Where(i => i.IsILRelevant())) {
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
            foreach(var attr in ifa.NonCalculatedAttributes) {
                code += $"        public string {attr.Name} {{ get; set; }}\n";
            }
            code += "    }\n\n";
            return code;
        }

        private string GenerateParserClass(ILInterface ifa)
        {
            string code = $"    public class {ifa.ShortName}Loader : IInterfaceLayerLoader {{\n";
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
            foreach(var attr in ifa.NonCalculatedAttributes) {
                code += attr.Name;
                if(!attr.Equals(ifa.NonCalculatedAttributes.Last())) {
                    code +=", ";
                }                
            }
            code += "\";\n";

            code += "            sql += \") values (\";\n";
            
            foreach(var attr in ifa.NonCalculatedAttributes) {
                code +=$"            if(!string.IsNullOrEmpty(line.{attr.Name})) {{\n";
                code +=$"                sql += \"\'\" + (line.{attr.Name}.Length>{GetMaxLength(attr)}?line.{attr.Name}{GetSubstringIfNeeded(attr, ifa)}:line.{attr.Name}).Replace(\"'\", \"''\") + \"\'";                
                if(attr.Equals(ifa.NonCalculatedAttributes.Last())) {
                    code += ")\\n\";\n"; 
                } else {
                    code += ", \";\n"; 
                }
                code += "            } else {\n";
                code += "                sql += \"\'\'";
                if(attr.Equals(ifa.NonCalculatedAttributes.Last())) {
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
            return $".Substring(0, {GetMaxLength(attr)})";                        
        }

        private int GetMaxLength(ILAttribute attr) {
            switch(attr.CDataType) {
                case CoreDataType.DATE:
                    return 10;
                case CoreDataType.DATETIME:
                    return 20;
                case CoreDataType.DECIMAL:                    
                    return attr.Length;
                case CoreDataType.VARCHAR:
                    return attr.Length;
                case CoreDataType.INT:
                    return 11; // 10 + Minus
            }
            return 0;
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
            code += "                string line = \"\";\n";
            code += "                long i = 0;\n\n";
            code += "                while((line = fs.ReadLine()) != null) {\n";
            code += "                    i += 1;\n";
            code += "                    yield return ParseLine(line, i);\n";
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
            code +=$"        public {ifa.ShortName}Line ParseLine(string line, long lineNumber) {{\n";
            // 
            // Zustandsvariablen des Automaten
            //
            code += "            // Zustands- und Hilfsvariablen\n";
            code +=$"            var state = {ifa.ShortName}ParserState.IN_{ifa.NonCalculatedAttributes[0].Name.ToUpper()};\n";
            code +=$"            var substate = {ifa.ShortName}ParserSubstate.INITIAL;\n";
            code +=$"            var content = new {ifa.ShortName}Line();\n";
            code += "            char c = ' ';\n";
            code += "            string buf = \"\";\n\n";
            //
            // Schleife zur Verarbeitung der Daten
            //
            code += "            // Verarbeitung\n";
            code += "            for(int i = 0; i < line.Length; i++) {\n";
            code += "                c = line[i];\n";
            code += "                switch(state) {\n";

            foreach(var attr in ifa.NonCalculatedAttributes) {
                // Generierung der Cases
                code += GenerateParseAttributeCase(ifa, attr);                
            }

            code +=$"                    case {ifa.ShortName}ParserState.FINAL:\n";
            code +=$"                        return content;\n";

            code += "                }\n";
            code += "            }\n";

            code +=$"            if(state == {ifa.ShortName}ParserState.FINAL) {{\n";
            code += "                return content;\n";
            code += "            } else {\n";            
            code += "                throw new InvalidOperationException($\"Ungültiger State am Ende der Zeile {lineNumber} => {state}\");\n";
            code += "            }\n";
            code += "        }\n\n";
            return code;
        }

        ///
        /// Generierung eines einzelnen Cases (zu ParserState)
        ///
        private string GenerateParseAttributeCase(ILInterface ifa, ILAttribute attr)
        {
            string code = "";            
            code +=$"case {ifa.ShortName}ParserState.IN_{attr.Name.ToUpper()}:\n";
            /// Vor Anführungszeichen
            code +=$"    if(substate == {ifa.ShortName}ParserSubstate.INITIAL) {{\n";
            code += "        if(c == '\"') {\n";
            code +=$"            substate = {ifa.ShortName}ParserSubstate.IN_STRING;\n";
            code += "        } else if(c == ';') {\n";
            // Zum nächsten Attribut weiterschalten
            if(attr == ifa.NonCalculatedAttributes.Last()) {
                code +=$"            state = {ifa.ShortName}ParserState.FINAL;\n";
            } else {
                var nextAttr = ifa.NonCalculatedAttributes[ifa.NonCalculatedAttributes.IndexOf(attr)+1];
                code +=$"            state = {ifa.ShortName}ParserState.IN_{nextAttr.Name.ToUpper()};\n";
            }
            code += "        } else {\n";            
            code +=$"            throw new InvalidDataException($\"Ungültiges Zeichen vor {ifa.Name}.{attr.Name} Zeile {{i}}\");\n";
            code += "        }\n";
            /// Nach Anführungszeichen
            code +=$"    }} else if(substate == {ifa.ShortName}ParserSubstate.IN_STRING) {{\n";
            code += "        if(c == '\"') {\n";
            code +=$"            content.{attr.Name} = buf;\n";
            code += "            buf = \"\";\n";
            code +=$"            substate = {ifa.ShortName}ParserSubstate.INITIAL;\n";
            if(attr == ifa.NonCalculatedAttributes.Last()) {
                code +=$"            state = {ifa.ShortName}ParserState.FINAL;\n";
            }            
            code += "        } else {\n";
            code += "            buf += c;\n";
            code += "        }\n";
            code += "    }\n";
            code +=$"    break;\n";            
            return code.Indent("                    ");
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
            code += "                        i += 1;\n";
            code += "                    }\n";
            code += "                }\n";
            code += "            }\n";
            code += "        }\n\n";
            return code;
        }

        private string GenerateCsvEnum(ILInterface ifa)
        {
            string code = $"    public enum {ifa.ShortName}ParserState {{\n        ";

            foreach(var attr in ifa.NonCalculatedAttributes) {
                code += $"IN_{attr.Name.ToUpper()}";                
                code += ", ";                
            }            

            code += "FINAL\n    }\n\n";
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