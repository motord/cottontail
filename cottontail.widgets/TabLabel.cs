//
// Authors: John Luke  <jluke@cfl.rr.com>
//          Jacob Ilsø Christensen <jacobilsoe@gmail.com>
// License: LGPL
//
using System;
using System.Drawing;
using Gtk;
using Gdk;
using cottontail.projects;

namespace cottontail.widgets
{
	public class TabLabel : HBox
	{
		private Label title;
		private Gtk.Image icon;
		private EventBox titleBox;
		private static Gdk.Pixbuf closeImage;
		private static Gdk.Pixbuf databaseIcon;
		private static Gdk.Pixbuf tableIcon;
		private static Gdk.Pixbuf viewIcon;
		private static Gdk.Pixbuf messengerIcon;
		private static Gdk.Pixbuf scriptIcon;
		private static Gdk.Pixbuf templateIcon;
		private static Gdk.Pixbuf libraryIcon;
		
		static TabLabel ()
		{
			try {
				closeImage = Gdk.Pixbuf.LoadFromResource ("cottontail.widgets.icons.MonoDevelop.Close.png");
				databaseIcon = Gdk.Pixbuf.LoadFromResource ("cottontail.widgets.icons.database.png");
				tableIcon = Gdk.Pixbuf.LoadFromResource ("cottontail.widgets.icons.table.png");
				viewIcon = Gdk.Pixbuf.LoadFromResource ("cottontail.widgets.icons.view.png");
				messengerIcon = Gdk.Pixbuf.LoadFromResource ("cottontail.widgets.icons.messenger.png");
				scriptIcon = Gdk.Pixbuf.LoadFromResource ("cottontail.widgets.icons.lua.png");
				templateIcon = Gdk.Pixbuf.LoadFromResource ("cottontail.widgets.icons.template.png");
				libraryIcon = Gdk.Pixbuf.LoadFromResource ("cottontail.widgets.icons.library.png");
			} catch (Exception e) {
//				MonoDevelop.Core.LoggingService.LogError ("Can't create pixbuf from resource: MonoDevelop.Close.png", e);
			}
		}
		
		protected TabLabel (IntPtr p): base (p)
		{
		}

		public TabLabel (Artifact a) : base (false, 0)
		{	
			Label label = new Label ();
			label.Text = a.ToString ();
			this.title = label;
			switch (a.Category) {
			case Category.Folder:
				return;
				break;
			case Category.DBConnection:
				this.icon = new Gtk.Image (databaseIcon);
				break;
			case Category.Table:
				this.icon = new Gtk.Image (tableIcon);
				break;
			case Category.View:
				this.icon = new Gtk.Image (viewIcon);
				break;
			case Category.Messenger:
				this.icon = new Gtk.Image (messengerIcon);
				break;
			case Category.Script:
				this.icon = new Gtk.Image (scriptIcon);
				break;
			case Category.Template:
				this.icon = new Gtk.Image (templateIcon);
				break;
			case Category.Library:
				this.icon = new Gtk.Image (libraryIcon);
				break;
			default:
				break;
			}
			icon.Xpad = 2;
			
			EventBox eventBox = new EventBox ();
			eventBox.BorderWidth = 0;
			eventBox.VisibleWindow = false;			
			eventBox.Add (icon);
			this.PackStart (eventBox, false, true, 0);

			titleBox = new EventBox ();
			titleBox.VisibleWindow = false;			
			titleBox.Add (title);
			this.PackStart (titleBox, true, true, 0);
			
			Gtk.Rc.ParseString ("style \"MonoDevelop.TabLabel.CloseButton\" {\n GtkButton::inner-border = {0,0,0,0}\n }\n");
			Gtk.Rc.ParseString ("widget \"*.MonoDevelop.TabLabel.CloseButton\" style  \"MonoDevelop.TabLabel.CloseButton\"\n");
			Button button = new Button ();
			button.CanDefault = false;
			Gtk.Image closeIcon = new Gtk.Image (closeImage);
			closeIcon.SetPadding (0, 0);
			button.Image = closeIcon;
			button.Relief = ReliefStyle.None;
			button.BorderWidth = 0;
			button.Clicked += new EventHandler (ButtonClicked);
			button.Name = "MonoDevelop.TabLabel.CloseButton";
			this.PackStart (button, false, true, 0);
			this.ClearFlag (WidgetFlags.CanFocus);
			this.BorderWidth = 0;

			this.ShowAll ();
		}
		
		public Label Label {
			get { return title; }
			set { title = value; }
		}
		
		public Gtk.Image Icon {
			get { return icon; }
			set { icon = value; }
		}
		
		public event EventHandler CloseClicked;
				
		public void SetTooltip (string tip, string desc)
		{
			titleBox.TooltipText = tip;
		}
		
		protected override bool OnButtonReleaseEvent (EventButton evnt)
		{
			if (evnt.Button == 2 && CloseClicked != null) {
				CloseClicked (this, null);
				return true;
			}
							
			return false;
		}
							
		private void ButtonClicked (object o, EventArgs eventArgs)
		{
			if (CloseClicked != null) {
				CloseClicked (this, null);				
			}
		}
	}
}
