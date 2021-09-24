using Microsoft.AspNetCore.Mvc;

using Quartz;
using Quartz.Impl.Matchers;

using System.Text;
using System.Threading.Tasks;

namespace CoreJob.App.Controllers
{
	[Route("scheduler")]
	[ApiController]
	public class SchedulerController : ControllerBase
	{
		private IScheduler scheduler;

		public SchedulerController(IScheduler scheduler)
		{
			this.scheduler = scheduler;
		}

		[Route("jobs")]
		public async Task<string> Jobs()
		{
			var sb = new StringBuilder();

			foreach (var jobKey in await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()))
			{
				var job = await scheduler.GetJobDetail(jobKey);
				sb.Append(jobKey.Group).Append(":").Append(jobKey.Name).AppendLine();
			}

			return sb.ToString();
		}

		[Route("pause")]
		public async Task<string> Pause()
		{
			await scheduler.PauseAll();
			return "Paused";
		}

		[Route("resume")]
		public async Task<string> Resume()
		{
			await scheduler.ResumeAll();
			return "Resumed";
		}

		[Route("status")]
		public string Status()
		{
			return "UP";
		}
	}
}
