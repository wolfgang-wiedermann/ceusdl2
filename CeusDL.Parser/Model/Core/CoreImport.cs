using System.Linq;
using System.Collections.Generic;
using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {
    public class CoreImport : CoreMainLevelObject
    {
        protected TmpImport BaseData {get;set;}
        
        public CoreImport(TmpImport tmp, CoreModel model) {
            BaseData = tmp;
            Objects = new List<CoreMainLevelObject>();                        
            foreach(var obj in tmp.Content.Objects) {
                if(obj.IsInterface) {
                    Objects.Add(new CoreInterface(obj.Interface, model));
                } else if(obj.IsComment) {
                    Objects.Add(new CoreComment(obj.Comment));
                } else if(obj.IsConfig) {
                    // Config aus Imports wird ignoriert.
                } else if(obj.IsImport) {
                    Objects.Add(new CoreImport(obj.Import, model));
                }
            }
        }

        // Alle über dieses File importierte Interfaces 
        // einschließlich der Elemente aus Kind-Imports
        public List<CoreInterface> Interfaces {
            get {
                var own = Objects.Where(i => i is CoreInterface)
                              .Select(i => (CoreInterface)i)
                              .ToList<CoreInterface>();

                var childImports = Objects.Where(i => i is CoreImport).Select(i => (CoreImport)i);

                var result = new List<CoreInterface>(own);                

                foreach(var childImport in childImports) {
                    result.AddRange(childImport.Interfaces);
                }

                return result;
            }
        }

        // Direkt über dieses File importierte Interfaces 
        // ohne Elemente aus Kind-Imports
        public List<CoreInterface> OwnInterfaces {
            get {
                return Objects.Where(i => i is CoreInterface)
                              .Select(i => (CoreInterface)i)
                              .ToList<CoreInterface>();
            }
        }

        public List<CoreMainLevelObject> Objects { get; private set; }

        public string WhitespaceBefore { get; set; }
    }
}