using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace NASVC
{
    public class Data
    {

        public static class Filesystem
        {
            public static string filePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            public static string directoryPath = System.IO.Path.GetDirectoryName(filePath);
            public const string filename = "NADB.sqlite";

            /*public const SQLiteChangeSetStartFlags Flags =
                // open the database in read/write mode
                SQLiteChangeSetStartFlags. |
                // create the database if it doesn't exist
                SQLite.SQLiteOpenFlags.Create |
                // enable multi-threaded database access
                SQLite.SQLiteOpenFlags.SharedCache;*/



            public static string fullName
            {
                // example: "C:\NodeAlive\NASVC\NASVC\bin\Debug\NADB.sqlite"
                get
                {
                    return Path.Combine(directoryPath, filename);
                }
            }
        }

        


        public static void Initialize() 
        {
            Log.Write("Initializing database...");
            try
            {
                string connectionString = "Data Source=" + Filesystem.fullName + ";Version=3;New=True;Compress=True;";
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                SQLiteConnection.CreateFile(Filesystem.fullName);
                Log.Write("Database " + Filesystem.filename + " created successfully at " + Filesystem.directoryPath);
            }
            catch
            {
                Log.Write("Database creation failure."); 
            }
        }
        public static string GetDBName()
        {
            return "The database name is " + Filesystem.fullName;
        }
    }
}
