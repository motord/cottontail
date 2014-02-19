using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Data.Sqlite;
using Npgsql;
using NLua;
using cottontail.projects;

namespace cottontail.database
{
	public class SQLiteConnection : IDatabaseConnection
	{
		private string host;
		private int port;
		private string username;
		private string password;
		private string database;
		private	IDbConnection dbcon;
		private const string template = @"{0}{
      host = ""{1}"",
      port = {2},
      username = ""{3}"",
      password = ""{4}"",
      database = ""{5}""
    }";
		private const string pattern = @"(.*){\s*host\s*=\s*""(.*)"",\s*port\s*=\s*(.*),\s*username\s*=\s*""(.*)"",\s*password\s*=\s*""(.*)"",\s*database\s*=\s*""(.*)""\s*}";

		public SQLiteConnection (LuaTable lt)
		{
			host = (string)lt ["host"];
			port = Convert.ToInt16(lt ["port"]);
			username = (string)lt ["username"];
			password = (string)lt ["password"];
			database = (string)lt ["database"];
			string connectionString = String.Format ("Server={0};Port={1};Database={4};User ID={2};Password={3};", 
			                                        host, port, username, password, database);
			dbcon = new NpgsqlConnection (connectionString);
			dbcon.Open ();
		}
		
		public SQLiteConnection (Artifact a)
		{
			StreamReader reader = new StreamReader (a.Path);
			Match match = Regex.Match (reader.ReadToEnd (), pattern, RegexOptions.IgnoreCase);
			host = match.Groups [2].Value;
			port = Int16.Parse (match.Groups [3].Value);
			username = match.Groups [4].Value;
			password = match.Groups [5].Value;
			database = match.Groups [6].Value;
			string connectionString = String.Format ("Server={0};Port={1};Database={4};User ID={2};Password={3};", 
			                                        host, port, username, password, database);
			dbcon = new NpgsqlConnection (connectionString);
			dbcon.Open ();
		}

		public void Execute (string sql, LuaTable lt)
		{
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader ();
			int i = 1;
			while (reader.Read()) {
				for (int j = 0; j < reader.FieldCount; j++) {
					((LuaTable)(lt [i])) [reader.GetName (j)] = reader.GetValue (j);
				}
				i++;
			}
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
		}
			
		public List<Artifact> Views ()
		{
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = "SELECT table_name FROM INFORMATION_SCHEMA.views WHERE table_schema = ANY (current_schemas(false))";
			IDataReader reader = dbcmd.ExecuteReader ();
			List<Artifact> views = new List<Artifact> ();
			while (reader.Read()) {
				Artifact v = new Artifact (reader.GetString (0), Category.View);
				views.Add (v);
			}
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			return views;
		}
		
		public List<Artifact> Tables ()
		{
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = "SELECT table_name FROM INFORMATION_SCHEMA.tables WHERE table_schema = ANY (current_schemas(false))";
			IDataReader reader = dbcmd.ExecuteReader ();
			List<Artifact> tables = new List<Artifact> ();
			while (reader.Read()) {
				Artifact t = new Artifact (reader.GetString (0), Category.Table);
				tables.Add (t);
			}
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			return tables;
		}
		
		public int RowCount(Artifact a)
		{
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = String.Format("SELECT COUNT(*) FROM {0}", a.Path);
			IDataReader reader = dbcmd.ExecuteReader ();
			reader.Read();
			int rowCount=Convert.ToInt32(reader.GetInt64(0));
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			return rowCount;
		}

		public string Template {
			get { return template;}	
		}
		
		public string Pattern {
			get { return pattern;}
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			dbcon.Close ();
			dbcon = null;
		}
		#endregion
	}
}


