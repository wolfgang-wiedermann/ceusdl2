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

        static void Main(string[] args)
        {
            GenerationOptions options = new GenerationOptions();
            // Unterscheidung zwischen IDE und Commandline!
            if(System.Diagnostics.Debugger.IsAttached) {
                // Dieser Code wird bei F5 in Visual Studio ausgeführt
                string ceusdlFileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\split_main.ceusdl";
                //string ceusdlFileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\arc2018.ceusdl";
                string dbConnectionFileName = @"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\connection.txt";
                string rootFolder = "."; 
                PrepareEnvironment(rootFolder);

                options.DbConnectionString = File.ReadAllText(dbConnectionFileName);
                options.GenerateSnowflake = true;
                options.GenerateStar = true;
                ExecuteCompilation(ceusdlFileName, options);
                //ExecuteCompilation(ceusdlFileName, null);
            } else {
                // Dieser Code wird beim Aufruf über Commandline ausgeführt
                var cla = new CommandLineApplication();                
                cla.Name = "ceusdlc";
                var ceusdlOpt = cla.Option("-c | --ceusdl <ceusdlfile>", "Path to the ceusdl file to compile", CommandOptionType.SingleValue);
                var dirOpt = cla.Option("-d | --directory <target_directory>", "Path to store the result of compilation.", CommandOptionType.SingleValue);
                var conOpt = cla.Option("-con | --connection <connectionfile>", "Textfile containing connection string to Database", CommandOptionType.SingleValue);
                var starOpt = cla.Option("--star", "Generate analytical layer as star scheme", CommandOptionType.NoValue);
                var snowflakeOpt = cla.Option("--snowflake", "Generate analytical layer as snowflake scheme", CommandOptionType.NoValue);
                cla.HelpOption("-? | -h | --help");

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

                    options.GenerateStar = starOpt.HasValue();
                    options.GenerateSnowflake = snowflakeOpt.HasValue();

                    PrepareEnvironment(rootFolder);
                    var srcFile = ceusdlOpt.Value();

                    if(!File.Exists(srcFile)) {
                        Console.WriteLine("ERROR: you have to specify a !!EXISTING!! ceusdl file to start its compilation.");
                        return 2;
                    }

                    ExecuteCompilation(srcFile, options);

                    return 0;
                });

                cla.Execute(args);
            }
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

        static void ExecuteCompilation(string srcFile, GenerationOptions options) {
            var conStr = options.DbConnectionString;
            var data = new ParsableData(System.IO.File.ReadAllText(srcFile), srcFile);            
            var p = new FileParser(data);
            var result = p.Parse();
            var model = new CoreModel(result);

            // CeusDL generieren.
            ExecuteStep(new CeusDLGenerator(model), GENERATED_CEUSDL);            

            // IL generieren.
            ExecuteStep(new CreateILGenerator(model), GENERATED_SQL);            
            ExecuteStep(new DropILGenerator(model), GENERATED_SQL);
            ExecuteStep(new LoadILGenerator(model), GENERATED_CODE);
            ExecuteStep(new GraphvizILGenerator(model), GENERATED_GRAPHVIZ);
            ExecuteStep(new DummyDataILGenerator(model), GENERATED_CSV);
            ExecuteStep(new DummyILPythonGenerator(model), GENERATED_PYCODE);

            // BL generieren
            ExecuteStep(new CreateBLGenerator(model), GENERATED_SQL);
            ExecuteStep(new DropBLGenerator(model), GENERATED_SQL);            
            ExecuteStep(new GraphvizBLGenerator(model), GENERATED_GRAPHVIZ);
            ExecuteStep(new LoadBLGenerator(model), GENERATED_SQL);            
            if(conStr != null) {
                // Aktualisierung nur generieren, wenn eine Verbindung zur Datenbank angegeben wurde.
                ExecuteStep(new UpdateBLGenerator(model, conStr), GENERATED_SQL);
            }
            ExecuteStep(new CreateDefDataGenerator(model), GENERATED_PYCODE);

            // BT generieren
            ExecuteStep(new CreateBTGenerator(model), GENERATED_SQL);
            ExecuteStep(new DropBTGenerator(model), GENERATED_SQL);
            ExecuteStep(new LoadBTGenerator(model), GENERATED_SQL);
            ExecuteStep(new GraphvizBTGenerator(model), GENERATED_GRAPHVIZ);
            
            // AL generieren (Starschema)
            if(options.GenerateStar) {
                ExecuteStep(new CreateStarALGenerator(model), GENERATED_SQL);
                ExecuteStep(new DropStarALGenerator(model), GENERATED_SQL);
                ExecuteStep(new LoadStarALGenerator(model), GENERATED_SQL);
            }
            
            // AL generieren (Snowflake-Schema)
            if(options.GenerateSnowflake) {
                ExecuteStep(new CreateSnowflakeALGenerator(model), GENERATED_SQL);
                ExecuteStep(new DropSnowflakeALGenerator(model), GENERATED_SQL);
                ExecuteStep(new LoadSnowflakeALGenerator(model), GENERATED_SQL);
            }
        }

        static void ExecuteStep(IGenerator generator, string baseFolder) {
            var code = generator.GenerateCode();

            if(code == null) {
                return;
            }
            
            foreach(var file in code) {
                File.WriteAllText(Path.Combine(baseFolder, file.FileName), file.Content);
            }
        }
    }
}
