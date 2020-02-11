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
    public class FilterData : BaseEntity
    {
        public FilterData()
        {
            
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public long AdvancedSortId { get; set; }

        
    }
}
