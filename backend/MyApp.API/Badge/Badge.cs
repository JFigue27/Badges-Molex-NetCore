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
    public class Badge : BaseDocument
    {
        public Badge()
        {
            CheckIn = DateTimeOffset.Now;
        }

        public string Value { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Citizenship { get; set; }
        public string Visiting { get; set; }
        public DateTimeOffset CheckIn { get; set; }
        public DateTimeOffset? CheckOut { get; set; }

        
    }
}
