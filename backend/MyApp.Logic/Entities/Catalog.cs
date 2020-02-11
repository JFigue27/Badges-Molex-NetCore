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
    public class Catalog : BaseEntity
    {
        public Catalog()
        {
            
            ///start:slot:ctor<<<///end:slot:ctor<<<
        }

        public string Value { get; set; }
        public string CatalogType { get; set; }
        public bool Hidden { get; set; }
        public string Parent { get; set; }
        [IsJson]
        public string Meta { get; set; }

        
        ///start:slot:properties<<<///end:slot:properties<<<
    }
}
