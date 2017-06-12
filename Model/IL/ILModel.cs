using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Model.Core;

namespace KDV.CeusDL.Model.IL {
    public class ILModel {        
        public string Database {get; private set;}
        public List<ILInterface> Interfaces {get; private set;}

        public CoreModel coreModel;

        public ILModel(CoreModel model) {
            coreModel = model;
            Database = model.Config.ILDatabase;            
            Interfaces = new List<ILInterface>();

            foreach(var ifa in model.Interfaces) {
                if(ifa.Type != CoreInterfaceType.DEF_TABLE && ifa.Type != CoreInterfaceType.DIM_VIEW) {
                    Interfaces.Add(new ILInterface(ifa, model));
                }
            }
        }
    }
}