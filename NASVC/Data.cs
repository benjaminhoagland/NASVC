using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Globalization;

namespace NASVC
{
    public partial class Data
    {

        public static class Filesystem
        {
            public static string filePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            public static string directoryPath = System.IO.Path.GetDirectoryName(filePath);
            public const string filename = "NADB.sqlite";
            public static string fullName
            {
                // example: "C:\NodeAlive\NASVC\NASVC\bin\Debug\NADB.sqlite"
                get
                {
                    return Path.Combine(directoryPath, filename);
                }
            }
        }
        public static class Update
		{
            public static void Node(string guid, DateTime lastResponse, bool alive)
			{
                var query = "UPDATE node " + 
                            "SET last_response = \"" + lastResponse.ToString(Data.timeformat) + "\", " +
                            "alive = \"" + (alive ? 1 : 0).ToString() + "\" " +
                            "WHERE guid = \"" + guid + "\";";
                Log.Write("used query: " + Environment.NewLine + query);
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                command.ExecuteNonQuery();
			    connection.Close();
            }
		}
        public static class Select
        {
            public static List<Schema.Table.Node> Node()
            {
                var returnList = new List<Schema.Table.Node>();
                var query = "SELECT * FROM node;";
                var log = false;
                if(log) Log.Write("Used query: \"" + System.Environment.NewLine + query + "\"");
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = query;
                connection.Open();
                SQLiteDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    Schema.Table.Node node = new Schema.Table.Node();
                    int index = 0;
                    int i; Int32.TryParse(reader[index++].ToString(),out i); node.ID = i;
                    node.EntityGUID = reader[index++].ToString();
                    node.Name = reader[index++].ToString();
                    node.GUID = reader[index++].ToString();
                    node.DateCreated = DateTime.ParseExact(reader[index++].ToString(),timeformat,CultureInfo.InvariantCulture);
                    int t; Int32.TryParse(reader[index++].ToString(),out t); node.Type = t;
                    node.MapGUID = reader[index++].ToString();
                    node.ClusterGUID = reader[index++].ToString();
                    int timeout; Int32.TryParse(reader[index++].ToString(),out timeout); node.Timeout = timeout;
                    if(reader[index++].ToString() == 1.ToString()) { node.Alive = true; } else { node.Alive = false; };
                    node.LastResponse = DateTime.ParseExact(reader[index++].ToString(),timeformat,CultureInfo.InvariantCulture);
                    returnList.Add(node);
                }
                try
                {
                }
                catch
                {
                    Log.WriteError("Database connection failure at " + MethodBase.GetCurrentMethod().Name);
                    Log.WriteWarning("Used query: \"" + System.Environment.NewLine + query + "\"");
                }
                connection.Close();
                return returnList;
            }
            public static List<Schema.Table.Script> Script()
            {
                var returnList = new List<Schema.Table.Script>();
                var query = "SELECT * FROM script;";
                var log = false;
                if(log) Log.Write("Used query: \"" + System.Environment.NewLine + query + "\"");
                try
                {
                    SQLiteConnection connection = new SQLiteConnection(connectionString);
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    connection.Open();
                    SQLiteDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        Schema.Table.Script script = new Schema.Table.Script();
                        int index = 0;
                        int i; Int32.TryParse(reader[index++].ToString(),out i); script.ID = i;
                        script.GUID = reader[index++].ToString();
                        script.NodeGUID = reader[index++].ToString();
                        script.Name = reader[index++].ToString();
                        script.DateCreated = DateTime.ParseExact(reader[index++].ToString(),timeformat,CultureInfo.InvariantCulture);
                        script.Path = reader[index++].ToString();
                        script.Contents = reader[index++].ToString();
                        returnList.Add(script);
                    }
                }
                catch
                {
                    Log.WriteError("Database connection failure at " + MethodBase.GetCurrentMethod().Name);
                    Log.WriteWarning("Used query: \"" + System.Environment.NewLine + query + "\"");
                }
                return returnList;
            }
            public static List<Schema.Table.Result> Result()
            {
                var returnList = new List<Schema.Table.Result>();
                var query = "SELECT * FROM result;";
                var log = false;
                if(log) Log.Write("Used query: \"" + System.Environment.NewLine + query + "\"");
                try
                {
                    SQLiteConnection connection = new SQLiteConnection(connectionString);
                    SQLiteCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    connection.Open();
                    SQLiteDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        Schema.Table.Result result = new Schema.Table.Result();
                        int index = 0;
                        int i; Int32.TryParse(reader[index++].ToString(),out i); result.ID = i;
                        result.GUID = reader[index++].ToString();
                        result.NodeGUID = reader[index++].ToString();
                        result.ScriptGUID = reader[index++].ToString();
                        result.MapGUID = reader[index++].ToString();
                        result.DateCreated = DateTime.ParseExact(reader[index++].ToString(),timeformat,CultureInfo.InvariantCulture);
                        bool s; bool.TryParse(reader[index++].ToString(),out s); result.Status = s;
                        result.Contents = reader[index++].ToString();
                        result.DateFinishedExecution = DateTime.ParseExact(reader[index++].ToString(),timeformat,CultureInfo.InvariantCulture);
                        int r; Int32.TryParse(reader[index++].ToString(),out r); result.Runtime = r;
                        returnList.Add(result);
                    }
                }
                catch
                {
                    Log.WriteError("Database connection failure at " + MethodBase.GetCurrentMethod().Name);
                    Log.WriteWarning("Used query: \"" + System.Environment.NewLine + query + "\"");
                }
                return returnList;
            }
        }
        public static void Initialize() 
        {
            Log.Write("Initializing database...");
            if(File.Exists(Filesystem.fullName))
            {
                Log.Write("Database " + Filesystem.filename + " found at " + Filesystem.directoryPath);
                Log.Write("Continuing...");

                // FLAG:TODO test connection
                // FLAG:TODO write log and exit app on connection failure
                // FLAG:TODO check data initialization
    
            }
			else
			{
                Log.WriteError("Database " + Filesystem.filename + " not found at " + Filesystem.directoryPath);
                Log.Write("Creating file...");
                try
                {
                    SQLiteConnection connection = new SQLiteConnection(connectionString);
                    SQLiteConnection.CreateFile(Filesystem.fullName);
                    connection.Close();
                    Log.Write("Database " + Filesystem.filename + " created successfully at " + Filesystem.directoryPath);
                    
                }
                catch
                {
                    Log.WriteError("Database creation failure."); 
                }
			}

        }
        public static string GetDBName()
        {
            return "The database name is " + Filesystem.fullName;
        }
        public static string connectionString = "Data Source=" + Filesystem.fullName + ";Version=3;New=True;Compress=True;";
        public static string timeformat = "yyyy-MM-dd HH:mm:ss"; 
        public class TableStructure
	    {
            public string Name { get; set; }
            public List<Attribute> Attributes { get; set; }
            public class Attribute
		    {
                public string Name { get; set; }
                public string Type { get; set; }
                public Attribute(string name, string type)
			    {
                    Name = name;
                    Type = type;
			    }

		    }
            public TableStructure(string name, List<Attribute> attributes)
		    {
                Name = name;
                Attributes = attributes;
		    }
	    }
        public class RecordStructure
	    {
            public class Attribute
		    {
                public string Name { get; set; }
                public string Value { get; set; }
                public Attribute()
                { 
                    Name = null;
                    Value = null;
                }
                public Attribute(string column, string value)
			    {
                    Name = column;
                    Value = value;
			    }
		    }
            public List<Attribute> attributes;
	    }
        public static void Insert(string tableName, List<RecordStructure.Attribute> columnValuePairs, bool log = false)
	    {
            // SQLite syntax:
            // INSERT INTO table (column1,column2 ,..)
            // VALUES( value1,	value2 ,...);
            var pairs = from p in columnValuePairs select p;
            string query = 
                "INSERT INTO " + tableName;
            query += "(" + System.Environment.NewLine;
            var last = pairs.Last<RecordStructure.Attribute>();
            foreach(var p in pairs)
		    {
                query += p.Name;
                if(!p.Equals(last)) { query += ","; }
                query += System.Environment.NewLine;
		    }
            query += ")" + System.Environment.NewLine;
            query += "VALUES (" + System.Environment.NewLine;
            foreach(var p in pairs)
		    {
                query += @"'" + p.Value + @"'";
                if(!p.Equals(last)) { query += ","; }
                query += System.Environment.NewLine;
		    }
            query += ");";

            if(log) Log.Write("Used query: \"" + System.Environment.NewLine + query + "\"");
            // Debug.Log(query);
            // return;
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();
            try
		    {
		    }
            catch
		    {
                Log.WriteError("Database connection failure at " + MethodBase.GetCurrentMethod().Name);
		        Log.WriteWarning("Used query: " + System.Environment.NewLine + query);
            }
	    }
    }
}
