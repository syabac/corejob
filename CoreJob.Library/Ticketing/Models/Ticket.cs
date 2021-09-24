using System;

namespace CoreJob.Library.Ticketing.Models
{
	public class Ticket
	{
		public Guid? Id { get; set; }
		public string TicketName { get; set; }
		public TicketStatus? Status { get; set; }
		public DateTime? CreationDate { get; set; }
		public DateTime? ExecutionDate { get; set; }
		public DateTime? CompletionDate { get; set; }
		public string Message { get; set; }
	}

	public enum TicketStatus
	{
		Pending = 0,
		Started = 1,
		Completed = 2,
		Error = 3,
		Canceled = 4
	}
}
