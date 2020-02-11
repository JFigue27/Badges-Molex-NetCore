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
    public class CatalogFieldValue : BaseEntity
    {
        public CatalogFieldValue()
        {
            IdsList = new long[] { };
        }

        //When scalar (bool, strings, so on):
        public string Value { get; set; }

        //When Relationships(has one or has many):
        public string Ids { get; set; }

        public long CatalogId { get; set; }

        [Reference]
        public Field Field { get; set; }
        public long? FieldId { get; set; }

        [Ignore]
        [NotMapped]
        public long[] IdsList
        {
            get
            {
                return Ids.FromJson<long[]>();
            }
            set
            {
                Ids = value.ToJson();
            }
        }
    }
}
