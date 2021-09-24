using CoreJob.CombineStatement.Models;
using CoreJob.Library.Data;

using Dapper;

using System;
using System.Collections.Generic;

namespace CoreJob.CombineStatement.Repository
{
	public class RecipientRepository
	{
		public bool Exists(string dataType, string period, string cif)
		{
			using var db = ConnectionFactory.Open();
			var any = db.QueryFirstOrDefault<int>("SELECT TOP 1 1 FROM Tbl_Recipient2 WHERE Period_Name = @period AND CIF = @cif AND ListNameRecipient = @dataType",
				new
				{
					period,
					cif,
					dataType
				});

			return any == 1;
		}

		public void Insert(Recipient recipient)
		{
			using var db = ConnectionFactory.Open();
			db.Execute(@"INSERT INTO Tbl_Recipient2
						   ([Name] ,[Email] ,[CIF]
						   ,[CreatedTime]  ,[UpdatedTime] ,[CreatedBy_Id]
						   ,[UpdatedBy_Id] ,[CreatedBy_Name]  ,[UpdatedBy_Name]
						   ,[Id_ListRecipient]  ,[ListNameRecipient]  ,[Period_Name]
						   ,[PDFData]  ,[Status_Code]  ,[Status_Name]
						   ,[Keterangan]  ,[Email2]  ,[Email3]
						   ,[Timestamp]  ,[StatusSMTPId]  ,[StatusSMTP]
						   ,[UpdatedSMTPBy_Id] ,[UpdatedSMTPTime] ,[UpdatedSMTPBy_Name])
					 VALUES
						   (@Name ,@Email ,@CIF				
						   ,@CreatedTime  ,@UpdatedTime ,@CreatedBy_Id
							,@UpdatedBy_Id,@CreatedBy_Name,@UpdatedBy_Name	
						   ,@Id_ListRecipient,@ListNameRecipient,@Period_Name		
						   ,@PDFData,@Status_Code,@Status_Name		
						   ,@Keterangan,@Email2,@Email3			
						   ,@Timestamp,@StatusSMTPId,@StatusSMTP		
						   ,@UpdatedSMTPBy_Id,@UpdatedSMTPTime,@UpdatedSMTPBy_Name)", recipient);
		}

		public void Update(Recipient recipient)
		{

			using var db = ConnectionFactory.Open();
			db.Execute(@"UPDATE Tbl_Recipient2
						SET [Name]=@Name, 
							Email=@Email, 
							Email2=@Email2, 
							Email3=@Email3, 
							PDFData=@PDFData,
							UpdatedBy_Id = 0,
							UpdatedBy_Name = 'System',
							UpdatedTime = GETDATE()
						WHERE CIF = @CIF AND Period_Name = @Period_Name", recipient);
		}

		public Recipient FindById(int id)
		{
			using var db = ConnectionFactory.Open();
			return db.QueryFirstOrDefault<Recipient>("SELECT * FROM Tbl_Recipient2 WHERE Id = @id", new { id });
		}

		public IEnumerable<int> FindAllIdByReadyToSendWithFlags(string dataType, string period)
		{
			using var db = ConnectionFactory.Open();
			return db.Query<int>(@"SELECT Id FROM Tbl_Recipient2 
				WHERE Period_Name = @period 
				AND ListNameRecipient = @dataType
				AND Email IS NOT NULL 
				AND Email <> ''
				AND (Status_Code = @status OR Status_Code IS NULL)",
				new
				{
					period,
					dataType,
					status = ProcessStatus.Resend
				});
		}

		public void MarkRecipientsAsResend(string dataType, string period)
		{
			using var db = ConnectionFactory.Open();
			var sql = @"UPDATE Tbl_Recipient2 
						SET Status_Code = @newStatus, 
							Status_Name = 'Resend', 
							UpdatedBy_Id = 0,
							UpdatedBy_Name = 'System',
							UpdatedTime = GETDATE()
						WHERE Period_Name = @period 
						AND ListNameRecipient = @dataType
							AND Status_Code = @oldStatus";

			db.Execute(sql, new
			{
				newStatus = ProcessStatus.Resend,
				period,
				dataType,
				oldStatus = ProcessStatus.Processed
			});
		}

		public void SetStatus(int recId, int? statusCode, string statusName, string note)
		{
			using var db = ConnectionFactory.Open();
			var sql = @"UPDATE Tbl_Recipient2 
						SET Status_Code = @statusCode, 
							Status_Name = @statusName, 
							Keterangan = @note,
							UpdatedBy_Id = 0,
							UpdatedBy_Name = 'System',
							UpdatedTime = GETDATE()
						WHERE Id = @recId";
			db.Execute(sql, new
			{
				recId,
				statusCode,
				statusName,
				note
			});
		}

		public void SetSmtpResult(string period, string email, SmtpDeliveryStatus smtpStatusCode, string result, DateTime? timestamp)
		{
			using var db = ConnectionFactory.Open();

			var ids = db.Query<int>("SELECT Id FROM Tbl_Recipient2 WHERE Period_Name = @period AND Email = @email",
				new
				{
					period,
					email
				});

			var sql = @"UPDATE Tbl_Recipient2 
						SET StatusSMTPId = @smtpStatusCode, 
							StatusSMTP = @smtpStatus, 
							[Timestamp] = @timestamp ,
							UpdatedSMTPBy_Id = 0,
							UpdatedSMTPTime = getdate(),
							UpdatedSMTPBy_Name='System'
						WHERE Period_Name = @period 
							AND Id IN @ids";

			db.Execute(sql, new
			{
				smtpStatusCode,
				smtpStatus = result,
				period,
				ids,
				timestamp
			});

		}
	}
}
