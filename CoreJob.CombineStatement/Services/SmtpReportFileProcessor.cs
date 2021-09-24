using CoreJob.CombineStatement.Models;
using CoreJob.CombineStatement.Repository;
using CoreJob.Library;

using log4net;

using LumenWorks.Framework.IO.Csv;

using System;
using System.Globalization;
using System.IO;

namespace CoreJob.CombineStatement.Services
{
	public class SmtpReportFileProcessor
	{
		private static readonly ILog logger = LogManager.GetLogger(typeof(SmtpReportFileProcessor));
		private const string OptionKey = "SmtpReportDirectory";
		private const string PrefixFile = "emerald_log_";

		private RecipientRepository recipientRepository = new();
		private ReportRepository reportRepository = new();
		private MailLogRepository mailLogRepository = new();

		public void ScanAndLoadFiles()
		{
			if (!Settings.Current.Options.ContainsKey(OptionKey))
				return;

			var dir = Settings.Current.Options[OptionKey].ToString();

			foreach (var csvFile in Directory.GetFiles(dir, PrefixFile + "*.csv"))
			{
				var procFile = csvFile + ".process";
				logger.Info($"Preparing process, rename file {csvFile} to {procFile}");

				try
				{
					File.Move(csvFile, procFile, true);

					var period = ProcessFile(procFile);

					var doneFile = csvFile + ".done-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

					logger.Info($"Mark file {procFile} as done {doneFile}");

					File.Move(procFile, doneFile, true);

					var path = Path.Combine(dir, $"Report_LogPengiriman_Email_{period}_2.xlsx");
					logger.Info($"Start generate excel report file {path}");

					if (File.Exists(path))
						File.Delete(path);

					using var fileStream = new FileStream(path, FileMode.CreateNew);

					reportRepository.ExportToExcel(period, fileStream);

					logger.Info($"Excel generated successfully {path}");
				}
				catch (Exception ex)
				{
					logger.Error($"Error while processing SMTP Report file {csvFile}", ex);

					var errorFile = csvFile + ".error-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

					File.Move(procFile, errorFile, true);
				}
			}
		}

		private string ProcessFile(string procFile)
		{
			var info = new FileInfo(procFile);
			var period = info.Name.Substring(PrefixFile.Length, 8);

			// sample contents
			// Timestamp,Sender,Recipient(s),Subject,Last Policy Action
			// 8/10/2021 19:25,bni_emerald@bni.co.id,NOVAYULIANTI0@GMAIL.COM,Laporan Konsolidasi Nasabah Jul-2021,Passed IMSVA scan and deliver

			// date format: M/d/yyyy H:m

			using var csvReader = new CsvReader(new StreamReader(procFile), true);
			var countProcessed = 0;

			while (csvReader.ReadNextRecord())
			{
				var timestamp = csvReader[0];
				var sender = csvReader[1];
				var recipient = csvReader[2];
				var subject = csvReader[3];
				var result = csvReader[4];
				var date = new DateTime(1990, 1, 1);
				var smtpStatusCode = ParseSmtpStatus(result);

				if(!DateTime.TryParseExact(timestamp, "M/d/yyyy H:m", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
				{
					logger.Warn($"Invalid date format in SMTP log report '{timestamp}'. Valid date format is M/d/yyyy H:m.");
					date = new DateTime(1990, 1, 1);
				}

				recipientRepository.SetSmtpResult(period, recipient, smtpStatusCode, result, date);
				mailLogRepository.SetStatus(period, recipient, smtpStatusCode, result, date);
				countProcessed++;

				logger.Info($"Update SMTP Status done for {recipient}");
			}

			logger.Info($"SMTP Report file processed data = {countProcessed} records");

			CalculateSummary(period);

			return period;
		}

		private SmtpDeliveryStatus ParseSmtpStatus(string result)
		{
			result = result + string.Empty;
			var smtpStatusCode = SmtpDeliveryStatus.Delivered; // default set to delivered

			if (result.EndsWith("Bounced", StringComparison.InvariantCultureIgnoreCase))
			{
				smtpStatusCode = SmtpDeliveryStatus.Bounced;
			}
			else if (result.EndsWith("Deferred", StringComparison.InvariantCultureIgnoreCase))
			{
				smtpStatusCode = SmtpDeliveryStatus.Deferred;
			}
			else if (result.EndsWith("Quarantined", StringComparison.InvariantCultureIgnoreCase))
			{
				smtpStatusCode = SmtpDeliveryStatus.Quarantined;
			}

			return smtpStatusCode;
		}

		private void CalculateSummary(string period)
		{
			var statusValues = (SmtpDeliveryStatus[])Enum.GetValues(typeof(SmtpDeliveryStatus));

			foreach (var status in statusValues)
			{
				try
				{
					var count = reportRepository.CountBySmtpStatus(period, status);
					reportRepository.AddSummary(period, status, count);
				}
				catch (Exception ex)
				{
					logger.Error($"Error occured when create summary for {period} and SmtpDeliveryStatus {status}", ex);
				}
			}
		}
	}
}
