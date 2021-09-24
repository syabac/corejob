using CoreJob.Library.Ticketing.Models;
using CoreJob.Library.Ticketing.Repository;

using log4net;

using Quartz;

using System;
using System.Threading.Tasks;

namespace CoreJob.Library.Ticketing
{
	public class TicketWorkerJob : IJob
	{
		protected ILog logger = LogManager.GetLogger(typeof(TicketWorkerJob));
		private TicketRepository ticketRepository = new();
		private TicketParameterRepository parameterRepo = new();

		public async Task Execute(IJobExecutionContext context)
		{
			var ticket = ticketRepository.FindFirstPendingTicket();

			if (ticket == null || ticket.Id == null)
			{
				logger.Debug("No Ticket avaiable waiting for execution");
				return;
			}

			try
			{
				logger.InfoFormat("Starting Ticket {0} with ID {1}", ticket.TicketName, ticket.Id, "Started");
				ticketRepository.SetTicketStatus((Guid)ticket.Id, TicketStatus.Started, "Started");

				var worker = TicketWorkerRegistry.Instance.GetWorker(ticket.TicketName);
				var message = string.Empty;

				if (worker == null)
				{
					logger.Warn($"Could not get TicketWorker '{ticket.TicketName}' instance from registry.");
					message = $"Could not get TicketWorker '{ticket.TicketName}' instance from registry.";
				}
				else
				{
					message = await worker.Run(new TicketExecutionContext
					{
						JobExecutionContext = context,
						Ticket = ticket,
						Parameters = parameterRepo.GetParametersByTicketId((Guid)ticket.Id)
					});
				}

				ticketRepository.SetTicketStatus((Guid)ticket.Id, TicketStatus.Completed, message);
				logger.InfoFormat("Completed Ticket {0} with ID {1}", ticket.TicketName, ticket.Id);
			}
			catch (Exception ex)
			{
				ticketRepository.SetTicketStatus((Guid)ticket.Id, TicketStatus.Error, "ERROR : " + ex.Message);
				logger.Error(string.Format("Completed with Error Ticket {0} with ID {1}", ticket.TicketName, ticket.Id), ex);
			}
		}
	}
}
