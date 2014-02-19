using System;
using System.Linq;
using Gtk;
using cottontail.scripting;
using cottontail.projects;
using cottontail.messaging;
using cottontail.database;

public partial class MainWindow: Gtk.Window
{	
	private Project project;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		InitializeTreeView ();
	}
	
	protected void OnAddDatabase (object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException ();
	}

	protected void OnAddRabbitExchange (object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException ();
	}

	protected void OnStop (object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException ();
	}

	protected void OnExecute (object sender, System.EventArgs e)
	{
		using (LuaRuntime runtime=new LuaRuntime(project)) {
			runtime.Log += OnLog;
			Messenger.logger.Log += OnLog;
			Artifact script = project.Scripts.ToArray () [0];
			runtime.Execute (script);
			runtime.Log -= OnLog;
			Messenger.logger.Log -= OnLog;
		}
	}

	protected void OnLog (object sender, LuaRuntimeEventArgs e)
	{
		textviewLog.Buffer.Text += e.Message + Environment.NewLine;
		textviewLog.ScrollToIter (textviewLog.Buffer.EndIter, 0, true, 0, 0);
	}
	
	protected void OnLog (object sender, MessengerEventArgs e)
	{
		textviewLog.Buffer.Text += e.Message + Environment.NewLine;
		textviewLog.ScrollToIter (textviewLog.Buffer.EndIter, 0, true, 0, 0);
	}

	protected void OnOpenProject (object sender, System.EventArgs e)
	{
		// Create and display a fileChooserDialog
		FileChooserDialog chooser = new FileChooserDialog (
	        "Please select the project folder ...",
	        this,
	        FileChooserAction.SelectFolder,
	        "Cancel", ResponseType.Cancel,
	        "Open", ResponseType.Accept);
	     
		if (chooser.Run () == (int)ResponseType.Accept) {
			project = new Project (chooser.CurrentFolder);
		} // end if
		chooser.Destroy ();
		PopulateTreeView (project);
	}
	
	private Gtk.TreeStore projectTreeStore;

	private void InitializeTreeView ()
	{
		Gtk.TreeViewColumn projectColumn = new Gtk.TreeViewColumn ();
		Gtk.CellRendererText projectCell = new Gtk.CellRendererText ();
 
		// Add the cell to the column
		projectColumn.PackStart (projectCell, true); 
		// Add the columns to the TreeView
		treeviewProject.AppendColumn (projectColumn);
 
		// Tell the Cell Renderers which items in the model to display
		projectColumn.SetCellDataFunc (projectCell, new Gtk.TreeCellDataFunc (RenderCell));

		projectTreeStore = new Gtk.TreeStore (typeof(Artifact));
		
	}

	private void RenderCell (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		Artifact v = (Artifact)model.GetValue (iter, 0);
 
		(cell as Gtk.CellRendererText).Text = v.ToString ();
	}
	
	private void PopulateTreeView (Project project)
	{
		treeviewProject.Columns [0].Title = project.ToString ();
		projectTreeStore.Clear ();

		Gtk.TreeIter iter = projectTreeStore.AppendValues (new Artifact ("Messengers", Category.Folder));
		foreach (Artifact messenger in project.Messengers) {
			projectTreeStore.AppendValues (iter, messenger);
			Messenger m = new Messenger (messenger);
		}

		iter = projectTreeStore.AppendValues (new Artifact ("Templates", Category.Folder));
		foreach (Artifact template in project.Templates) {
			projectTreeStore.AppendValues (iter, template); 
		}

		iter = projectTreeStore.AppendValues (new Artifact ("DBConnections", Category.Folder));
		Gtk.TreeIter iterPostgreSQL = projectTreeStore.AppendValues (iter, new Artifact ("PostgreSQL", Category.Folder));
		foreach (Artifact dbcon in project.DBConnections.Where(c=>c.Extension==".postgresql")) {
			Gtk.TreeIter iterDB = projectTreeStore.AppendValues (iterPostgreSQL, dbcon);
			using (IDatabaseConnection c = new PostgreSQLConnection (dbcon)) {
				Gtk.TreeIter iterTables = projectTreeStore.AppendValues (iterDB, new Artifact ("Tables", Category.Folder));
				foreach (Artifact table in c.Tables()) {
					projectTreeStore.AppendValues (iterTables, table);
				}
				Gtk.TreeIter iterViews = projectTreeStore.AppendValues (iterDB, new Artifact ("Views", Category.Folder));
				foreach (Artifact view in c.Views()) {
					projectTreeStore.AppendValues (iterViews, view);
				}
			}
		}
		Gtk.TreeIter iterSQLite = projectTreeStore.AppendValues (iter, new Artifact ("SQLite", Category.Folder));
		foreach (Artifact dbcon in project.DBConnections.Where(c=>c.Extension==".sqlite")) {
			Gtk.TreeIter iterDB = projectTreeStore.AppendValues (iterSQLite, dbcon);
			using (IDatabaseConnection c = new SQLiteConnection (dbcon)) {
				Gtk.TreeIter iterTables = projectTreeStore.AppendValues (iterDB, new Artifact ("Tables", Category.Folder));
				foreach (Artifact table in c.Tables()) {
					projectTreeStore.AppendValues (iterTables, table);
				}
				Gtk.TreeIter iterViews = projectTreeStore.AppendValues (iterDB, new Artifact ("Views", Category.Folder));
				foreach (Artifact view in c.Views()) {
					projectTreeStore.AppendValues (iterViews, view);
				}
			}
		}
		
		iter = projectTreeStore.AppendValues (new Artifact ("Libraries", Category.Folder));
		foreach (Artifact library in project.Libraries) {
			projectTreeStore.AppendValues (iter, library);
		}
		
		iter = projectTreeStore.AppendValues (new Artifact ("Scripts", Category.Folder));
		foreach (Artifact script in project.Scripts) {
			projectTreeStore.AppendValues (iter, script);
		}
		 
		// Assign the model to the TreeView
		treeviewProject.Model = projectTreeStore;		
	}

	protected void OnPreferences (object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException ();
	}

	protected void OnSaveProject (object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException ();
	}

	protected void OnSaveProjectAs (object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException ();
	}

	protected void OnAddScript (object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException ();
	}

	protected void OnNewProject (object sender, System.EventArgs e)
	{
		// Create and display a fileChooserDialog
		FileChooserDialog chooser = new FileChooserDialog (
	        "Please create the project folder ...",
	        this,
	        FileChooserAction.CreateFolder,
	        "Cancel", ResponseType.Cancel,
	        "Open", ResponseType.Accept);
	     
		if (chooser.Run () == (int)ResponseType.Accept) {
			project = new Project (chooser.Filename);
		} // end if
		chooser.Destroy ();
		PopulateTreeView (project);
	}

	protected void OnCloseProject (object sender, System.EventArgs e)
	{
		throw new System.NotImplementedException ();
	}

	protected void OnQuit (object sender, System.EventArgs e)
	{
		Application.Quit ();
	}

	protected void OnDeleteEvent (object o, Gtk.DeleteEventArgs args)
	{
		Application.Quit ();
		args.RetVal = true;
	}
}
