using System;

namespace CoreJob.CombineStatement.Models
{
	public class FlagKirimScheduler
	{
		public string Period_Name { get; set; }
		public FlagKirimStatus? FlagKirim { get; set; }
		public DateTime? CreatedTime { get; set; }
		public DateTime? UpdatedTime { get; set; }
		public string CreatedBy_Id { get; set; }
		public string UpdatedBy_Id { get; set; }
		public int Id { get; set; }
		public string Tahun_Period { get; set; }
		public string StatusMessage { get; set; }
	}

	public enum FlagKirimStatus
	{
		Pending = 0,
		OnProgress = 1,
		Done = 3,
		Error = 4
	}
}
