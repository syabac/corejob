using System;

namespace CoreJob.CombineStatement.Models
{
	public class MailLog
	{
		public int Id { get; set; }
		public string Status { get; set; }
		public string Message { get; set; }
		public DateTime? CreatedTime { get; set; }
		public int? IdRecipient { get; set; }
		public int? CreatedById { get; set; }
		public string CreatedByName { get; set; }
		public string Period { get; set; }
		public string Email { get; set; }
		public DateTime? Timestamp { get; set; }
		public int? StatusSMTPId { get; set; }
		public string StatusSMTP { get; set; }
		public int? UpdatedSMTPBy_Id { get; set; }
		public DateTime? UpdatedSMTPTime { get; set; }
		public string UpdatedSMTPBy_Name { get; set; }
		public string Status_Name { get; set; }
	}
}
