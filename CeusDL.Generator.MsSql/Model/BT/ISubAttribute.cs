using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.BT
{
    public interface ISubAttribute
    {
        string ShortName { get; }
        string Alias { get; }
        string Name { get; }
        string FullName { get; }
        CoreDataType DataType { get; }
        string SqlDataType { get; }
    }
}
