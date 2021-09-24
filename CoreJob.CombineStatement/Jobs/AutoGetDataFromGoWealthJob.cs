using CoreJob.CombineStatement.Services;
using CoreJob.Library;

using Quartz;

using System;
using System.Threading.Tasks;

namespace CoreJob.CombineStatement.Jobs
{
	public class AutoGetDataFromGoWealthJob : Job
	{
		public override async Task Run(IJobExecutionContext ctx)
		{
			var service = new GoWealthService();
			var period = DateTime.Now.AddMonths(-1).ToString("MMM-yyyy");

			await service.Run(period);
		}
	}
}
