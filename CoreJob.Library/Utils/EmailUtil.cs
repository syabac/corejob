using log4net;

using System;
using System.Net;
using System.Net.Mail;

namespace CoreJob.Library.Utils
{
	public class EmailUtil
	{
		private static ILog logger = LogManager.GetLogger(typeof(EmailUtil));
		public static async void Send(MailMessage mail)
		{
			var opts = Settings.Current.SmtpSetting;
			var smtp = new SmtpClient
			{
				EnableSsl = opts.UseSsl,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				Host = opts.Server,
				Port = opts.Port,
				Timeout = opts.Timeout
			};

			if (!string.IsNullOrWhiteSpace(opts.User))
			{
				smtp.Credentials = new NetworkCredential(opts.User, opts.Password);
			}

			try
			{
				await smtp.SendMailAsync(mail);
			}
			catch (Exception ex)
			{
				logger.Error($"Failed to support Exception", ex);
			}
		}
	}
}
