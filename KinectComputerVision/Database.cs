using KinectComputerVision.CVFrames;
using System;
using System.Data.SQLite;

namespace KinectComputerVision
{

    public enum FrameType
    {
        DEPTH_FACE = 1,
        RGB_FACE = 2,
        SKELETON = 3,
        FULL_FACE = 4 //HD FACE + ANGLE
    }


    public class Database
    {
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        private SQLiteDataAdapter DB;


        public Database()
        {
            var sqlite = new SQLiteConnection("Data Source=G:\\project\\KinectRealTimeVision\\KinectComputerVision\\bin\\Debug\\x86\\database\\current.sqlite");

            this.sql_con = sqlite;
            this.CreateDBStructure();
        }

        private void CreateDBStructure()
        {
            sql_con.Open();

            string sql = @"BEGIN TRANSACTION; 
            CREATE TABLE IF NOT EXISTS 'classifier'(
                'id'    INTEGER NOT NULL UNIQUE,
                'type_id'   INTEGER NOT NULL DEFAULT 0,
                'trained'   INTEGER NOT NULL DEFAULT 0,
                PRIMARY KEY('id' AUTOINCREMENT)
            );
            CREATE TABLE IF NOT EXISTS 'preprocessor'(
                'id'    INTEGER NOT NULL UNIQUE,
                'type_id'   INTEGER NOT NULL DEFAULT 0,
                'trained'   INTEGER NOT NULL DEFAULT 0,
                PRIMARY KEY('id' AUTOINCREMENT)
            );
            CREATE TABLE IF NOT EXISTS 'sub_prepro'(
                'id'    INTEGER NOT NULL UNIQUE,
                'prepro_id' INTEGER NOT NULL,
                'data'  BLOB NOT NULL,
                PRIMARY KEY('id' AUTOINCREMENT)
            );
            CREATE TABLE IF NOT EXISTS 'cv_frame'(
                'id'    INTEGER NOT NULL UNIQUE,
                'type_id'   INTEGER NOT NULL,
                'data'  BLOB NOT NULL,
                'cla_id'    INTEGER NOT NULL,
                'pre_id'    INTEGER NOT NULL,
                'label' INTEGER NOT NULL,
                PRIMARY KEY('id' AUTOINCREMENT)
            );
            CREATE TABLE IF NOT EXISTS 'person'(
                'id'    INTEGER NOT NULL UNIQUE,
                'cla_id'    INTEGER NOT NULL,
                'pre_id'    INTEGER NOT NULL,
                'name'  TEXT NOT NULL UNIQUE,
                'label' INTEGER NOT NULL,
                'timestamp' INTEGER NOT NULL DEFAULT 0,
                PRIMARY KEY('id' AUTOINCREMENT)
            );
            CREATE TABLE IF NOT EXISTS 'sub_classi'(
                'id'    INTEGER NOT NULL UNIQUE,
                'classi_id' INTEGER NOT NULL,
                'data'  BLOB NOT NULL,
                PRIMARY KEY('id' AUTOINCREMENT)
            );
            CREATE TABLE IF NOT EXISTS 'settings'(
                'id'    INTEGER NOT NULL UNIQUE,
                'key'   TEXT NOT NULL,
                'value' TEXT NOT NULL,
                PRIMARY KEY('id' AUTOINCREMENT)
            );
            CREATE TABLE IF NOT EXISTS 'settings_classif'(
                'id'    INTEGER NOT NULL UNIQUE,
                'cla_id'    INTEGER NOT NULL,
                'set_id'    INTEGER NOT NULL,
                PRIMARY KEY('id' AUTOINCREMENT)
            );
            CREATE TABLE IF NOT EXISTS 'settings_prepro'(
                'id'    INTEGER NOT NULL UNIQUE,
                'pre_id'    INTEGER NOT NULL,
                'set_id'    INTEGER NOT NULL,
                PRIMARY KEY('id' AUTOINCREMENT)
            );
            COMMIT;";

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

        public static CVModel LoadModel(CVModel type, int id = 0)
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



    }
}
