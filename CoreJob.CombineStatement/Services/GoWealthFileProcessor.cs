using CoreJob.CombineStatement.Models;
using CoreJob.CombineStatement.Repository;
using CoreJob.Library;

using log4net;

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace CoreJob.CombineStatement.Services
{
	public class GoWealthFileProcessor
	{
		private static ILog logger = LogManager.GetLogger(typeof(GoWealthFileProcessor));
		private FileInfo file;

		public GoWealthFileProcessor(FileInfo fileName)
		{
			this.file = fileName;
		}

		public void StartProcess()
		{
			if (!file.Exists)
			{
				logger.ErrorFormat("Unabled to find file {0} to read. Process stopped.", file.FullName);
				return;
			}

			using var fileHandler = file.OpenText();
			var line = string.Empty;
			var lineCount = 0;
			var invalidLines = new StringBuilder();
			var repo = new RecipientRepository();
			var baseDir = file.Directory;
			var timer = Stopwatch.StartNew();
			var dataType = "EMERALD";

			while ((line = fileHandler.ReadLine()) != null)
			{
				var lineNumber = lineCount++;

				if (lineNumber == 0 || string.IsNullOrWhiteSpace(line)) // skip first line, it's header. Or empty line
				{
					continue;
				}

				// line is pipe-separator, CIF|NAMA|EMAIL|PERIOD
				var tokens = line.Split('|', StringSplitOptions.TrimEntries);

				if (tokens.Length < 4)
				{
					invalidLines.Append("LINE:").Append(lineNumber).Append("|").AppendLine(line);
					continue;
				}

				var cif = tokens[0];
				var customer = tokens[1];
				var email = tokens[2];
				var period = tokens[3];
				var pdfFile = Path.Combine(baseDir?.FullName, string.Format("{0}-{1}.pdf", cif, period?.ToUpper().Replace("-", "")));
				int? statusCode = null;
				string statusName = null;
				string note = null;

				if (string.IsNullOrWhiteSpace(email))
				{
					statusCode = 102;
					statusName = "Deferred";
					note = "Email address is empty";
				}

				if(!MailAddress.TryCreate(email, out MailAddress _))
				{
					statusCode = 102;
					statusName = "Deferred";
					note = "Email address is not in valid format";
				}

				// if PDF file is not included in zip file, then make it NULL
				if (string.IsNullOrWhiteSpace(pdfFile) || !File.Exists(pdfFile))
				{
					pdfFile = null;
					statusCode = 103;
					statusName = "Failed";
					note = "PDF Not Found";
				}
				
				try
				{
					var recp = new Recipient
					{
						CIF = cif,
						CreatedBy_Id = 0,
						CreatedBy_Name = "System",
						CreatedTime = DateTime.Now,
						Email = string.IsNullOrWhiteSpace(email) ? null : email,
						Name = customer,
						Id_ListRecipient = 34,
						ListNameRecipient = dataType,
						Period_Name = period,
						Status_Code = statusCode,
						Status_Name = statusName,
						PDFData = pdfFile,
						Keterangan = note
					};

					if (repo.Exists(dataType, period, cif))
					{
						repo.Update(recp);
						logger.InfoFormat("Update existing record. CIF {0} and Period {1}", cif, period);
					}
					else
					{
						repo.Insert(recp);
						logger.InfoFormat("Insert new record to database. CIF {0} and Period {1}", cif, period);
					}
				}
				catch (Exception ex)
				{
					logger.Error(string.Format("Failed to insert to database.  CIF {0} and Period {1}", cif, period), ex);
					invalidLines.Append("LINE:").Append(lineNumber).Append("|").Append(line).Append("|").AppendLine(ex.Message);
				}

				if (invalidLines.Length > 0)
				{
					logger.ErrorFormat("GoWealth Error Records : \r\n {0}", invalidLines);

					var errorFile = Path.Combine(Settings.Current.DataDirectory, "error-load-" + DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss") + ".error");

					File.WriteAllTextAsync(errorFile, invalidLines.ToString());
				}
			}

			timer.Stop();

			logger.InfoFormat("Process load data completed in {0}, Total records processed {1} rows.", timer.Elapsed, lineCount);
		}
	}
}
