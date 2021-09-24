using CoreJob.CombineStatement.Models;
using CoreJob.Library.Data;

using Dapper;

namespace CoreJob.CombineStatement.Repository
{
	public class FlagKirimRepository
	{
		public FlagKirimScheduler FindPending()
		{
			using var db = ConnectionFactory.Open();
			return db.QueryFirstOrDefault<FlagKirimScheduler>("SELECT TOP 1 p.* FROM Tbl_FlagKirimScheduler p WHERE FlagKirim = @flag ORDER BY Id",
				new { flag = FlagKirimStatus.Pending });
		}

		public void SetStatus(int id, FlagKirimStatus status, string message)
		{
			using var db = ConnectionFactory.Open();
			var sql = @"UPDATE Tbl_FlagKirimScheduler 
						SET FlagKirim = @status,
						StatusMessage = @message,
						UpdatedBy_Id = 0,
						UpdatedTime = GETDATE()
						WHERE Id = @id";

			db.Execute(sql, new
			{
				id,
				status,
				message
			});
		}
	}
}
