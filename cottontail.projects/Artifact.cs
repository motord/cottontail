using System;
using System.IO;

namespace cottontail.projects
{
	public class Artifact
	{
		private string path;
		private Category category;

		public Artifact (string p, Category c)
		{
			path = p;
			category=c;
		}

		public override string ToString ()
		{
			return System.IO.Path.GetFileNameWithoutExtension(path);
		}

		public string Path {
			get { return path;}
		}
		
		public string Extension {
			get { return System.IO.Path.GetExtension(path);}	
		}
	}
}

