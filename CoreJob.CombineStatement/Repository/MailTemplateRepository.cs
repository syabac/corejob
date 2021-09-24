using CoreJob.CombineStatement.Models;
using CoreJob.Library.Data;

using Dapper;

namespace CoreJob.CombineStatement.Repository
{
	public class MailTemplateRepository
	{
		public MailTemplate FindByType(string dataType)
		{
			using var db = ConnectionFactory.Open();

			return db.QueryFirstOrDefault<MailTemplate>("SELECT * FROM Tbl_EmailTemplate Where [Name] = @dataType AND IsActive = 1", new { dataType });
		}
	}
}
