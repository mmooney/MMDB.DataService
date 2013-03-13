using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Raven.Client;

namespace MMDB.DataService.Data
{
	public class RavenManager : IRavenManager
	{
		private IDocumentSession DocumentSession { get; set; }

		public RavenManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public string GetAttachmentString(string attachmentId)
		{
			var attachment = this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.GetAttachment(attachmentId);
			if(attachment == null)
			{
				throw new Exception(string.Format("Attachment {0} not found", attachmentId));
			}
			using (var reader = new StreamReader(attachment.Data()))
			{
				return reader.ReadToEnd();
			}

		}
		public byte[] GetAttachment(string attachmentId)
		{
			var attachment = this.DocumentSession.Advanced.DocumentStore.DatabaseCommands.GetAttachment(attachmentId);
			if(attachment == null)
			{
				throw new Exception("Attachment Not Found: " + attachmentId);
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
