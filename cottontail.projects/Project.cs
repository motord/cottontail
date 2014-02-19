using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace cottontail.projects
{
	public class Project
	{
		private List<Artifact> templates;
		private List<Artifact> messengers;
		private List<Artifact> libraries;
		private List<Artifact> scripts;
		private List<Artifact> dbconnections;
		
		private string folder; 

		public Project (string fd)
		{
			folder=fd;
			templates= new List<Artifact>();
			messengers= new List<Artifact>();
			libraries= new List<Artifact>();
			scripts=new List<Artifact>();
			dbconnections = new List<Artifact>();
			
			foreach (string file in Directory.GetFiles(folder, "*.messenger"))
			{
				Messengers.Add(new Artifact(file, Category.Messenger));
			}
			foreach (string file in Directory.GetFiles(folder, "*.tmpl"))
			{
				Templates.Add (new Artifact(file, Category.Template));
			}
			foreach (string file in Directory.GetFiles(folder, "*.postgresql"))
			{
				DBConnections.Add(new Artifact(file, Category.DBConnection));
			}
			foreach (string file in Directory.GetFiles(folder, "*.sqlite"))
			{
				DBConnections.Add(new Artifact(file, Category.DBConnection));
			}
			foreach (string file in Directory.GetFiles(folder, "*.lib"))
			{
				Libraries.Add(new Artifact(file, Category.Library));
			}
			foreach (string file in Directory.GetFiles(folder, "*.lua"))
			{
				Scripts.Add(new Artifact(file, Category.Script));
			}
		}
		
		public List<Artifact> Templates
		{
			get {return templates;}
		}
		
		public List<Artifact> Messengers
		{
			get {return messengers;}
		}
		
		public List<Artifact> DBConnections
		{
			get {return dbconnections;}
		}

		public List<Artifact> Libraries
		{
			get {return libraries;}
		}

		public List<Artifact> Scripts
		{
			get {return scripts;}
		}
		
		public override string ToString ()
		{
			return string.Format ("Project: {0}", folder);
		}
	}
}

