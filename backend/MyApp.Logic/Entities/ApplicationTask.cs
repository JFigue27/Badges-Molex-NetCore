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
    public class ApplicationTask : BaseEntity
    {
        public ApplicationTask()
        {
            CreatedAt = DateTimeOffset.Now;
        }

        public string Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string AssignedTo { get; set; }
        public string ClosedBy { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
        public long? ForeignKey { get; set; }
        public string ForeignType { get; set; }
        public string ForeignURL { get; set; }
        public string ForeignApp { get; set; }
        public bool IsCanceled { get; set; }

        
    }
}
