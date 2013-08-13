using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Raven.Client;
using System.Configuration;
using System.Collections.Concurrent;
using System.Reflection;
using MMDB.DataService.Data.Settings;

namespace MMDB.DataService.Data
{
	public class ConnectionSettingsManager : IConnectionSettingsManager
	{
		public IDocumentSession DocumentSession { get; set; }
		private ConcurrentDictionary<string,IEnumerable<PropertyInfo>> PropertyDictionary;

		public ConnectionSettingsManager()
		{
			this.PropertyDictionary = new ConcurrentDictionary<string, IEnumerable<PropertyInfo>>();
		}

		public ConnectionSettingsManager(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
			this.PropertyDictionary = new ConcurrentDictionary<string,IEnumerable<PropertyInfo>>();
		}

		public virtual T Load<T>(EnumSettingSource source, string key) where T:ConnectionSettingBase, new()
		{
			T returnValue;
			switch(source)
			{
				case EnumSettingSource.AppSetting:
					returnValue = TryLoadAppSettings<T>(key);
					break;
				case EnumSettingSource.ConnectionString:
					returnValue = TryLoadConnectionString<T>(key);
					break;
				case EnumSettingSource.Database:
					returnValue = TryLoadDatabase<T>(key);
					break;
				default:
					throw new UnknownEnumValueException(source);
			}
			return returnValue;
		}

		private T TryLoadDatabase<T>(string key) where T:ConnectionSettingBase
		{
			return this.DocumentSession.Query<T>().SingleOrDefault(i=>i.Key == key);
		}

		private T TryLoadConnectionString<T>(string key) where T : ConnectionSettingBase, new()
		{
			var data = ConfigurationManager.ConnectionStrings[key];
			if(data == null)
			{
				throw new Exception(string.Format("ConnectionString \"{0}\" not found", key));
			}
			if(data == null || string.IsNullOrEmpty(data.ConnectionString))
			{
				return null;
			}
			else 
			{
				return ParseStringData<T>(data.ConnectionString, key);
			}
		}

		private T TryLoadAppSettings<T>(string key) where T : ConnectionSettingBase, new()
		{
			string data = ConfigurationManager.AppSettings[key];
			if (data == null)
			{
				throw new Exception(string.Format("AppSetting \"{0}\" not found", key));
			}
			if (string.IsNullOrEmpty(data))
			{
				return null;
			}
			else 
			{
				return ParseStringData<T>(data, key);
			}
		}

		private T ParseStringData<T>(string data, string key) where T: ConnectionSettingBase, new()
		{
			T returnValue = new T
			{
				Key = key
			};
			var propertyList = this.GetPropertyList<T>();
			if(propertyList != null)
			{
				var keyValueList = data.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
				foreach(var keyValueItem in keyValueList)
				{
					var keyValueParts = keyValueItem.Trim().Split(new char[] {'='}, StringSplitOptions.RemoveEmptyEntries);
					if(keyValueParts.Length >= 2)
					{
						string keyItem = keyValueParts[0].Trim();
						string valueItem = keyValueParts[1].Trim();
						var propInfo = propertyList.FirstOrDefault(i=>i.Name == keyItem);
						if(propInfo != null && !string.IsNullOrEmpty(valueItem))
						{
							if(propInfo.PropertyType == typeof(int?)) 
							{
								var typedValue = int.Parse(valueItem);
								propInfo.SetValue(returnValue, typedValue, null);
							}
							else 
							{
								var typedValue = Convert.ChangeType(valueItem, propInfo.PropertyType);
								propInfo.SetValue(returnValue, typedValue, null);
							}
						}
					}
				}
				var connectionStringProperty = propertyList.FirstOrDefault(i=>i.Name == "ConnectionString");
				if(connectionStringProperty != null)
				{
					connectionStringProperty.SetValue(returnValue, data, null);
				}
			}
			return returnValue;
		}

		private IEnumerable<PropertyInfo> GetPropertyList<T>()
		{
			IEnumerable<PropertyInfo> returnValue;
			string typeName = typeof(T).FullName;
			if(!this.PropertyDictionary.TryGetValue(typeName, out returnValue))
			{
				returnValue = typeof(T).GetProperties();
				this.PropertyDictionary.TryAdd(typeName, returnValue);
			}
			return returnValue;
		}

	}
}
