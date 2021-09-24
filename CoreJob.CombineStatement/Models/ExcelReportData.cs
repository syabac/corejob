using System;

namespace CoreJob.CombineStatement.Models
{
	public class ExcelReportData
	{
		public string CIF { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string StatusName { get; set; }
		public string Keterangan { get; set; }
		public string StatusSMTP { get; set; }
		public DateTime? Timestamp { get; set; }
		public string StatusSMTPName { get; set; }
	}
}
