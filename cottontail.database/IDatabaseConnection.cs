using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NLua;
using cottontail.projects;

namespace cottontail.database
{
	public interface IDatabaseConnection : IDisposable
	{
		void Execute (string sql, LuaTable lt);
			
		List<Artifact> Views ();
		
		List<Artifact> Tables ();
		
		int RowCount (Artifact a);
		
		DataTable Browse (Artifact a);

	}
}

