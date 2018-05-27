using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;

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
        public bool TableExists(string tableName) {
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

        public bool IsColumnSameDefinition(IBLAttribute attr) {
            throw new NotImplementedException("Noch nicht implementiert");
            /*
            using(var cmd = con.CreateCommand()) {
                cmd.CommandText = "select 1 from information_schema.columns where table_name = @table_name and table_schema = 'dbo' and column_name = @column_name";
                cmd.Prepare();
                cmd.Parameters.Add(new SqlParameter("table_name", tableName));
                cmd.Parameters.Add(new SqlParameter("column_name", columnName));
                object result = cmd.ExecuteScalar();
                return result != null;
            } 
            */           
        }
    }
}