using CoreJob.CombineStatement.Repository;
using CoreJob.CombineStatement.Services;
using CoreJob.Library.Ticketing;

using log4net;

using System.Threading.Tasks;

namespace CoreJob.CombineStatement.TicketWorkers
{
	[TicketWorker("SendEmailTicketWorker")]
	public class SendEmailTicketWorker : ITicketWorker
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof(SendEmailTicketWorker));
		public async Task<string> Run(TicketExecutionContext executionContext)
		{
			string period = null;
			string recpId = null;
			executionContext.Parameters?.TryGetValue("PERIOD", out period);
			executionContext.Parameters?.TryGetValue("RECIPIENT_GROUP", out recpId);

			if (string.IsNullOrWhiteSpace(period) || string.IsNullOrWhiteSpace(recpId))
			{
				logger.Warn($"SendEmailTicketWorker mandatory parameters (PERIOD & RECIPIENT_GROUP) are not valid. Ticket stopped.");

				return "Mandatory parameters (PERIOD & RECIPIENT_GROUP) are not valid. Ticket stopped.";
			}

			var emailService = new SendEmailService();
			var recpRepository = new RecipientRepository();

			recpRepository.MarkRecipientsAsResend(recpId, period);
			
			var result = await emailService.SendAll(recpId, period);

			return string.Format("Completed. Total Sent {0} records. Total Error {1} records. {2}", result.TotalSent, result.TotalError, result.Message);
		}
	}
}
