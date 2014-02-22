using System;
using cottontail.projects;
using cottontail.messaging;

namespace cottontail.widgets
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ExchangeEditor : Gtk.Bin
	{
		public ExchangeEditor (Artifact a)
		{
			this.Build ();
			Messenger m=new Messenger(a);
			entryHost.Text=m.Host;
			entryPort.Text=Convert.ToString(m.Port);
			entryUsername.Text=m.Username;
			entryPassword.Text=m.Password;
			entryVirtualhost.Text=m.Virtualhost;
			entryExchange.Text=m.Exchange;
		}
	}
}

