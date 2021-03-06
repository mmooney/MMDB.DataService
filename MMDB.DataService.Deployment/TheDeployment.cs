//ReSharper disable ConvertToLambdaExpression
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
using System.IO;
using dropkick.Configuration.Dsl;
using dropkick.Configuration.Dsl.Files;
using dropkick.Configuration.Dsl.Iis;
using dropkick.Configuration.Dsl.RoundhousE;
using dropkick.Configuration.Dsl.Security;
using dropkick.Configuration.Dsl.WinService;
using dropkick.Wmi;
using System;

namespace MMDB.DataService.Deployment
{
    public class TheDeployment : Deployment<TheDeployment, DeploymentSettings>
    {
        #region Constructors

        public TheDeployment()
        {
            Define(settings =>
            {
				//DeploymentStepsFor(Db,
				//                   s =>
				//                   {
				//                       //s.RoundhousE()
				//                       //    .ForEnvironment(settings.Environment)
				//                       //    .OnDatabase(settings.DbName)
				//                       //    .WithScriptsFolder(settings.DbSqlFilesPath)
				//                       //    .WithDatabaseRecoveryMode(settings.DbRecoveryMode)
				//                       //    .WithRestorePath(settings.DbRestorePath)
				//                       //    .WithRepositoryPath("__REPLACE_ME__")
				//                       //    .WithVersionFile("_BuildInfo.xml")
				//                       //    .WithRoundhousEMode(settings.RoundhousEMode)
				//                       //    ;
				//                   });

				//DeploymentStepsFor(Web,
				//                   s =>
				//                   {
				//                       s.CopyDirectory(@"..\_PublishedWebSites\__REPLACE_ME__").To(@"{{WebsitePath}}").DeleteDestinationBeforeDeploying();

				//                       s.CopyFile(@"..\environment.files\{{Environment}}\{{Environment}}.web.config").ToDirectory(@"{{WebsitePath}}").RenameTo(@"web.config");

				//                       s.Security(securityOptions =>
				//                       {
				//                           securityOptions.ForPath(settings.WebsitePath, fileSecurityConfig => fileSecurityConfig.GrantRead(settings.WebUserName));
				//                           securityOptions.ForPath(Path.Combine(settings.WebsitePath, "logs"), fs => fs.GrantReadWrite(settings.WebUserName));
				//                           securityOptions.ForPath(@"~\C$\Windows\Microsoft.NET\Framework\v4.0.30319\Temporary ASP.NET Files", fs => fs.GrantReadWrite(settings.WebUserName));
				//                           if (Directory.Exists(@"~\C$\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files"))
				//                           {
				//                               securityOptions.ForPath(@"~\C$\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files", fs => fs.GrantReadWrite(settings.WebUserName));
				//                           }
				//                       });
				//                   });

				//DeploymentStepsFor(VirtualDirectory,
				//                   s =>
				//                   {
				//                       s.Iis7Site(settings.VirtualDirectorySite)
				//                        .VirtualDirectory(settings.VirtualDirectoryName)
				//                        .SetAppPoolTo("Default Web Site", pool =>
				//                                        {
				//                                            pool.SetRuntimeToV4();
				//                                            //pool.UseClassicPipeline();
				//                                            //pool.Enable32BitAppOnWin64();
				//                                        }).SetPathTo(@"{{WebsitePath}}");
				//                   });

                DeploymentStepsFor(Host,
                                   s =>
                                   {
                                       //var serviceName = "__REPLACE_ME__.{{Environment}}";
									   var serviceName = "{{ServiceName}}";
                                       s.WinService(serviceName).Stop();

									   s.CopyDirectory(@"WindowsService").To(@"{{TargetServiceDirectory}}").DeleteDestinationBeforeDeploying();

									   s.CopyFile(@"Configs\{{Environment}}\MMDB.DataService.WindowsService.exe.config").ToDirectory(@"{{TargetServiceDirectory}}");

									   s.Security(o =>
									   {
									       o.LocalPolicy(lp =>
									       {
									           lp.LogOnAsService(settings.ServiceUserName);
									           lp.LogOnAsBatch(settings.ServiceUserName);
									       });

									       o.ForPath(settings.TargetServiceDirectory, fs => fs.GrantRead(settings.ServiceUserName));
									   //    //o.ForPath(Path.Combine(settings.TargetServiceDirectory,"logs"), fs => fs.GrantReadWrite(settings.ServiceUserName));
									   });
                                       s.WinService(serviceName).Delete();
									   var service = s.WinService(serviceName).Create().WithCredentials(settings.ServiceUserName, settings.ServiceUserPassword).WithDisplayName("{{ServiceName}}").WithServicePath(@"{{TargetServiceDirectory}}\MMDB.DataService.WindowsService.exe").
                                           WithStartMode(settings.ServiceStartMode);
                                        if(!string.IsNullOrEmpty(settings.ServiceDependencies))
                                        {
                                            var list = settings.ServiceDependencies.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                                            foreach(var item in list)
                                            {
                                                service = service.AddDependency(item);
                                            }
                                        }

									   if (settings.ServiceStartMode != ServiceStartMode.Disabled && settings.ServiceStartMode != ServiceStartMode.Manual)
									   {
										   s.WinService(serviceName).Start();
									   }
                                   });
            });
        }

        #endregion

        #region Properties

        //order is important
        //public static Role Db { get; set; }
        //public static Role Web { get; set; }
        //public static Role VirtualDirectory { get; set; }
        public static Role Host { get; set; }

        #endregion
	}
}