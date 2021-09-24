
using Microsoft.Data.SqlClient;

using System.Data;

namespace CoreJob.Library.Data
{
	/// <summary>
	/// Open Connection to database
	/// </summary>
	public sealed class ConnectionFactory
	{
		/// <summary>
		/// Open new connection to DB, 
		/// it's the caller responsibility to dispose DB object when it's not longer needed
		/// </summary>
		/// <returns></returns>
		public static IDbConnection Open()
		{
			var db = new SqlConnection(Settings.Current.ConnectionString);
			db.Open();

			return db;
		}
	}
}
