using System;
using cottontail.projects;
using cottontail.database;

namespace cottontail.widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class DatabaseEditor : Gtk.Bin
	{
		public DatabaseEditor (Artifact a)
		{
			this.Build ();
			using (PostgreSQLConnection con=new PostgreSQLConnection(a)) {
				entryHost.Text = con.Host;
				entryPort.Text = Convert.ToString (con.Port);
				entryUsername.Text = con.Username;
				entryPassword.Text = con.Password;
				entryDatabase.Text = con.Database;
			}
		}
	}
}

