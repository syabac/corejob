namespace CoreJob.CombineStatement.Models
{
	public enum SmtpDeliveryStatus
	{
		Delivered = 1,
		Bounced = 2,
		Deferred = 3,
		Quarantined = 4,
		Failed = 5,
		All = 6
	}
}
