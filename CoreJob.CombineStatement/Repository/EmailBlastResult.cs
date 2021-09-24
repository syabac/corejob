namespace CoreJob.CombineStatement.Repository
{
	public class EmailBlastResult
	{
		public int TotalSent { get; set; }
		public int TotalError { get; set; }
		public int Total { get { return TotalError + TotalSent; } }
		public string Message { get; set; }
	}
}
