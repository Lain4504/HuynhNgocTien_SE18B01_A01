using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace NewsManagementSystem_Assigment01.Services
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class SendMailService
    {
        MailSettings _mailSettings {  get; set; }
        public SendMailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task<string> SendMailAsync(MailContent mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));

            email.To.Add(new MailboxAddress(mailContent.To, mailContent.To));
            email.Subject = mailContent.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
               await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
               await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
               await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "LOI " + ex.Message;
            }


            smtp.Disconnect(true);
            return "Gui Thanh cong";
        }

    }

    public class MailContent
    {
        public string To { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
    }
           
}
