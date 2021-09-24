using CoreJob.Library.Ticketing.Models;

using Quartz;

using System.Collections.Generic;

namespace CoreJob.Library.Ticketing
{
	public class TicketExecutionContext
	{
		public Ticket Ticket { get; set; }
		public IDictionary<string, string> Parameters { get; set; }
		public IJobExecutionContext JobExecutionContext { get; set; }
	}
}
