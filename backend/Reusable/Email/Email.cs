using Reusable.Attachments;
using ServiceStack.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace Reusable.EmailServices
{
    public class EmailService : IEmailService
    {
        public static IAppSettings AppSettings { get; set; }
        private SmtpClient smtp;

        public string From { get; set; }
        public string FromPassword { get; set; }

        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        public string AttachmentsFolder { get; set; }
        public string Template { get; set; }
        public Dictionary<string, object> TemplateParameters { get; set; }

        public EmailService()
        {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();

            From = AppSettings.Get<string>("smtp.from");
            FromPassword = AppSettings.Get<string>("smtp.password");

            var smtpServer = AppSettings.Get<string>("smtp.server");
            var smtpPort = AppSettings.Get<string>("smtp.port");

            smtp = new SmtpClient(smtpServer, int.Parse(smtpPort));
        }

        public void SendMail()
        {
            try
            {
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(From, FromPassword);

                MailMessage message = new MailMessage();
                message.From = new MailAddress(From, From);

                foreach (var to in To)
                {
                    message.To.Add(new MailAddress(to));
                }
                foreach (var cc in Cc)
                {
                    message.CC.Add(new MailAddress(cc));
                }
                foreach (var bcc in Bcc)
                {
                    message.Bcc.Add(new MailAddress(bcc));
                }

                message.Subject = Subject;
                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.UTF8;

                message.Body = Body;

                string baseAttachmentsPath = AppSettings.Get<string>("EmailAttachments");
                var attachments = AttachmentsIO.getAttachmentsFromFolder(AttachmentsFolder, "EmailAttachments");
                foreach (var attachment in attachments)
                {
                    string filePath = baseAttachmentsPath + attachment.Directory + "\\" + attachment.FileName;
                    FileInfo file = new FileInfo(filePath);
                    message.Attachments.Add(new System.Net.Mail.Attachment(new FileStream(filePath, FileMode.Open, FileAccess.Read), attachment.FileName));
                }

                smtp.Send(message);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
