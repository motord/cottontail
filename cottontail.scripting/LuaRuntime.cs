using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NLua;
using NLua.Event;
using NLua.Exceptions;
using cottontail.projects;
using cottontail.messaging;

namespace cottontail.scripting
{
	public class LuaRuntime : IDisposable
	{
		private readonly Lua lua = new Lua ();
		private Project project;
		private const string templateTemplate = @"{0} = lutem:new()
ret,errmsg = {0}:load('{1}')";
		private const string packages = @"import('cottontail.database', 'cottontail.database')
import('cottontail.messaging', 'cottontail.messaging')";
		private const string messengerTemplate = @"function {0}(m) {0}= Messenger(m) end";
		private const string postgreSQLTemplate = @"function {0}(m) {0}= PostgreSQLConnection(m) end";
		private const string sqliteTemplate = @"function {0}(m) {0}= SQLiteConnection(m) end";
		private const string libraryTemplate = @"{0}=dofile('{1}')";
		
		public LuaRuntime (Project p)
		{
			lua.LoadCLRPackage ();
			lua.DoString (packages);
			lua.RegisterFunction ("print", this, GetType ().GetMethod ("Print"));
			lua.DebugHook += HandleLuaDebugHook;
			lua.HookException += HandleLuaHookException;
			project = p;
			StreamReader reader = new StreamReader (System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("cottontail.scripting.lutem.lua"));
			lua.DoString (reader.ReadToEnd ());
			reader = new StreamReader (System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("cottontail.scripting.inspect.lua"));
			lua.DoString (reader.ReadToEnd ());
			reader = new StreamReader (System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("cottontail.scripting.uuid.lua"));
			lua.DoString (reader.ReadToEnd ());
			reader = new StreamReader (System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("cottontail.scripting.date.lua"));
			lua.DoString (reader.ReadToEnd ());
		}

		void HandleLuaHookException (object sender, HookExceptionEventArgs e)
		{
			Print (e.Exception.Message);
		}

		void HandleLuaDebugHook (object sender, DebugHookEventArgs e)
		{
			Print (e.LuaDebug.source);
		}
		
		public void Execute (Artifact script)
		{
			foreach (Artifact messenger in project.Messengers) {
				lua.DoString (String.Format (messengerTemplate, messenger.ToString ()));
				lua.DoFile (messenger.Path);
			}
			foreach (Artifact dbcon in project.DBConnections) {
				switch (dbcon.Extension) {
				case ".postgresql":
					lua.DoString (String.Format (postgreSQLTemplate, dbcon.ToString ()));
					lua.DoFile (dbcon.Path);
					break;
				case ".sqlite":				
					lua.DoString (String.Format (sqliteTemplate, dbcon.ToString ()));
					lua.DoString(String.Format("{0}{{'{1}'}}", dbcon.ToString(), dbcon.Path));
					break;
				default:
					break;
				}
			}
			foreach (Artifact template in project.Templates) {
				lua.DoString (String.Format (templateTemplate, template.ToString (), template.Path));
			}
			foreach (Artifact library in project.Libraries) {
				lua.DoString (String.Format(libraryTemplate, library.ToString(), library.Path));
			}
			lua.DoFile (script.Path);
			foreach (Artifact messenger in project.Messengers) {
				lua.DoString (String.Format (@"{0}:Dispose()", messenger.ToString ()));
			}
			foreach (Artifact dbcon in project.DBConnections) {
				lua.DoString (String.Format (@"{0}:Dispose()", dbcon.ToString ()));
			}
		}

		public void Dispose ()
		{
			lua.DebugHook -= HandleLuaDebugHook;
			lua.HookException -= HandleLuaHookException;
			lua.Dispose ();
		}

		public event EventHandler<LuaRuntimeEventArgs> Log;
			
		public void Print (string msg)
		{
			LuaRuntimeEventArgs args = new LuaRuntimeEventArgs ();
			args.Message = string.Format ("LUA> {0}", msg);
			if (Log != null)
				Log (this, args);
		}
	}
}

