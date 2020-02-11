using Reusable.Attachments;
using Reusable.CRUD.Contract;
using Reusable.CRUD.Entities;
using Reusable.CRUD.JsonEntities;
using ServiceStack;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Logic.Entities
{
    public class Email : BaseDocument
    {
        public Email()
        {
            ToList = new List<Contact>();
            CcList = new List<Contact>();
            BccList = new List<Contact>();
            Attachments = new List<Attachment>();
        }

        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AdditionalNote { get; set; }
        public string AttachmentsFolder { get; set; }
        [NotMapped]
        [Ignore]
        public List<Attachment> Attachments { get; set; }
        [NotMapped]
        [Ignore]
        public bool IsResent { get; set; }
        public long ForeignKey { get; set; }
        public string ForeignType { get; set; }
        public string ForeignURL { get; set; }
        public string ForeignApp { get; set; }
        public long? ForeignCommonKey { get; set; }

        [Ignore]
        public List<Contact> ToList
        {
            get { return To.FromJson<List<Contact>>(); }
            set { To = value.ToJson(); }
        }
        [Ignore]
        public List<Contact> CcList
        {
            get { return Cc.FromJson<List<Contact>>(); }
            set { Cc = value.ToJson(); }
        }
        [Ignore]
        public List<Contact> BccList
        {
            get { return Bcc.FromJson<List<Contact>>(); }
            set { Bcc = value.ToJson(); }
        }

    }
}
