using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MMDB.Shared;
using Raven.Client;

namespace MMDB.DataService.Data.Metadata
{
	public class DataServiceViewManager
	{
		private IDocumentSession DocumentSession { get; set; }

		public DataServiceViewManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public List<DataObjectView> GetViewList()
		{
			return this.DocumentSession.Query<DataObjectView>().ToList();
		}

		public string GetViewData(DataObjectView viewObject)
		{
			switch(viewObject.StorageType)
			{
				case EnumViewStorageType.ResourceFile:
					{
						var assembly = Assembly.Load(viewObject.ResourceAssemblyName.Replace(".dll",""));
						using(var stream = assembly.GetManifestResourceStream(viewObject.ResourceIdentifier))
						{
							if(stream == null)
							{
								throw new Exception("Resource " + viewObject.ResourceIdentifier + " not found in assembly" + viewObject.ResourceAssemblyName);
							}
							return StreamHelper.ReadAll(stream);
						}
					}
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public void CreateView(string virtualPath, string resourceAssemblyName, string resourceIdentifier)
		{
			var item = new DataObjectView
			{
				VirtualPath = virtualPath,
				StorageType = EnumViewStorageType.ResourceFile,
				ResourceAssemblyName = resourceAssemblyName,
				ResourceIdentifier = resourceIdentifier
			};
			this.DocumentSession.Store(item);
			this.DocumentSession.SaveChanges();
		}

		public DataObjectView GetView(int id)
		{
			return this.DocumentSession.Load<DataObjectView>(id);
		}

		public void UpdateView(int id, string virtualPath, string resourceAssemblyName, string resourceIdentifier)
		{
			var item = this.GetView(id);
			item.VirtualPath = virtualPath;
			item.ResourceAssemblyName = resourceAssemblyName;
			item.ResourceIdentifier = resourceIdentifier;
			this.DocumentSession.SaveChanges();
		}
	}
}
