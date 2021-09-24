using CoreJob.CombineStatement.Services;
using CoreJob.Library;

using Quartz;

using System.Threading.Tasks;

namespace CoreJob.CombineStatement.Jobs
{
	public class LoadSmtpReportFileJob : Job
	{
		private SmtpReportFileProcessor smtpReportFileProcessor = new();

		public override Task Run(IJobExecutionContext ctx)
		{
			smtpReportFileProcessor.ScanAndLoadFiles();
			return Task.CompletedTask;
		}
	}
}
