using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using MMDB.DataService.Data;
using MMDB.Shared;

namespace MMDB.DataService.Web
{
	public class DataServiceVirtualFile : VirtualFile
	{
		private string FileData { get; set; }

		public DataServiceVirtualFile(string data, string virtualPath) : base(virtualPath)
		{
			this.FileData = data;
		}

		public override bool IsDirectory
		{
			get
			{
				return base.IsDirectory;
			}
		}
		public override Stream Open()
		{
			var stream = StreamHelper.CreateMemoryStream(this.FileData);
			stream.Position = 0;
			return stream;
		}

	}
}