using Reusable.Attachments;
using ServiceStack.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using ServiceStack;
using RestSharp;
using System;
using RestSharp.Authenticators;

namespace Reusable.EmailServices
{
    public class MailgunService : IEmailService
    {
        public MailgunService()
        {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            TemplateParameters = new Dictionary<string, object>();

            From = ConfigurationManager.AppSettings["mailgun.from"];
            API_KEY = ConfigurationManager.AppSettings["mailgun.api.key"];

            var url = ConfigurationManager.AppSettings["mailgun.api.url"];
            if (!string.IsNullOrWhiteSpace(url))
                Client = new RestClient
                {
                    BaseUrl = new Uri(url),
                    Authenticator = new HttpBasicAuthenticator("api", API_KEY)
                };
        }

        public IAppSettings AppSettings { get; set; }

        public string From { get; set; }
        public string FromPassword { get; set; }
        public string API_KEY { get; set; }

        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public string Template { get; set; }
        public Dictionary<string, object> TemplateParameters { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }

        RestClient Client;

        public string AttachmentsFolder { get; set; }

        public void SendMail()
        {
            var request = new RestRequest();
            request.Method = Method.POST;

            request.AddParameter("from", From);

            foreach (var to in To)
                request.AddParameter("to", to);

            foreach (var cc in Cc)
                request.AddParameter("cc", cc);

            foreach (var bcc in Bcc)
                request.AddParameter("bcc", bcc);

            request.AddParameter("subject", Subject);

            string baseAttachmentsPath = ConfigurationManager.AppSettings["EmailAttachments"];
            var attachments = AttachmentsIO.getAttachmentsFromFolder(AttachmentsFolder, "EmailAttachments");
            foreach (var attachment in attachments)
            {
                string filePath = baseAttachmentsPath + attachment.Directory + "\\" + attachment.FileName;
                request.AddFile("attachment", filePath);
            }

            if (!string.IsNullOrWhiteSpace(Template))
            {
                request.AddParameter("template", Template);

                foreach (var param in TemplateParameters)
                    request.AddParameter($"v:{param.Key}", param.Value);
            }
            else
                request.AddParameter("html", Body);

            var response = Client.Execute(request);
            var content = response.Content;
            if (!response.IsSuccessful) throw new Exception(content);
        }
    }
}
