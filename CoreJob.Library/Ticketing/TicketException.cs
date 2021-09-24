using System;
using System.Runtime.Serialization;

namespace CoreJob.Library.Ticketing
{
	public class TicketException : Exception
	{
		public TicketException() : base()
		{
		}

		public TicketException(string message) : base(message)
		{
		}

		public TicketException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected TicketException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
