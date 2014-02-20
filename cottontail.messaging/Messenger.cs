using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using RabbitMQ.Client;
using cottontail.projects;
using NLua;

namespace cottontail.messaging
{
	public class Messenger : IDisposable
	{
		private string host;
		private int port;
		private string virtualhost;
		private string exchange;
		private string username;
		private string password;
		private readonly ConnectionFactory factory;
		private readonly IConnection connection;
		private readonly IModel channel;
		private const string template = @"{0}{
      host = ""{1}"",
      port = {2},
      virtualhost = ""{3}"",
      exchange = ""{4}"",
      username = ""{5}"",
      password = ""{6}""
    }";
		private const string pattern = @"(.*){\s*host\s*=\s*""(.*)"",\s*port\s*=\s*(.*),\s*virtualhost\s*=\s*""(.*)"",\s*exchange\s*=\s*""(.*)"",\s*username\s*=\s*""(.*)"",\s*password\s*=\s*""(.*)""\s*}";
		public static Logger logger = new Logger ();

		public Messenger (LuaTable lt)
		{
			host = (string)lt ["host"];
			port = Convert.ToInt16 (lt ["port"]);
			virtualhost = (string)lt ["virtualhost"];
			exchange = (string)lt ["exchange"];
			username = (string)lt ["username"];
			password = (string)lt ["password"];

			factory = new ConnectionFactory () { HostName = host, VirtualHost=virtualhost, UserName=username, Password=password };
			connection = factory.CreateConnection ();
			channel = connection.CreateModel ();
			channel.ExchangeDeclarePassive (exchange);
		}
		
		public Messenger (Artifact a)
		{
			StreamReader reader = new StreamReader (a.Path);
			Match match = Regex.Match (reader.ReadToEnd (), pattern, RegexOptions.IgnoreCase);
			host = match.Groups [2].Value;
			port = Int16.Parse (match.Groups [3].Value);
			virtualhost = match.Groups [4].Value;
			exchange = match.Groups [5].Value;
			username = match.Groups [6].Value;
			password = match.Groups [7].Value;

			factory = new ConnectionFactory () { HostName = host, VirtualHost=virtualhost, UserName=username, Password=password };
			connection = factory.CreateConnection ();
			channel = connection.CreateModel ();
			channel.ExchangeDeclarePassive (exchange);
		}
		
		public void Publish (string routingKey, string message)
		{
			logger.InfoWrite ("Publishing >> {0}", message);
			IBasicProperties properties = channel.CreateBasicProperties();
			properties.SetPersistent (true);
			properties.ContentEncoding = "UTF-8";
			byte[] body = Encoding.UTF8.GetBytes (message);
			Publish (routingKey, properties, body);
		}
		
		public void PublishJSON (string routingKey, string message)
		{
			logger.InfoWrite ("Publishing(JSON) >> {0}", message);
			IBasicProperties properties = channel.CreateBasicProperties();
			properties.SetPersistent (true);
			properties.ContentEncoding = "UTF-8";
			properties.ContentType = "application/json";
			byte[] body = Encoding.UTF8.GetBytes (message);
			Publish (routingKey, properties, body);
		}

		private void Publish (string routingKey, IBasicProperties properties, byte[] body)
		{
			channel.BasicPublish (exchange, routingKey, properties,body);
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
			channel.Dispose ();
			connection.Dispose ();
		}
		#endregion
	}
}
