// ==============================================================================
// 
// ACuriousMind and FerventCoder Copyright © 2011 - Released under the Apache 2.0 License
// 
// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
// ==============================================================================
using dropkick.Configuration;
using dropkick.Tasks.RoundhousE;
using dropkick.Wmi;

namespace MMDB.DataService.Deployment
{
	public class DeploymentSettings : DropkickConfiguration
	{

		#region Properties

		public string ServiceName { get; set; }
		public string TargetServiceDirectory { get; set; }

		//service info
		public ServiceStartMode ServiceStartMode { get; set; }
		public string ServiceUserName { get; set; }
		public string ServiceUserPassword { get; set; }
        public string ServiceDependencies { get; set; }

		#endregion
	}
}