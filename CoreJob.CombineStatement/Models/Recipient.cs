using System;

namespace CoreJob.CombineStatement.Models
{
	public class Recipient
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string CIF { get; set; }
		public DateTime? CreatedTime { get; set; }
		public DateTime? UpdatedTime { get; set; }
		public int? CreatedBy_Id { get; set; }
		public int? UpdatedBy_Id { get; set; }
		public string CreatedBy_Name { get; set; }
		public string UpdatedBy_Name { get; set; }
		public int? Id_ListRecipient { get; set; }
		public string ListNameRecipient { get; set; }
		public string Period_Name { get; set; }
		public string PDFData { get; set; }
		public int? Status_Code { get; set; }
		public string Status_Name { get; set; }
		public string Keterangan { get; set; }
		public string Email2 { get; set; }
		public string Email3 { get; set; }
		public DateTime? Timestamp { get; set; }
		public SmtpDeliveryStatus? StatusSMTPId { get; set; }
		public string StatusSMTP { get; set; }
		public int? UpdatedSMTPBy_Id { get; set; }
		public DateTime? UpdatedSMTPTime { get; set; }
		public string UpdatedSMTPBy_Name { get; set; }
	}

	public enum ProcessStatus
	{
		Processed = 101, 
		Deferred = 102,
		Failed = 103,
		Resend = 300
	}
}
