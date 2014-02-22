using System;
using cottontail.projects;

namespace cottontail
{
	public class ArtifactEventArgs : EventArgs
	{
		public Artifact	CurrentArtifact { get; set; }

		public bool Modified{ get; set; }
	}
}

