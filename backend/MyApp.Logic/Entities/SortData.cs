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
    public class SortData : BaseEntity
    {
        public SortData()
        {
            
        }

        public string Value { get; set; }
        public int Sequence { get; set; }
        public string AscDesc { get; set; }
        public long AdvancedSortId { get; set; }

        
    }
}
