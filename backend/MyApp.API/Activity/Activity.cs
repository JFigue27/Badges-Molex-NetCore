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
    public class Activity : BaseDocument
    {
        public Activity()
        {
            CreatedAt = DateTimeOffset.Now;
        }

        public long ForeignKey { get; set; }
        public string ForeignType { get; set; }
        public string ForeignURL { get; set; }
        public string ForeignApp { get; set; }
        public long? ForeignCommonKey { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        
    }
}
