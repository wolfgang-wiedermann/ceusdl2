using System;
using System.Linq;
using KDV.CeusDL.Generator.CeusDL;
using KDV.CeusDL.Model;
using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.IL;
using KDV.CeusDL.Parser;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Test {
    public class CeusDLGeneratorTest {
        public static void RunTests() {
            var test = new CeusDLGeneratorTest();

            var data = new ParsableData(System.IO.File.ReadAllText(@"C:\Users\wiw39784\Documents\git\CeusDL2\Test\Data\file_demo2.ceusdl"));
            //var data = new ParsableData(System.IO.File.ReadAllText(@"C:\Users\wiw39784\Documents\git\CeusDL2\sample.ceusdl"));
            var p = new FileParser(data);
            var result = p.Parse();
            var model = new CoreModel(result);

            test.GenerateConfig(model);
            test.TestILModel(model);
        }

        public void GenerateConfig(CoreModel model) {
            CeusDLGenerator gen = new CeusDLGenerator(model);
            gen.GenerateCode();
        }

        public void TestILModel(CoreModel input) {
            ILModel model = new ILModel(input);
            Console.WriteLine(model.Database);
        }
    }
}