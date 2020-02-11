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
    public class Approval : BaseDocument
    {
        public Approval()
        {
            RequestedDate = DateTimeOffset.Now;
            ApplicationTasks = new List<ApplicationTask>();
        }

        public DateTimeOffset RequestedDate { get; set; }
        public string RequestDescription { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Hyperlink { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public int? RequiredQuantity { get; set; }
        public double? RequiredPercentage { get; set; }
        public string Approvers { get; set; }
        public string Subscribers { get; set; }
        public string Status { get; set; }
        public string ForeignApp { get; set; }
        public string ForeignType { get; set; }
        public long? ForeignKey { get; set; }
        public string ForeignTarget { get; set; }
        [NotMapped]
        [Ignore]
        public List<ApplicationTask> ApplicationTasks { get; set; }

        [NotMapped]
        [Ignore]
        public List<Contact> ApproversList
        {
            get { return Approvers.FromJson<List<Contact>>(); }
            set { Approvers = value.ToJson(); }
        }

        [NotMapped]
        [Ignore]
        public List<Contact> SubscribersList
        {
            get { return Subscribers.FromJson<List<Contact>>(); }
            set { Subscribers = value.ToJson(); }
        }

    }
}
