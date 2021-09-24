using CoreJob.CombineStatement.Models;
using CoreJob.Library.Data;

using Dapper;

namespace CoreJob.CombineStatement.Repository
{
	public class FtpFileLogRepository
	{
		public FtpFileLog FindLatestLogByPeriod(string dataType, string period, string fileName)
		{
			using var db = ConnectionFactory.Open();

			return db.QueryFirstOrDefault<FtpFileLog>("SELECT TOP 1 * FROM CombineStatement_FtpFileLog WHERE Period = @period AND FileName = @fileName AND DataType = @dataType ORDER BY Id DESC ", new
			{
				fileName,
				period,
				dataType
			});
			
		}

		public void Insert(FtpFileLog data)
		{
			using var db = ConnectionFactory.Open();

			db.Execute("INSERT INTO CombineStatement_FtpFileLog(Period, FileName, CheckSum, LogDate, DataType) " +
				" VALUES(@Period, @FileName, @CheckSum, @LogDate, @DataType)", data);
		}
	}
}
