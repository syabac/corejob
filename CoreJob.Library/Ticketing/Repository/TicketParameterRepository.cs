using CoreJob.Library.Data;
using CoreJob.Library.Ticketing.Models;

using Dapper;

using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreJob.Library.Ticketing.Repository
{
	public class TicketParameterRepository
	{
		public IDictionary<string, string> GetParametersByTicketId(Guid ticketId)
		{
			using var db = ConnectionFactory.Open();

			return db.Query<TicketParameter>("SELECT * FROM CoreJob_TicketParameter WHERE TicketId = @TicketId", new { TicketId = ticketId })
				.ToDictionary(m => m.ParameterName, m => m.ParameterValue);
		}
	}
}
