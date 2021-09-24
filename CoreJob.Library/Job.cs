using log4net;

using Quartz;

using System;
using System.Threading.Tasks;

namespace CoreJob.Library
{
	public abstract class Job : IJob
	{
		private static ILog logger = LogManager.GetLogger(typeof(Job));


		public virtual async Task Execute(IJobExecutionContext context)
		{
			var jobName = context.JobDetail.Key.Name;
			var instanceId = context.FireInstanceId;

			logger.DebugFormat("Job Start = {0} InstanceID = {1}", jobName, instanceId);

			try
			{
				await Run(context);
			}
			catch (Exception ex)
			{
				logger.Error(string.Format("Job Error = {0} InstanceID = {1}", jobName, instanceId), ex);
			}

			logger.DebugFormat("Job Completed = {0} InstanceID = {1}", jobName, instanceId);
		}

		public abstract Task Run(IJobExecutionContext ctx);
	}
}
