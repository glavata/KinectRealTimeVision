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


    public static class Database
    {
        private static SQLiteConnection sql_con;
        private static SQLiteCommand sql_cmd;
        private static SQLiteDataAdapter DB;

        private static void SetConnection()
        {
            sql_con = new SQLiteConnection
                ("Data Source=cv_db.db;Version=3;New=False;Compress=True;");
        }

        public static void ExecuteQuery(string txtQuery)
        {
            SetConnection();
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
