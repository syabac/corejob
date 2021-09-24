using CoreJob.Library;

using log4net;

using System.Threading.Tasks;

namespace CoreJob.CombineStatement.Services
{
	public class GoWealthService
	{
		private static ILog logger = LogManager.GetLogger(typeof(GoWealthService));

		public async Task Run(string period)
		{
			var options = Settings.Current.Options;

			options.TryGetValue("GoWealthFTP.Server", out var server);
			options.TryGetValue("GoWealthFTP.Port", out var port);
			options.TryGetValue("GoWealthFTP.User", out var user);
			options.TryGetValue("GoWealthFTP.Password", out var password);
			options.TryGetValue("GoWealthFTP.Directory", out var remoteDir);
			options.TryGetValue("GoWealthFTP.Passive", out var passiveMode);

			var ftpDownloader = new GoWealthFtpDownloader(new GoWealthFtpDownloaderConfig
			{
				PassiveMode = passiveMode is bool pm && pm,
				Password = password as string,
				Port = port is int ? (int)port : 21,
				RemoteDirectory = remoteDir as string,
				Server = server as string,
				User = user as string
			});

			var txtFile = await ftpDownloader.Download(period);

			if (txtFile == null)
			{
				logger.WarnFormat("Failed to download remote file for period = {0}", period);
				return;
			}

			var fileProcessor = new GoWealthFileProcessor(txtFile);

			fileProcessor.StartProcess();
		}
	}
}
