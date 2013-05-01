using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
	public interface IRavenManager
	{
		byte[] GetAttachment(string attachmentId);
		string GetAttachmentString(string attachmentId);
		void SetAttachment(string attachmentId, string attachmentData);
	}
}
