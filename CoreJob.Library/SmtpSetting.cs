namespace CoreJob.Library
{
	public class SmtpSetting
	{
		public string Server { get; set; }
		public int Port { get; set; } = 25;
		public string User { get; set; }
		public string Password { get; set; }
		public bool UseSsl { get; set; }
		public int Timeout { get; set; } = 15;
	}
}
