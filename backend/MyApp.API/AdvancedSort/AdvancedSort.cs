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
    public class AdvancedSort : BaseEntity
    {
        public AdvancedSort()
        {
            
        }

        public string Name { get; set; }
        public string UserName { get; set; }
        [Reference]
        public List<SortData> Sorting { get; set; }
        [Reference]
        public List<FilterData> Filtering { get; set; }

        
    }
}
