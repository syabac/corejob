using System;

namespace CoreJob.Library.Ticketing.Models
{
	public class TicketParameter
	{
		public Guid? Id { get; set; }
		public Guid TicketId { get; set; }
		public string ParameterName { get; set; }
		public string ParameterValue { get; set; }
	}
}
