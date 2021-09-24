using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreJob.App.Models
{
	public class TicketRequest
	{
		[Required]
		public string TicketName { get; set; }
		public IDictionary<string, string> Parameters { get; set; }
	}
}
