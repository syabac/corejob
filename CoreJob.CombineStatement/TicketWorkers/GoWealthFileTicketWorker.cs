using CoreJob.CombineStatement.Services;
using CoreJob.Library.Ticketing;

using System.Threading.Tasks;

namespace CoreJob.CombineStatement.TicketWorkers
{
	[TicketWorker("GoWealthFileTicketWorker")]
	public class GoWealthFileTicketWorker : ITicketWorker
	{
		public async Task<string> Run(TicketExecutionContext executionContext)
		{
			string period = null;
			executionContext.Parameters?.TryGetValue("PERIOD", out period);

			if (string.IsNullOrWhiteSpace(period))
			{
				return "PERIOD Parameter is required. Process stoppped.";
			}

			var service = new GoWealthService();

			await service.Run(period);

			return "Completed";
		}
	}
}
