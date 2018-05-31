using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.BL;

namespace KDV.CeusDL.Utilities.BL {
    public class ModificationAnalyzer {
        private BLModel model = null;
        private DbConnection con = null;

        public ModificationAnalyzer(BLModel model, SqlConnection con) {
            this.model = model;
            this.con = con;
        }

        // TODO: Hier die vergleichende Analyse des BL-Inhalts lt. Datenbankschema (information_schema) und
        //       des gewünschten Zustands lt. CEUSDL (BLModel) durchführen...
        #region complex operations

        public bool TableExistsUnmodified(IBLInterface ifa) {
            if(!TableWithNameExists(ifa.Name)) {
                return false;
            }

            foreach(var attr in ifa.Attributes) {
                if(!ColumnExists(ifa.Name, attr.Name)) {
                    return false;
                }
                if(!ColumnHasCorrectType(attr)) {
                    return false;
                }
            }

            return true;
        }

        public bool InterfaceRenamed(IBLInterface ifa) {
            if(TableWithNameExists(ifa.Name)) {
                return false;
            }
            if(!TableWithNameExists(ifa.FormerName)) {
                return false;
            }                       

            return true;
        }

        public bool TableExistsModified(IBLInterface ifa) {
            if(!TableWithNameExists(ifa.Name)) {
                return false;
            }

            bool unmodified = true;
            foreach(var attr in ifa.Attributes) {
                if(!ColumnExists(ifa.Name, attr.Name)) {
                    unmodified &= false;
                }
                if(!ColumnHasCorrectType(attr)) {
                    unmodified &= false;
                }
            }

            unmodified &= !HasRemovedColums(ifa);

            return !unmodified;
        }

        private bool HasRemovedColums(IBLInterface ifa) {
            if(TableWithNameExists(ifa.Name)) {
                var dbCols = GetColumnNamesFromDb(ifa.Name);
                var cdlCols = ifa.Attributes.Select(a => a.Name).ToList<string>();
                foreach(var dbcol in dbCols) {
                    if(!cdlCols.Contains(dbcol)) {
                        return true;
                    }
                }
            }     
            return false;
        }        

        private bool ColumnHasCorrectType(IBLAttribute attr)
        {
            if(!con.State.Equals(System.Data.ConnectionState.Open)) {
                con.Open();
            }
            using(var cmd = con.CreateCommand()) {
                List<SqlParameter> openParams = new List<SqlParameter>();
                cmd.CommandText =  "select 1 from information_schema.columns where table_name = @table_name and table_schema = 'dbo' ";
                cmd.CommandText += "and column_name = @column_name and data_type = @data_type ";
                switch(attr.DataType) {
                    case CoreDataType.VARCHAR:
                        cmd.CommandText += "and character_maximum_length = @max_len";
                        openParams.Add(new SqlParameter("max_len", attr.Length));
                        break;
                    // TODO: für Decimal noch nachtragen ..
                }
                cmd.Prepare();
                cmd.Parameters.Add(new SqlParameter("table_name", attr.ParentInterface.Name));
                cmd.Parameters.Add(new SqlParameter("column_name", attr.Name));
                cmd.Parameters.Add(new SqlParameter("data_type", BLDataType.GetSqlDataType(attr)));
                foreach(var param in openParams) {
                    cmd.Parameters.Add(param);
                }
                object result = cmd.ExecuteScalar();
                return result != null;
            }
        }

        #endregion complex operations
        #region simple operations
        public bool TableWithNameExists(string tableName) {
            if(tableName == null) {
                return false;
            }
            if(!con.State.Equals(System.Data.ConnectionState.Open)) {
                con.Open();
            }
            using(var cmd = con.CreateCommand()) {
                cmd.CommandText = "select 1 from information_schema.tables where table_name = @table_name and table_schema = 'dbo'";
                cmd.Prepare();
                cmd.Parameters.Add(new SqlParameter("table_name", tableName));
                object result = cmd.ExecuteScalar();
                return result != null;
            }            
        }

        public bool ColumnExists(string tableName, string columnName) {
            if(tableName == null || columnName == null) {
                return false;
            }
            if(!con.State.Equals(System.Data.ConnectionState.Open)) {
                con.Open();
            }
            using(var cmd = con.CreateCommand()) {
                cmd.CommandText = "select 1 from information_schema.columns where table_name = @table_name and table_schema = 'dbo' and column_name = @column_name";
                cmd.Prepare();
                cmd.Parameters.Add(new SqlParameter("table_name", tableName));
                cmd.Parameters.Add(new SqlParameter("column_name", columnName));
                object result = cmd.ExecuteScalar();
                return result != null;
            }            
        }        

        public bool TableRenamed(string name, string formerName) {
            if(TableWithNameExists(name)) {
                return false;
            } else if(TableWithNameExists(formerName)) {
                // TODO: Hier müsste eigentlich auch auf die Felder, die automatisch mit
                //       dem Interface-Namen beginnen geprüft werden...
                //       (Das geht aber praktisch nur mit einem Objekt vom Typ IBLInterface)
                return true;
            } else {
                return false;
            }
        }

        public List<string> GetColumnNamesFromDb(string tableName) {
            if(!con.State.Equals(System.Data.ConnectionState.Open)) {
                con.Open();
            }
            List<string> result = new List<string>();
            using(var cmd = con.CreateCommand()) {
                cmd.CommandText = "select column_name from information_schema.columns where table_name = @table_name and table_schema = 'dbo' order by ordinal_position";
                cmd.Prepare();
                cmd.Parameters.Add(new SqlParameter("table_name", tableName));                

                using(var rdr = cmd.ExecuteReader()) {
                    while(rdr.Read()) {
                        result.Add(rdr.GetString(0));
                    }
                }

                return result;
            }
        }

        #endregion simple operations
    }
}