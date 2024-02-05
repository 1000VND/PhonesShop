using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Mail
{
	internal class EmailService : IEmailService
	{
		public EmailSettings _smtpSetting { get; }
		public ILogger<EmailService> _logger { get; }

		public EmailService(IOptions<EmailSettings> smtpSetting, ILogger<EmailService> logger)
		{
			_smtpSetting = smtpSetting.Value;
			_logger = logger;
		}

		public async Task<bool> SendEmail(Email email)
		{
			try
			{
				var subject = email.Subject;
				var emailBody = email.Body;

				var message = new MailMessage(_smtpSetting.User, email.To, subject, emailBody);
				using (var emailClient = new SmtpClient(_smtpSetting.Host, _smtpSetting.Port))
				{
					emailClient.Credentials = new NetworkCredential(
						_smtpSetting.User,
						_smtpSetting.Password);

					await emailClient.SendMailAsync(message);
				}

				_logger.LogInformation("Email sent successfully.");

				return true;
			}
			catch
			{
				_logger.LogError("Email sending failed.");
				return false;

			}
		}
	}
}
