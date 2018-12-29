using System;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

using KDV.CeusDL.Model.Core;
using KDV.CeusDL.Model.MySql.BL;

namespace KDV.CeusDL.Utilities.MySql.BL {
    public class ModificationAnalyzer {
        private BLModel model = null;
        private DbConnection con = null;

        public ModificationAnalyzer(BLModel model, MySqlConnection con) {
            this.model = model;
            this.con = con;
        }

        // TODO: Hier die vergleichende Analyse des BL-Inhalts lt. Datenbankschema (information_schema) und
        //       des gewünschten Zustands lt. CEUSDL (BLModel) durchführen...
        #region complex operations

        public bool InterfaceRenamed(IBLInterface ifa) {
            if(TableWithNameExists(ifa.Name)) {
                return false;
            }
            if(!TableWithNameExists(ifa.FormerName)) {
                return false;
            }                       

            return true;
        }

        // TODO:für TableExistsModified brauch ich noch Testfälle
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
                if(!ConstraintExists(ifa.Name, $"{ifa.Name}_UK", ifa.UniqueKeyAttributes.Select(a => a.Name).ToList())) {
                    unmodified &= false;
                }
            }

            unmodified &= !HasRemovedColums(ifa);

            return !unmodified;
        }

        public List<string> ListDeletedInterfaceNames(BLModel model) {
            var tablesDB = FindAllBLAndDefTablesInDB(model);
            var tablesCEUSDL = model.Interfaces
                                .Select(i => i.Name)
                                .Union(model.Interfaces.Select(i => i.FormerName))
                                .ToList<string>();

            return tablesDB.Where(t => !tablesCEUSDL.Contains(t)).ToList<string>();
        }

        // Ermittelt alle BL und Def-Tabellen in der BL-Datenbank
        // (das schließt gleich auch die _BAK Tabellen mit ein!)
        public List<string> FindAllBLAndDefTablesInDB(BLModel model) {
            List<string> result = new List<string>();
            AssureOpenConnection();
            using(var cmd = con.CreateCommand()) {
                cmd.CommandText = $"select table_name from information_schema.tables ";
                cmd.CommandText += "where table_catalog = @table_catalog ";
                cmd.CommandText += "and (table_name like @name_filter1 or table_name like @name_filter2) and table_type = 'BASE TABLE'";

                cmd.Prepare();

                cmd.Parameters.Add(new MySqlParameter("table_catalog", model.Config.BLDatabase));
                cmd.Parameters.Add(new MySqlParameter("name_filter1", $"{model.Config.Prefix}_BL_%"));
                cmd.Parameters.Add(new MySqlParameter("name_filter2", $"{model.Config.Prefix}_def_%"));

                using(var rdr = cmd.ExecuteReader()) {
                    while(rdr.Read()) {
                        result.Add(rdr.GetString(0));
                    }
                }
            }
            return result;
        }

        // Prüft, ob in der Datenbank noch Spalten vorhanden sind, die im ceusdl-Code
        // schon entfernt wurden.
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
            AssureOpenConnection();
            using(var cmd = con.CreateCommand()) {
                List<MySqlParameter> openParams = new List<MySqlParameter>();
                cmd.CommandText =  "select 1 from information_schema.columns where table_name = @table_name and table_schema = 'dbo' ";
                cmd.CommandText += "and column_name = @column_name and data_type = @data_type ";
                switch(attr.DataType) {
                    case CoreDataType.VARCHAR:
                        cmd.CommandText += "and character_maximum_length = @max_len";
                        openParams.Add(new MySqlParameter("max_len", attr.Length));
                        break;
                    case CoreDataType.DECIMAL:
                        cmd.CommandText += "and numeric_precision = @max_len and numeric_scale = @scale";
                        openParams.Add(new MySqlParameter("max_len", attr.Length));
                        openParams.Add(new MySqlParameter("scale", attr.Decimals));
                        break;
                }
                cmd.Prepare();
                cmd.Parameters.Add(new MySqlParameter("table_name", attr.ParentInterface.Name));
                cmd.Parameters.Add(new MySqlParameter("column_name", attr.Name));
                cmd.Parameters.Add(new MySqlParameter("data_type", BLDataType.GetSqlDataType(attr)));
                foreach(var param in openParams) {
                    cmd.Parameters.Add(param);
                }
                object result = cmd.ExecuteScalar();
                return result != null;
            }
        }

        #endregion complex operations
        #region simple operations
        public bool TableWithNameExists(string tableName)
        {
            if (tableName == null)
            {
                return false;
            }
            AssureOpenConnection();
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "select 1 from information_schema.tables where table_name = @table_name and table_schema = 'dbo'";
                cmd.Prepare();
                cmd.Parameters.Add(new MySqlParameter("table_name", tableName));
                object result = cmd.ExecuteScalar();
                return result != null;
            }
        }

        /// 
        /// Sicherstellen, dass die Datenbankverbindung offen ist und ggf. eine sprechende
        /// Meldung liefern
        ///
        private void AssureOpenConnection()
        {
            if (!con.State.Equals(System.Data.ConnectionState.Open))
            {
                try
                {
                    con.Open();
                }
                catch (MySqlException ex)
                {
                    throw new Exception("Eine Verbindung zur Prüfung der Datenbank konnte nicht hergestellt werden", ex);
                }
            }
        }

        public bool ColumnExists(string tableName, string columnName) {
            if(tableName == null || columnName == null) {
                return false;
            }
            AssureOpenConnection();
            using(var cmd = con.CreateCommand()) {
                cmd.CommandText = "select 1 from information_schema.columns where table_name = @table_name and table_schema = 'dbo' and column_name = @column_name";
                cmd.Prepare();
                cmd.Parameters.Add(new MySqlParameter("table_name", tableName));
                cmd.Parameters.Add(new MySqlParameter("column_name", columnName));
                object result = cmd.ExecuteScalar();
                return result != null;
            }            
        }

        public bool ConstraintExists(string tableName, string constraintName, List<string> fields) {
            if(tableName == null || constraintName == null || fields == null) {
                return false;
            }
            AssureOpenConnection();
            using(var cmd = con.CreateCommand()) {
                cmd.CommandText = "select count(*) from information_schema.CONSTRAINT_COLUMN_USAGE "
                    +"where table_name = @table_name "
                    +"and constraint_name = @constraint_name "
                    +"and column_name in (";                
                for(int i = 0; i < fields.Count; i++) {                    
                    cmd.CommandText += $"@field{i}";
                    if(i != fields.Count-1) {
                        cmd.CommandText += ", ";
                    }
                }
                cmd.CommandText += ")";                
                cmd.Prepare();
                cmd.Parameters.Add(new MySqlParameter("table_name", tableName));
                cmd.Parameters.Add(new MySqlParameter("constraint_name", constraintName));
                for(int i = 0; i < fields.Count; i++) {
                    cmd.Parameters.Add(new MySqlParameter($"field{i}", fields[i]));
                }
                object result = cmd.ExecuteScalar();
                return result != null && ((int)result) == fields.Count;
            }
        }

        public bool TableRenamed(string name, string formerName) {
            if(TableWithNameExists(name)) {
                return false;
            } else if(TableWithNameExists(formerName)) {               
                return true;
            } else {
                return false;
            }
        }

        private List<string> GetColumnNamesFromDb(string tableName) {
            AssureOpenConnection();
            List<string> result = new List<string>();
            using(var cmd = con.CreateCommand()) {
                cmd.CommandText = "select column_name from information_schema.columns where table_name = @table_name and table_schema = 'dbo' order by ordinal_position";
                cmd.Prepare();
                cmd.Parameters.Add(new MySqlParameter("table_name", tableName));                

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