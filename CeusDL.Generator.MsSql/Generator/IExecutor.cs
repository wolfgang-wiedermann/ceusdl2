using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CeusDL.Generator.MsSql.Generator
{
    public interface IExecutor : IDisposable
    {
        void ExecuteSQL(string sqlFileName);
    }
}
