using KinectComputerVision.CVFrames;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace KinectComputerVision
{

    
    public class Database
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;


        public Database()
        {
            string curPath = Directory.GetCurrentDirectory();
            var sqlite = new SQLiteConnection("Data Source=" + curPath + "\\database\\current.sqlite");

            this.sql_con = sqlite;
            this.CreateDBStructure();
        }

        private void CreateDBStructure()
        {
            sql_con.Open();
            string sql = "BEGIN TRANSACTION;";

            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add("type_id", "INTEGER NOT NULL DEFAULT 0");
            settings.Add("trained", "INTEGER NOT NULL DEFAULT 0");
            sql += CreateTableGeneric("classifier", settings);
            sql += CreateTableGeneric("preprocessor", settings);
            settings.Clear();
            settings.Add("prepro_id", "INTEGER NOT NULL");
            settings.Add("data", "BLOB NOT NULL");
            sql += CreateTableGeneric("sub_prepro", settings);
            settings.Clear();
            settings.Add("type_id", "INTEGER NOT NULL UNIQUE");
            settings.Add("data", "BLOB NOT NULL");
            settings.Add("cla_id", "INTEGER NOT NULL");
            settings.Add("pre_id", "INTEGER NOT NULL");
            settings.Add("label", "INTEGER NOT NULL");
            sql += CreateTableGeneric("cv_frame", settings);
            settings.Remove("type_id");
            settings.Remove("data");
            settings.Add("name", "TEXT NOT NULL UNIQUE");
            settings.Add("timestamp", "INTEGER NOT NULL DEFAULT 0");
            sql += CreateTableGeneric("person", settings);
            settings.Clear();
            settings.Add("classi_id", "INTEGER NOT NULL");
            settings.Add("data", "BLOB NOT NULL");
            sql += CreateTableGeneric("sub_classi", settings);
            settings.Clear();
            settings.Add("key", "TEXT NOT NULL");
            settings.Add("value", "TEXT NOT NULL");
            sql += CreateTableGeneric("settings", settings);
            settings.Clear();
            settings.Add("cla_id", "INTEGER NOT NULL");
            settings.Add("set_id", "INTEGER NOT NULL");
            sql += CreateTableGeneric("settings_classif", settings);
            sql += CreateTableGeneric("settings_prepro", settings);
            sql += "COMMIT;";

            SQLiteCommand command = new SQLiteCommand(sql, sql_con);
            command.ExecuteNonQuery();
        }


    
        public void ExecuteQuery(string txtQuery)
        {
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuery;
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        public static CVModel LoadModel(CVModelType type, int id = 0)
        {
            throw new NotImplementedException();
        }

        public static void SaveModel(CVModel model)
        {
            throw new NotImplementedException();
        }

        public static void SaveFrame(CVFrame frame)
        {
            throw new NotImplementedException();
        }


        private string CreateTableGeneric(string table_name, Dictionary<String, String> rows)
        {
            string rowStr  = "('id' INTEGER NOT NULL UNIQUE, " + string.Join(",", rows.Select(x => "'" + x.Key + "' " + x.Value).ToArray()) + ", PRIMARY KEY('id' AUTOINCREMENT));";
            string query = String.Format("CREATE TABLE IF NOT EXISTS '{0}'{1}", table_name, rowStr);
            return query;
        }


    }
}
