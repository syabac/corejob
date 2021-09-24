using System;

namespace CoreJob.CombineStatement.Models
{
	public class FtpFileLog
	{
		public int Id { get; set; }
		public string Period { get; set; }
		public string FileName { get; set; }
		public string CheckSum { get; set; }
		public DateTime? LogDate { get; set; }
		public string DataType { get; set; }
	}
}
