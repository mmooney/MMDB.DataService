using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data
{
	public class RavenManager
	{
		private IDocumentSession DocumentSession { get; set; }

		public RavenManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public byte[] GetAttachment(string attachmentID)
		{
			var attachment = this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.GetAttachment(attachmentID);
			if(attachment == null)
			{
				throw new Exception("Attachment Not Found: " + attachmentID);
			}
			using(var memoryStream = new MemoryStream())
			{
				attachment.Data().CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}

		public IEnumerable<Raven.Abstractions.Data.Attachment> GetAttachmentList()
		{
			return this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.GetAttachmentHeadersStartingWith("",0,int.MaxValue).ToList();
		}
	}
}
