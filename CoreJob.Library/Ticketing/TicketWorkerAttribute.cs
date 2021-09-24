using System;

namespace CoreJob.Library.Ticketing
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class TicketWorkerAttribute : Attribute
	{
		public string TicketName { get; set; }

		public TicketWorkerAttribute(string ticketName)
		{
			TicketName = ticketName;
		}
	}
}
