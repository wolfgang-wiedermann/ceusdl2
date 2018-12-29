using System;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.MySql.BT
{
    public interface ISubAttribute
    {
        string ShortName { get; }
        string Alias { get; }
        string Name { get; }        
        CoreDataType DataType { get; }
        string SqlDataType { get; }
    }
}
