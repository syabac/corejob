using System;
using System.IO;
using System.Security.Cryptography;

namespace CoreJob.Library.Utils
{
	public class CalculateFileHash
	{
		/// <summary>
		/// Calculate MD5 checksum of file
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string CalculateMD5(string filename)
		{
			using (var md5 = MD5.Create())
			{
				using (var stream = File.OpenRead(filename))
				{
					var hash = md5.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
				}
			}
		}
	}
}
