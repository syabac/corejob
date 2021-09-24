using CoreJob.CombineStatement.Models;
using CoreJob.Library.Data;

using Dapper;

using System;

namespace CoreJob.CombineStatement.Repository
{
	public class MailLogRepository
	{
		public void Insert(MailLog log)
		{
			var sql = @"INSERT INTO Tbl_Log_Email2
                           ([Status],[Message],[CreatedTime]
                           ,[IdRecipient],[CreatedById],[CreatedByName]
                           ,[Period],[Email],[Timestamp]
                           ,[StatusSMTPId],[StatusSMTP],[UpdatedSMTPBy_Id]
                           ,[UpdatedSMTPTime],[UpdatedSMTPBy_Name],[Status_Name])
                     VALUES
                           (@Status,@Message,@CreatedTime
                           ,@IdRecipient,@CreatedById,@CreatedByName
                           ,@Period,@Email,@Timestamp
                           ,@StatusSMTPId,@StatusSMTP,@UpdatedSMTPBy_Id
                           ,@UpdatedSMTPTime,@UpdatedSMTPBy_Name,@Status_Name)";
			using var db = ConnectionFactory.Open();

			db.Execute(sql, log);

		}


		public void SetStatus(string period, string email, SmtpDeliveryStatus statusCode, string result, DateTime? timestamp)
		{
			var sql = @"UPDATE Tbl_Log_Email2
						SET StatusSMTPId = @statusCode,
							StatusSMTP = @result,
							[Timestamp] = @timestamp ,
							UpdatedSMTPBy_Id = 0,
							UpdatedSMTPTime = getdate(),
							UpdatedSMTPBy_Name='System'
						WHERE Period = @period 
							AND Email = @email
							AND StatusSMTPId IS NULL";
			using var db = ConnectionFactory.Open();

			db.Execute(sql, new
			{
				period,
				email,
				statusCode,
				result,
				timestamp
			});
		}
	}
}
