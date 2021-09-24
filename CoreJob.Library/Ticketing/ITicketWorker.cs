using System.Threading.Tasks;

namespace CoreJob.Library.Ticketing
{
	public interface ITicketWorker
	{
		/// <summary>
		/// Run ticketworker
		/// </summary>
		/// <param name="executionContext"></param>
		/// <returns>Process result message of this ticket worker</returns>
		public Task<string> Run(TicketExecutionContext executionContext);
	}
}
