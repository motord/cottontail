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
		private string database;
		private	IDbConnection dbcon;
		private int browserSize;

		public SQLiteConnection (LuaTable lt)
		{
			database = (string)lt [1];
			string connectionString = String.Format ("Data Source={0};Version=3;", database);
			dbcon = new SqliteConnection (connectionString);
			dbcon.Open ();
		}
		
		public SQLiteConnection (Artifact a, int size=100)
		{
			database = a.Path;
			string connectionString = String.Format ("Data Source={0};Version=3;", database);
			dbcon = new SqliteConnection (connectionString);
			dbcon.Open ();
			browserSize=size;
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
			dbcmd.CommandText = "SELECT name FROM sqlite_master WHERE type='view';";
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
			dbcmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
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

		public DataTable Browse(Artifact a)
		{
			IDbCommand dbcmd = dbcon.CreateCommand ();
			dbcmd.CommandText = String.Format("SELECT * FROM {0} LIMIT {1}", a.Path, browserSize);
			IDataReader reader = dbcmd.ExecuteReader ();
			DataTable dt=new DataTable();
			dt.Load(reader);
			reader.Close ();
			reader = null;
			dbcmd.Dispose ();
			dbcmd = null;
			return dt;
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


