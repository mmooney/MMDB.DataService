using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.DataService.Data
{
	public interface IRavenManager
	{
		string GetAttachmentString(string attachmentId);
	}
}
