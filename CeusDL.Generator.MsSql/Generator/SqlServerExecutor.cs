using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CeusDL.Generator.MsSql.Generator
{
    public class SqlServerExecutor : IExecutor
    {
        private DbConnection con = null;        
        private string baseDirectory = null;

        public SqlServerExecutor(string connectionString, string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
            con = new SqlConnection(connectionString);
            con.Open();         
        }

        public void Dispose()
        {            
            con.Dispose();
        }

        public void ExecuteSQL(string sqlFileName)
        {
            sqlFileName = Path.Combine(baseDirectory, sqlFileName);
            if (File.Exists(sqlFileName))
            {
                var sql = File.ReadAllText(sqlFileName);                                
                var scripts = Regex.Split(sql, @"[ \n\r\t]+[gG][oO][ \n\r\t]+", RegexOptions.Multiline);
                using(var cmd = con.CreateCommand()) {
                    foreach(var script in scripts.Where(s => !string.IsNullOrWhiteSpace(s))) {
                        cmd.CommandText = script;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                throw new FileNotFoundException(sqlFileName);
            }
        }
    }
}
