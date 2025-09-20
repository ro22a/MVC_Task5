

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace ETicketsSystem.Utility
{
	public class EmailSender : IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var client = new SmtpClient("smtp.gmail.com", 587)
			{
				EnableSsl = true,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential("rosa.ayman66@gmail.com", "tksa pupy etdc mhuu")
			};

			return client.SendMailAsync(
				new MailMessage(from: "rosa.ayman66@gmail.com",
				to: email,
				subject,
				htmlMessage)
			{
					IsBodyHtml= true

			});


			

		}
	}
}
