using System.Net.Mail;
using System.Net;

namespace LotusGoIMWebAPI.Common
{
    public class EmailHelper
    {
        private readonly IConfiguration configuration;

        public EmailHelper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void SendMessage(string to, string subject, string body)
        {
            MailMessage message = new MailMessage();
            message.To.Add(to);
            message.From = new MailAddress(configuration["Email:Address"]!, "LotusGoIM", System.Text.Encoding.UTF8);
            message.Subject = subject;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.Body = body;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = false;
            message.Priority = MailPriority.Normal;

            SmtpClient client = new SmtpClient();

            client.Credentials = new NetworkCredential(configuration["Email:Address"], configuration["Email:AuthCode"]);

            client.Port = int.Parse(configuration["Email:Port"]??"25");

            client.Host = configuration["Email:Host"]!;

            try
            {
                client.Send(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
