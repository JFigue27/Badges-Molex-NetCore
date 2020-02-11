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
    public class Field : BaseEntity
    {
        public Field()
        {
        }

        public string FieldName { get; set; }
        public string FieldType { get; set; }

        //[Reference]
        //public CatalogDefinition CatalogDefinition { get; set; }
        [References(typeof(CatalogDefinition))]
        public long CatalogDefinitionId { get; set; }

        [Reference]
        public CatalogDefinition Foreign { get; set; }
        [References(typeof(CatalogDefinition))]
        public long? ForeignId { get; set; }
    }
}
