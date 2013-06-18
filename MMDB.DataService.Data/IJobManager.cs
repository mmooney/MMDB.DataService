using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.DataService.Data.Dto.Jobs;

namespace MMDB.DataService.Data
{
	public interface IJobManager
	{
		List<JobDefinition> LoadJobList();
		JobDefinition GetJobDefinition(int id);
		JobDefinition LoadJobDefinition(int id);
		void DeleteJobDefinition(int id);
		void UpdateJobDataStatus(JobData item, EnumJobStatus status);
		void DeleteJobData(JobData item);

		void ImportJobs(List<JobDefinition> jobDefinitionList);

		Type GetJobConfigurationType(Type jobType);

		List<JobStatus> GetAllJobStatus();

		JobDefinition CreateSimpleJob(string jobName, Guid guid, string assemblyName, string className, int intervalMinutes, int delayStartMinutes);
		JobDefinition CreateCronJob(string jobName, Guid guid, string assemblyName, string className, string cronScheduleExpression);
		void UpdateSimpleJob(int id, string jobName, Guid guid, string assemblyName, string className, int intervalMinutes, int delayStartMinutes, Jobs.JobConfigurationBase jobConfigurationBase);
		void UpdateCronJob(int id, string jobName, Guid guid, string assemblyName, string className, string cronScheduleExpression, Jobs.JobConfigurationBase jobConfigurationBase);

		void EnsureConfiguration(JobDefinition jobDefinition);

		IQueryable<JobData> GetJobDataQueue(JobDefinition jobDefinition);
		IQueryable<JobData> GetJobDataQueue(int jobDefinitionId);
	}
}
