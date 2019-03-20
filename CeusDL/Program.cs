using System;
using System.IO;
using KDV.CeusDL.Generator;
using KDV.CeusDL.Generator.AL.Star;
using KDV.CeusDL.Generator.AL.Snowflake;
using KDV.CeusDL.Generator.BL;
using KDV.CeusDL.Generator.BT;
using KDV.CeusDL.Generator.CeusDL;
using KDV.CeusDL.Generator.IL;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Parser;
using Microsoft.Extensions.CommandLineUtils;
using KDV.CeusDL.Validator;
using CeusDL.Generator.MsSql.Generator;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CeusDL2
{
    ///
    /// Nach Exe kompilieren mit Aufruf 
    /// dotnet publish -c Release -r win7-x64 
    /// in der Commandline
    ///
    class Program
    {
        static string GENERATED_SQL;
        static string GENERATED_CEUSDL;
        static string GENERATED_CODE;
        static string GENERATED_PYCODE;
        static string GENERATED_GRAPHVIZ;
        static string GENERATED_CSV;

        static List<GeneratorResult> CoreILSQLStatements = new List<GeneratorResult>();
        static List<GeneratorResult> UpdateSQLStatements = new List<GeneratorResult>();     // BL
        static List<GeneratorResult> ReplaceSQLStatements = new List<GeneratorResult>();    // BL
        static List<GeneratorResult> CoreBTSQLStatements = new List<GeneratorResult>();
        static List<GeneratorResult> StarSQLStatements = new List<GeneratorResult>();       // AL
        static List<GeneratorResult> SnowflakeSQLStatements = new List<GeneratorResult>();  // AL

        public static bool IsUnix => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        static void Main(string[] args)
        {
            GenerationOptions options = new GenerationOptions();
            // Unterscheidung zwischen IDE und Commandline!
            if(System.Diagnostics.Debugger.IsAttached) {
                // Dieser Code wird bei F5 in Visual Studio ausgeführt
                //string ceusdlFileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\split_main.ceusdl";
                //string ceusdlFileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\sp_main.ceusdl";
                string ceusdlFileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\ext_main.ceusdl";
                string dbConnectionFileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\connection.txt";
                options.GenerateMySql = false;
                options.GenerateMsSql = true;
                options.ExecuteReplace = false;
                options.ExecuteUpdate = false;
                options.ExecuteUpdateWithReload = false;
                options.GenerateConstraints = false;

                if(IsUnix) {
                    ceusdlFileName = @"/Users/wiw39784/develop/dotnet/ceusdl2/Test/Data/ma_main.ceusdl";
                    dbConnectionFileName = @"/Users/wiw39784/develop/dotnet/ceusdl2/Test/Data/connection_mac.txt";
                    options.GenerateMySql = true;
                    options.GenerateMsSql = false;
                    options.ExecuteReplace = false;
                    options.ExecuteUpdate = true;
                    options.ExecuteUpdateWithReload = false;
                }

                string rootFolder = "."; 
                PrepareEnvironment(rootFolder);

                options.DbConnectionString = File.ReadAllText(dbConnectionFileName);
                //options.GenerateSnowflake = true;
                options.GenerateStar = true;
                ExecuteCompilation(ceusdlFileName, options);
                if (!string.IsNullOrWhiteSpace(options.DbConnectionString) && options.ExecuteUpdate) {
                    ExecuteUpdate(GENERATED_SQL, options);
                }
                else if (!string.IsNullOrWhiteSpace(options.DbConnectionString) && options.ExecuteReplace)
                {
                    ExecuteReplace(GENERATED_SQL, options);
                }
            } else {
                // Dieser Code wird beim Aufruf über Commandline ausgeführt
                var cla = new CommandLineApplication();                
                cla.Name = "ceusdlc";
                var ceusdlOpt = cla.Option("-c | --ceusdl <ceusdlfile>", "Path to the ceusdl file to compile", CommandOptionType.SingleValue);
                var dirOpt = cla.Option("-d | --directory <target_directory>", "Path to store the result of compilation.", CommandOptionType.SingleValue);
                var conOpt = cla.Option("--connection <connectionfile>", "Textfile containing connection string to Database", CommandOptionType.SingleValue);
                var mssqlOpt = cla.Option("--mssql", "Generate for Microsoft SQL Server", CommandOptionType.NoValue);
                var mysqlOpt = cla.Option("--mysql", "Generate for MySQL and MariaDB", CommandOptionType.NoValue);
                var starOpt = cla.Option("--star", "Generate analytical layer as star scheme", CommandOptionType.NoValue);
                var snowflakeOpt = cla.Option("--snowflake", "Generate analytical layer as snowflake scheme", CommandOptionType.NoValue);
                var executeUpdate = cla.Option("--update", "Update Baselayer, Replace everything else", CommandOptionType.NoValue);
                var executeUpdateWithReload = cla.Option("--update-with-reload", "Update BaseLayer like --update and reload data to bt and al", CommandOptionType.NoValue);
                var executeReplace = cla.Option("--replace", "Replace all Layers (deletes all Data)", CommandOptionType.NoValue);
                var generateConstraints = cla.Option("--generate-constraints", "Generate SQL-Constraints for BaseLayer", CommandOptionType.NoValue);
                var help = cla.HelpOption("-? | --help");

                cla.OnExecute(() => {
                    string rootFolder = ".";
                    string conStr = null;
                    if(!ceusdlOpt.HasValue()) {
                        Console.WriteLine("ERROR: you have to specify a ceusdl file to start its compilation, use -c <filename>.ceusdl");
                        Console.WriteLine("     : list help using --help");
                        return 1;
                    }
                    if(dirOpt.HasValue()) {
                        rootFolder = dirOpt.Value();
                    }
                    if(conOpt.HasValue()) {
                        if(File.Exists(conOpt.Value())) {
                            conStr = File.ReadAllText(conOpt.Value());
                        } else {
                            Console.WriteLine("ERROR: the specified connection file was not found");
                            return 3; // Datei nicht gefunden
                        }
                    }
                    options.DbConnectionString = conStr;

                    options.GenerateConstraints = generateConstraints.HasValue();
                    options.GenerateStar = starOpt.HasValue();
                    options.GenerateSnowflake = snowflakeOpt.HasValue();
                    options.GenerateMySql = mysqlOpt.HasValue() && (!mssqlOpt.HasValue());
                    options.GenerateMsSql = (!mysqlOpt.HasValue()) || mssqlOpt.HasValue();
                    options.ExecuteUpdate = executeUpdate.HasValue() || executeUpdateWithReload.HasValue();
                    options.ExecuteUpdateWithReload = executeUpdateWithReload.HasValue();
                    options.ExecuteReplace = executeReplace.HasValue();

                    int result = CheckOptions(options);
                    if(result != 0) return result;   

                    PrepareEnvironment(rootFolder);
                    var srcFile = ceusdlOpt.Value();

                    if(!File.Exists(srcFile)) {
                        Console.WriteLine("ERROR: you have to specify a !!EXISTING!! ceusdl file to start its compilation.");
                        return 2;
                    }

                    ExecuteCompilation(srcFile, options);

                    if (options.ExecuteUpdate)
                    {
                        ExecuteUpdate(GENERATED_SQL, options);
                    }
                    else if (options.ExecuteReplace)
                    {
                        ExecuteReplace(GENERATED_SQL, options);
                    }

                    return 0;
                });

                cla.Execute(args);
            }
        }

        private static int CheckOptions(GenerationOptions options)
        {
            if(options.ExecuteReplace && options.ExecuteUpdate) {
                Console.WriteLine("ERROR: ExecuteReplace and ExecuteUpdate can not be selected together");
                return 4; // Invalid combination of options
            }
            if((options.ExecuteReplace || options.ExecuteUpdate) && string.IsNullOrEmpty(options.DbConnectionString)) {
                Console.WriteLine("ERROR: An Execute (update or replace) command can just be run with a given database connection (--connection)");
                return 4; // Invalid combination of options
            }
            if(options.GenerateSnowflake && options.GenerateStar 
                && (options.ExecuteUpdate || options.ExecuteReplace)) {
                Console.WriteLine("ERROR: using an execute option is just possible with generate star or generate snowflake, not with both together");
                return 4; // Invalid combination of options
            }
            return 0;
        }

        static void PrepareEnvironment(string rootFolder) {
            var generated = Path.Combine(rootFolder, "Generated");
            var generatedSQL = Path.Combine(generated, "SQL");
            var generatedCeusDL = Path.Combine(generated, "CeusDL");            
            var generatedCode = Path.Combine(generated, "CSharp");
            var generatedPyCode = Path.Combine(generated, "Python");
            var generatedGraphviz = Path.Combine(generated, "Graphviz");
            var generatedCsv = Path.Combine(generated, "Csv");

            if(!Directory.Exists(generated)) {
                Directory.CreateDirectory(generated);   
            } else {
                Directory.Delete(generated, true);
                Directory.CreateDirectory(generated);  
            }

            if(!Directory.Exists(generatedSQL)) {
                Directory.CreateDirectory(generatedSQL);   
            }

            if(!Directory.Exists(generatedCeusDL)) {
                Directory.CreateDirectory(generatedCeusDL);   
            }

            if(!Directory.Exists(generatedCode)) {
                Directory.CreateDirectory(generatedCode);   
            }

            if(!Directory.Exists(generatedPyCode)) {
                Directory.CreateDirectory(generatedPyCode);   
            }

            if(!Directory.Exists(generatedGraphviz)) {
                Directory.CreateDirectory(generatedGraphviz);   
            }

            if(!Directory.Exists(generatedCsv)) {
                Directory.CreateDirectory(generatedCsv);   
            }

            GENERATED_SQL = generatedSQL;
            GENERATED_CEUSDL = generatedCeusDL;
            GENERATED_CODE = generatedCode;
            GENERATED_PYCODE = generatedPyCode;
            GENERATED_GRAPHVIZ = generatedGraphviz;
            GENERATED_CSV = generatedCsv;
        }

        static void ExecuteCompilation(string srcFile, GenerationOptions options)
        {
            string conStr = null;
            if (options != null)
            {
                conStr = options.DbConnectionString;
            }
            var data = new ParsableData(System.IO.File.ReadAllText(srcFile), srcFile);
            var p = new FileParser(data);
            var result = p.Parse();
            var model = new CoreModel(result);

            var validationResult = CoreModelValidator.Validate(model);
            validationResult.Print();
            if (validationResult.ContainsErrors())
            {
                Console.WriteLine("ERROR: Compilation stopped by validation errors");
                return;
            }

            // CeusDL generieren.
            ExecuteStep(new CeusDLGenerator(model), GENERATED_CEUSDL);

            // Code generieren
            if(options.GenerateMySql) {
                ExecuteGenerationMySql(options, conStr, model);
            } else {
                ExecuteGenerationMsSql(options, conStr, model);
            }
        }

        private static void ExecuteGenerationMySql(GenerationOptions options, string conStr, CoreModel model)
        {
            // IL generieren.
            CoreILSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.IL.DropILGenerator(model), GENERATED_SQL));
            CoreILSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.IL.CreateILGenerator(model), GENERATED_SQL));
            ExecuteStep(new KDV.CeusDL.Generator.MySql.IL.LoadILGenerator(model), GENERATED_CODE);
            ExecuteStep(new KDV.CeusDL.Generator.MySql.IL.GraphvizILGenerator(model), GENERATED_GRAPHVIZ);
            ExecuteStep(new DummyDataILGenerator(model), GENERATED_CSV);
            ExecuteStep(new DummyILPythonGenerator(model), GENERATED_PYCODE);

            // BL generieren
            ReplaceSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.BL.DropBLGenerator(model), GENERATED_SQL));
            ReplaceSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.BL.CreateBLGenerator(model, options.GenerateConstraints), GENERATED_SQL));
            ReplaceSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.BL.InitialDefaultValuesGenerator(model), GENERATED_SQL));
            ExecuteStep(new KDV.CeusDL.Generator.MySql.BL.GraphvizBLGenerator(model), GENERATED_GRAPHVIZ);
            ExecuteStep(new KDV.CeusDL.Generator.MySql.BL.LoadBLGenerator(model), GENERATED_SQL);
            if (!string.IsNullOrEmpty(conStr))
            {
                // Aktualisierung nur generieren, wenn eine Verbindung zur Datenbank angegeben wurde.
                UpdateSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.BL.UpdateBLGenerator(model, conStr, options.GenerateConstraints), GENERATED_SQL));
            }
            ExecuteStep(new KDV.CeusDL.Generator.MySql.BL.CreateDefDataGenerator(model), GENERATED_PYCODE);
 
            // BT generieren
            CoreBTSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.BT.DropBTGenerator(model), GENERATED_SQL));            
            CoreBTSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.BT.CreateBTGenerator(model), GENERATED_SQL));
            ExecuteStep(new KDV.CeusDL.Generator.MySql.BT.LoadBTGenerator(model), GENERATED_SQL);
            ExecuteStep(new KDV.CeusDL.Generator.MySql.BT.GraphvizBTGenerator(model), GENERATED_GRAPHVIZ);
             

            // AL generieren (Starschema)
            if (options.GenerateStar)
            {
                StarSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.AL.Star.DropStarALGenerator(model), GENERATED_SQL));
                StarSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.AL.Star.CreateStarALGenerator(model), GENERATED_SQL));
                ExecuteStep(new KDV.CeusDL.Generator.MySql.AL.Star.LoadStarALGenerator(model), GENERATED_SQL);
                ExecuteStep(new KDV.CeusDL.Generator.MySql.AL.Star.CopyStarALGenerator(model), GENERATED_SQL);
                ExecuteStep(new KDV.CeusDL.Generator.AL.Star.CreateMicrostrategyCmdStarGenerator(model), GENERATED_SQL);
            }
            /* 
            // AL generieren (Snowflake-Schema)
            if (options.GenerateSnowflake)
            {
                SnowflakeSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.AL.Snowflake.DropSnowflakeALGenerator(model), GENERATED_SQL));
                SnowflakeSQLStatements.AddRange(ExecuteStep(new KDV.CeusDL.Generator.MySql.AL.Snowflake.CreateSnowflakeALGenerator(model), GENERATED_SQL));
                ExecuteStep(new KDV.CeusDL.Generator.MySql.AL.Snowflake.LoadSnowflakeALGenerator(model), GENERATED_SQL);
                ExecuteStep(new KDV.CeusDL.Generator.AL.Snowflake.CreateMicrostrategyCmdSnowflakeGenerator(model), GENERATED_SQL);
            }
            */
        }

        private static void ExecuteGenerationMsSql(GenerationOptions options, string conStr, CoreModel model)
        {
            // IL generieren.
            CoreILSQLStatements.AddRange(ExecuteStep(new DropILGenerator(model), GENERATED_SQL));
            CoreILSQLStatements.AddRange(ExecuteStep(new CreateILGenerator(model), GENERATED_SQL));
            ExecuteStep(new LoadILGenerator(model), GENERATED_CODE);
            ExecuteStep(new GraphvizILGenerator(model), GENERATED_GRAPHVIZ);
            ExecuteStep(new DummyDataILGenerator(model), GENERATED_CSV);
            ExecuteStep(new DummyILPythonGenerator(model), GENERATED_PYCODE);

            // BL generieren
            ReplaceSQLStatements.AddRange(ExecuteStep(new DropBLGenerator(model), GENERATED_SQL));
            ReplaceSQLStatements.AddRange(ExecuteStep(new CreateBLGenerator(model, options.GenerateConstraints), GENERATED_SQL));
            ReplaceSQLStatements.AddRange(ExecuteStep(new InitialDefaultValuesGenerator(model), GENERATED_SQL));
            ExecuteStep(new GraphvizBLGenerator(model), GENERATED_GRAPHVIZ);
            ExecuteStep(new LoadBLGenerator(model), GENERATED_SQL);
            if (!string.IsNullOrEmpty(conStr))
            {
                // Aktualisierung nur generieren, wenn eine Verbindung zur Datenbank angegeben wurde.
                UpdateSQLStatements.AddRange(ExecuteStep(new UpdateBLGenerator(model, conStr, options.GenerateConstraints), GENERATED_SQL));
            }
            ExecuteStep(new CreateDefDataGenerator(model), GENERATED_PYCODE);

            // BT generieren
            CoreBTSQLStatements.AddRange(ExecuteStep(new DropBTGenerator(model), GENERATED_SQL));
            CoreBTSQLStatements.AddRange(ExecuteStep(new CreateBTGenerator(model), GENERATED_SQL));
            if(options.ExecuteUpdateWithReload) {
                CoreBTSQLStatements.AddRange(ExecuteStep(new LoadBTGenerator(model), GENERATED_SQL));
            } else {
                ExecuteStep(new LoadBTGenerator(model), GENERATED_SQL);
            }
            ExecuteStep(new GraphvizBTGenerator(model), GENERATED_GRAPHVIZ);

            // AL generieren (Starschema)
            if (options.GenerateStar)
            {
                StarSQLStatements.AddRange(ExecuteStep(new DropStarALGenerator(model), GENERATED_SQL));
                StarSQLStatements.AddRange(ExecuteStep(new CreateStarALGenerator(model), GENERATED_SQL));
                if(options.ExecuteUpdateWithReload) {
                    StarSQLStatements.AddRange(ExecuteStep(new LoadStarALGenerator(model), GENERATED_SQL));
                } else {
                    ExecuteStep(new LoadStarALGenerator(model), GENERATED_SQL);
                }
                ExecuteStep(new CopyStarALGenerator(model), GENERATED_SQL);
                ExecuteStep(new KDV.CeusDL.Generator.AL.Star.CreateMicrostrategyCmdStarGenerator(model), GENERATED_SQL);
            }

            // AL generieren (Snowflake-Schema)
            if (options.GenerateSnowflake)
            {
                SnowflakeSQLStatements.AddRange(ExecuteStep(new DropSnowflakeALGenerator(model), GENERATED_SQL));
                SnowflakeSQLStatements.AddRange(ExecuteStep(new CreateSnowflakeALGenerator(model), GENERATED_SQL));
                if(options.ExecuteUpdateWithReload) {
                    SnowflakeSQLStatements.AddRange(ExecuteStep(new LoadSnowflakeALGenerator(model), GENERATED_SQL));
                } else {
                    ExecuteStep(new LoadSnowflakeALGenerator(model), GENERATED_SQL);
                }
                ExecuteStep(new KDV.CeusDL.Generator.AL.Snowflake.CreateMicrostrategyCmdSnowflakeGenerator(model), GENERATED_SQL);
            }
        }

        private static void ExecuteReplace(string sql, GenerationOptions options)
        {
            if (string.IsNullOrEmpty(options.DbConnectionString))
            {
                throw new InvalidOperationException("ExecuteUpdate ist nur mit Datenbankverbindung zulässig");
            }

            if(options.GenerateMsSql) 
            {
                using (IExecutor exec = new SqlServerExecutor(options.DbConnectionString, GENERATED_SQL))
                {
                    ExecuteReplaceInternal(options, exec);
                }
            }
            else if(options.GenerateMySql)
            {
                using (IExecutor exec = new MySqlExecutor(options.DbConnectionString, GENERATED_SQL))
                {
                    ExecuteReplaceInternal(options, exec);
                }
            }
        }

        private static void ExecuteReplaceInternal(GenerationOptions options, IExecutor exec)
        {
            foreach (var stm in CoreILSQLStatements)
            {
                exec.ExecuteSQL(stm.FileName);
            }
            foreach (var stm in ReplaceSQLStatements)
            {
                exec.ExecuteSQL(stm.FileName);
            }
            foreach (var stm in CoreBTSQLStatements)
            {
                exec.ExecuteSQL(stm.FileName);
            }
            if (options.GenerateSnowflake)
            {
                foreach (var stm in SnowflakeSQLStatements)
                {
                    exec.ExecuteSQL(stm.FileName);
                }
            }
            else if (options.GenerateStar)
            {
                foreach (var stm in StarSQLStatements)
                {
                    exec.ExecuteSQL(stm.FileName);
                }
            }
        }

        private static void ExecuteUpdate(string sql, GenerationOptions options)
        {
            if (string.IsNullOrEmpty(options.DbConnectionString))
            {
                throw new InvalidOperationException("ExecuteUpdate ist nur mit Datenbankverbindung zulässig");
            }

            if(options.GenerateMsSql) 
            {
                using (IExecutor exec = new SqlServerExecutor(options.DbConnectionString, GENERATED_SQL))
                {
                    ExecuteUpdateInternal(options, exec);
                }
            }
            else if(options.GenerateMySql) 
            {
                using (IExecutor exec = new MySqlExecutor(options.DbConnectionString, GENERATED_SQL))
                {
                    ExecuteUpdateInternal(options, exec);
                }
            }
        }

        private static void ExecuteUpdateInternal(GenerationOptions options, IExecutor exec)
        {
            foreach (var stm in CoreILSQLStatements)
            {
                exec.ExecuteSQL(stm.FileName);
            }
            exec.ExecuteSQL("BL_Drop_FKs.sql");
            foreach (var stm in UpdateSQLStatements)
            {
                exec.ExecuteSQL(stm.FileName);
            }
            if(options.GenerateConstraints) {
                exec.ExecuteSQL("BL_Create_FKs.sql");
            }
            foreach (var stm in CoreBTSQLStatements)
            {
                exec.ExecuteSQL(stm.FileName);
            }
            if (options.GenerateSnowflake)
            {
                foreach (var stm in SnowflakeSQLStatements)
                {
                    exec.ExecuteSQL(stm.FileName);
                }
            }
            else if (options.GenerateStar)
            {
                foreach (var stm in StarSQLStatements)
                {
                    exec.ExecuteSQL(stm.FileName);
                }
            }
        }

        static List<GeneratorResult> ExecuteStep(IGenerator generator, string baseFolder) {
            var code = generator.GenerateCode();

            if(code == null) {
                return new List<GeneratorResult>();
            }
            
            foreach(var file in code) {
                File.WriteAllText(Path.Combine(baseFolder, file.FileName), file.Content);
            }

            return code;
        }
    }
}
