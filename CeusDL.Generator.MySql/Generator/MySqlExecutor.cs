using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CeusDL.Generator.MsSql.Generator
{
    public class MySqlExecutor : IExecutor
    {
        private DbConnection con = null;        
        private string baseDirectory = null;

        public MySqlExecutor(string connectionString, string baseDirectory)
        {
            this.baseDirectory = baseDirectory;
            con = new MySqlConnection(connectionString);
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
                var scripts = Regex.Split(sql, @"[ \n\r\t]*;\n\n[ \n\r\t]*", RegexOptions.Multiline);
                using(var cmd = con.CreateCommand()) {
                    foreach(var script in scripts.Where(s => !string.IsNullOrWhiteSpace(s))) {
                        cmd.CommandText = script;
                        try {
                            cmd.ExecuteNonQuery();
                        } catch(Exception ex) {
                            Console.WriteLine("ERROR: "+ ex.Message);
                            if(!script.Contains("alter table")) {
                                Console.WriteLine($"SQL to ERROR: {script}");
                            }
                        }
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
