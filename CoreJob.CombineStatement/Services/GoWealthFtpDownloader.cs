using CoreJob.CombineStatement.Models;
using CoreJob.CombineStatement.Repository;
using CoreJob.Library;
using CoreJob.Library.Utils;

using FluentFTP;

using log4net;

using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CoreJob.CombineStatement.Services
{
	public class GoWealthFtpDownloader
	{
		const string DATA_TYPE = "GOWEALTH";
		private static ILog logger = LogManager.GetLogger(typeof(GoWealthFtpDownloader));
		private GoWealthFtpDownloaderConfig config;

		public GoWealthFtpDownloader(GoWealthFtpDownloaderConfig config)
		{
			this.config = config;
		}

		public async Task<FileInfo> Download(string period)
		{
			logger.InfoFormat("Preparing connection to FTP server {0}:{1} with credential {2}:{3}", config.Server, config.Port, config.User, config.Password);

			if (string.IsNullOrEmpty(period))
			{
				logger.Info("Period parameter is null or empty.");
				return null;
			}

			var logRepository = new FtpFileLogRepository();
			using var client = new FtpClient(config.Server, config.Port, config.User, config.Password);

			logger.InfoFormat("Trying to connect to remote FTP server {0}:{1}", config.Server, config.Port);

			await client.ConnectAsync();

			if (config.PassiveMode)
			{
				client.DataConnectionType = FtpDataConnectionType.PASV;
			}

			client.DownloadDataType = FtpDataType.Binary;

			await client.SetWorkingDirectoryAsync(config.RemoteDirectory);

			logger.InfoFormat("Change FTP current directory to {0}", config.RemoteDirectory);

			var zipFile = period + ".zip";
			var localZipFile = period + ".zip"; // need to change

			logger.InfoFormat("Check file {0} exists on remote server", zipFile);

			if (!await client.FileExistsAsync(zipFile))
			{
				logger.InfoFormat("Zip file '{0} is not available on remote server. Please check if remote file is exists. Process stopped.", zipFile);
				return null;
			}

			logger.InfoFormat("Start download file {0} from remote server", zipFile);
			var downloadStatus = await client.DownloadFileAsync(localZipFile, zipFile, FtpLocalExists.Overwrite);

			if (!File.Exists(localZipFile))
			{
				logger.InfoFormat("Unable to download {0} from remote server. Download status {1}. Process stopped.", zipFile, downloadStatus);
				return null;
			}

			var logProcess = logRepository.FindLatestLogByPeriod(DATA_TYPE, period, zipFile);
			var localHash = CalculateFileHash.CalculateMD5(localZipFile);

			if (logProcess != null)
			{
				var hasProcessed = string.Equals(logProcess.CheckSum, localHash, StringComparison.CurrentCultureIgnoreCase);

				if (hasProcessed)
				{
					logger.InfoFormat("File is already processed before at {0} with LogId = {1}. Process stopped.", logProcess.LogDate, logProcess.Id);
					return null;
				}
			}

			var newLogProcess = new FtpFileLog
			{
				CheckSum = localHash,
				FileName = localZipFile,
				LogDate = DateTime.Now,
				Period = period,
				DataType = DATA_TYPE
			};

			logRepository.Insert(newLogProcess);
			logger.InfoFormat("Starting to process file {0} with checksum {1}", localZipFile, localHash);

			var extractDir = Settings.Current.DataDirectory;

			try
			{
				ZipFile.ExtractToDirectory(localZipFile, extractDir, true);

				logger.InfoFormat("Zip file extracted. Deleting the original Zip file {0}", localZipFile);
				File.Delete(localZipFile);
			}
			catch (Exception ex)
			{
				logger.Error($"Error occured when extracting zip file {localZipFile}", ex);
				return null;
			}

			var monthDir = Path.Combine(extractDir, period);
			var txtFile = Path.Combine(monthDir, period + ".txt");

			if (!Directory.Exists(monthDir))
			{
				logger.InfoFormat("Unable to find folder {0} from extracted files.", monthDir);
				return null;
			}

			if (!File.Exists(txtFile))
			{
				logger.InfoFormat("Unable to find file {0} from extracted zip files. Process stopped.", txtFile);
				return null;
			}

			return new FileInfo(txtFile);
		}
	}

	public class GoWealthFtpDownloaderConfig
	{
		public string Server { get; set; }
		public int Port { get; set; } = 21;
		public string User { get; set; }
		public string Password { get; set; }
		public string RemoteDirectory { get; set; }
		public bool PassiveMode { get; set; }

	}
}
