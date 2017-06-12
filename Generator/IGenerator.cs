using System;
using System.Collections.Generic;

namespace KDV.CeusDL.Generator {
    interface IGenerator {
        List<GeneratorResult> GenerateCode();
    }
}