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
        }
        public string Value { get; set; }
        public bool Hidden { get; set; }

        [Reference]
        public CatalogDefinition CatalogDefinition { get; set; }
        public long CatalogDefinitionId { get; set; }

        [Reference]
        public List<CatalogFieldValue> Values { get; set; }
    }
}
