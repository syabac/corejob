using CoreJob.CombineStatement.Models;
using CoreJob.CombineStatement.Repository;
using CoreJob.Library.Utils;

using log4net;

using System;
using System.IO;
using System.Net.Mail;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;

namespace CoreJob.CombineStatement.Services
{
	public class SendEmailService
	{
		private static ILog logger = LogManager.GetLogger(typeof(SendEmailService));

		private RecipientRepository recipientRepository = new();
		private MailTemplateRepository templateRepository = new();
		private MailLogRepository mailLogRepository = new();

		public Task<EmailBlastResult> SendAll(string dataType, string period)
		{
			var template = templateRepository.FindByType(dataType);

			if (string.IsNullOrWhiteSpace(template?.Body))
			{
				logger.Warn($"Could not find Mail Template with name = ${dataType}. Process stopped.");
				return Task.FromResult(new EmailBlastResult
				{
					Message = $"Could not find Mail Template with name = ${dataType}. Process stopped."
				});
			}

			var result = new EmailBlastResult();
			var query = recipientRepository.FindAllIdByReadyToSendWithFlags(dataType, period)
							.AsParallel()
							.WithDegreeOfParallelism(4);

			foreach (var recpId in query)
			{
				try
				{
					SendRecipient(recpId, template);
					result.TotalSent++;
				}
				catch (Exception ex)
				{
					logger.Error($"Exception occured when sending email to RecipientId = {recpId}", ex);
					result.TotalError++;
				}
			}

			return Task.FromResult(result); ;
		}

		private void SendRecipient(int recpId, MailTemplate template)
		{
			var recipient = recipientRepository.FindById(recpId);

			logger.InfoFormat("Preparing mail content for {0} {1}", recipient.Name, recipient.Email);

			if (string.IsNullOrWhiteSpace(recipient.PDFData) || !File.Exists(recipient.PDFData))
			{
				logger.InfoFormat("PDF File is not found for CIF = {0} - {1}", recipient.CIF, recipient.Name);
				recipientRepository.SetStatus(recpId, 103, "Failed", "PDF Not Found");
				return;
			}

			SendMailMessage(recipient, template);

			logger.InfoFormat("Mail sent to SMTP. For {0} {1}", recipient.Name, recipient.Email);
			mailLogRepository.Insert(new MailLog
			{
				CreatedById = 0,
				CreatedByName = "System",
				CreatedTime = DateTime.Now,
				Email = recipient.Email,
				IdRecipient = recipient.Id,
				Message = "Deliver",
				Period = recipient.Period_Name,
				Status = "101",
				Status_Name = "Deliver"
			});

			recipientRepository.SetStatus(recpId, 101, "Deliver", "Deliver");
		}

		private void SendMailMessage(Recipient recipient, MailTemplate template)
		{
			var values = ObjectToDictionary(recipient);

			// for backward compatibility
			values.Add("nama", recipient.Name);
			values.Add("periode", recipient.Period_Name);

			var mail = new MailMessage
			{
				Subject = TemplateUtil.Parse(template.Subject, values),
				Body = TemplateUtil.Parse(template.Body, values),
				IsBodyHtml = true,
				Sender = new MailAddress(template.Email_Sender_Name),
				From = new MailAddress(template.Email_Sender_Name)
			};

			mail.To.Add(new MailAddress(recipient.Email, recipient.Name));
			mail.Attachments.Add(new Attachment(recipient.PDFData));

			EmailUtil.Send(mail);
		}

		private IDictionary<string, string> ObjectToDictionary(object values)
		{
			var dict = new Dictionary<string, string>();
			var props = values.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var prop in props)
			{
				var propName = prop.Name;
				var value = prop.GetValue(values)?.ToString();

				dict.Add(propName, value);
			}

			return dict;
		}
	}
}
