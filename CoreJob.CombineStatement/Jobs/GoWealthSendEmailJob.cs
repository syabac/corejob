using CoreJob.CombineStatement.Models;
using CoreJob.CombineStatement.Repository;
using CoreJob.CombineStatement.Services;
using CoreJob.Library;

using log4net;

using Quartz;

using System;
using System.Threading.Tasks;

namespace CoreJob.CombineStatement.Jobs
{
	public class GoWealthSendEmailJob : Job
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof(GoWealthSendEmailJob));

		private SendEmailService sendEmailService = new();
		private FlagKirimRepository flagKirimRepository = new();
		public override Task Run(IJobExecutionContext ctx)
		{
			ctx.JobDetail.JobDataMap.TryGetValue("DataType", out object dataType);

			if (dataType == null)
				return Task.CompletedTask;

			var pending = flagKirimRepository.FindPending();

			if (pending == null)
			{
				logger.Debug("No Email Pending Process. Job stopped.");
				return Task.CompletedTask;
			}

			flagKirimRepository.SetStatus(pending.Id, FlagKirimStatus.OnProgress, "Sending mail process started.");

			var period = pending.Period_Name + "-" + pending.Tahun_Period;
			try
			{
				sendEmailService.SendAll(dataType.ToString(), period);
			}
			catch (Exception ex)
			{
				flagKirimRepository.SetStatus(pending.Id, FlagKirimStatus.Error, $"ProcessId {pending.Id} : Error occured with message : \r\n {ex.Message}");
				logger.Error($"ProcessId {pending.Id}", ex);
			}
			return Task.CompletedTask;
		}
	}
}
