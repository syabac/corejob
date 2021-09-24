using CoreJob.Library.Data;
using CoreJob.Library.Ticketing.Models;

using Dapper;

using System;
using System.Collections.Generic;

namespace CoreJob.Library.Ticketing.Repository
{
	public class TicketRepository
	{
		public Ticket FindById(Guid? ticketId)
		{
			using var db = ConnectionFactory.Open();
			return db.QueryFirstOrDefault<Ticket>("SELECT * FROM CoreJob_Ticket WHERE Id = @ticketId", new { ticketId });
		}

		public Guid Create(string ticketName, IDictionary<string, string> parameters = null)
		{
			using var db = ConnectionFactory.Open();
			var id = Guid.NewGuid();
			var display = parameters == null ? null : string.Join(",", parameters.Values);

			db.Execute("INSERT INTO CoreJob_Ticket(Id, TicketName, Status, CreationDate, ParameterDisplay) VALUES(@id, @ticketName, @status, @date, @display)",
				new
				{
					id,
					ticketName,
					status = TicketStatus.Pending,
					date = DateTime.Now,
					display
				});

			if (parameters == null || parameters.Count == 0)
				return id;

			parameters.AsList().ForEach(kp =>
			{
				db.Execute("INSERT INTO CoreJob_TicketParameter(Id, TicketId, ParameterName, ParameterValue) VALUES(@Id, @TicketId, @ParameterName, @ParameterValue)",
					new
					{
						Id = Guid.NewGuid(),
						TicketId = id,
						ParameterName = kp.Key,
						ParameterValue = kp.Value
					});
			});

			return id;
		}

		public Ticket FindFirstPendingTicketByJob(string jobName)
		{
			using var db = ConnectionFactory.Open();

			return db.QueryFirstOrDefault<Ticket>("SELECT TOP 1 * FROM CoreJob_Ticket WHERE JobName = @JobName AND Status = @Status ORDER BY CreationDate DESC",
				new
				{
					JobName = jobName,
					Status = TicketStatus.Pending
				});
		}

		public Ticket FindFirstPendingTicket()
		{
			using var db = ConnectionFactory.Open();

			return db.QueryFirstOrDefault<Ticket>("SELECT TOP 1 * FROM CoreJob_Ticket WHERE Status = @Status ORDER BY CreationDate ASC",
				new
				{
					Status = TicketStatus.Pending
				});
		}

		public int SetTicketStatus(Guid ticketId, TicketStatus ticketStatus, string message = null)
		{
			using var db = ConnectionFactory.Open();
			var setDate = "";

			if (ticketStatus == TicketStatus.Started)
			{
				setDate = ", ExecutionDate = @date ";
			}
			else if (ticketStatus == TicketStatus.Completed || ticketStatus == TicketStatus.Error)
			{
				setDate = ", CompletionDate = @date ";
			}

			return db.Execute($"UPDATE CoreJob_Ticket SET Status = @ticketStatus, Message = @message {setDate} WHERE Id = @ticketId", new
			{
				ticketStatus,
				ticketId,
				message,
				date = DateTime.Now
			});
		}
	}
}
