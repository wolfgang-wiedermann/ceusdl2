using System;
using System.Collections.Generic;

namespace KDV.CeusDL.Generator {
    public interface IGenerator {
        List<GeneratorResult> GenerateCode();
    }
}