using KDV.CeusDL.Parser.TmpModel;

namespace KDV.CeusDL.Model.Core {

    public class CoreConfig : CoreMainLevelObject {

        public string Prefix {get; private set;}
        public string ILDatabase {get; private set;}
        public string BLDatabase {get; private set;}
        public string BTDatabase {get; private set;}
        public string ALDatabase {get; private set;}
        public string EtlDbServer {get; private set;}
        public string WhitespaceBefore { get; set; }

        public CoreConfig(TmpConfig tmp) {
            WhitespaceBefore = tmp.WhitespaceBefore;
            
            foreach(var param in tmp.Parameters) {
                switch(param.Name) {
                    case "prefix":
                        this.Prefix = param.Value;
                        break;
                    case "il_database": 
                        this.ILDatabase = param.Value;
                        break;
                    case "bl_database":
                        this.BLDatabase = param.Value;
                        break;
                    case "bt_database":
                        this.BTDatabase = param.Value;
                        break;
                    case "al_database":
                        this.ALDatabase = param.Value;
                        break;
                    case "etl_db_server":
                        this.EtlDbServer = param.Value;
                        break;
                    default:
                        throw new InvalidParameterException($"Ungültiger Parameter {param.Name} in der Config");
                }
            }
        }
    }

}