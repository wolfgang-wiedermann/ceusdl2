﻿using System;
using System.IO;
using KDV.CeusDL.Generator;
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

        static void Main(string[] args)
        {            
            // Unterscheidung zwischen IDE und Commandline!
            if(System.Diagnostics.Debugger.IsAttached) {
                // Dieser Code wird bei F5 in Visual Studio ausgeführt
                string rootFolder = "."; 
                PrepareEnvironment(rootFolder);
                //PartialParserTest.RunTests();
                //InterpreterTest.RunTests();
                //CeusDLGeneratorTest.RunTests();
                ExecuteCompilation(@"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\file_demo2.ceusdl");
            } else {
                // Dieser Code wird beim Aufruf über Commandline ausgeführt
                var cla = new CommandLineApplication();                
                cla.Name = "ceusdlc";
                var ceusdlOpt = cla.Option("-c | --ceusdl <ceusdlfile>", "Path to the ceusdl file to compile", CommandOptionType.SingleValue);
                var dirOpt = cla.Option("-d | --directory <target_directory>", "Path to store the result of compilation.", CommandOptionType.SingleValue);
                cla.HelpOption("-? | -h | --help");

                cla.OnExecute(() => {
                    string rootFolder = ".";
                    if(!ceusdlOpt.HasValue()) {
                        Console.WriteLine("ERROR: you have to specify a ceusdl file to start its compilation, use -c <filename>.ceusdl");
                        return 1;
                    }
                    if(dirOpt.HasValue()) {
                        rootFolder = dirOpt.Value();
                    }

                    PrepareEnvironment(rootFolder);
                    var srcFile = ceusdlOpt.Value();

                    if(!File.Exists(srcFile)) {
                        Console.WriteLine("ERROR: you have to specify a !!EXISTING!! ceusdl file to start its compilation.");
                        return 2;
                    }

                    ExecuteCompilation(srcFile);

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

            GENERATED_SQL = generatedSQL;
            GENERATED_CEUSDL = generatedCeusDL;
            GENERATED_CODE = generatedCode;
        }

        static void ExecuteCompilation(string srcFile) {
            var data = new ParsableData(System.IO.File.ReadAllText(srcFile));            
            var p = new FileParser(data);
            var result = p.Parse();
            var model = new CoreModel(result);

            // CeusDL generieren.
            ExecuteStep(new CeusDLGenerator(model), GENERATED_CEUSDL);            

            // IL generieren.
            ExecuteStep(new CreateILGenerator(model), GENERATED_SQL);            
            ExecuteStep(new DropILGenerator(model), GENERATED_SQL);
            ExecuteStep(new LoadILGenerator(model), GENERATED_CODE);            

            // TODO: IL-Parser in C# generieren ... und dann BL
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