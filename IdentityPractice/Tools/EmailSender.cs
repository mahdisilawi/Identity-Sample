using System.Net;
using System.Net.Mail;

namespace IdentityPractice.Tools
{

    public interface IEmailSender
    {
        Task SendEmailAysnc(EmailModel email);
    }

    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAysnc(EmailModel email)
        {
            string fromPassword = "your Gmail Password";

            MailMessage message = new MailMessage()
            {
                From = new MailAddress("your Gmail Account","کافه مود"),
                To = {email.To},
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true,
            };

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                UseDefaultCredentials =false,
                Credentials = new NetworkCredential(message.From.Address,fromPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network               
            };
            smtpClient.EnableSsl = true;
            smtpClient.Send(message);
            await Task.CompletedTask;
        }
    } 


    public class EmailModel
    {
        public EmailModel(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }

        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
