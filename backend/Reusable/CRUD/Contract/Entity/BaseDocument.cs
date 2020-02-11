using Reusable.CRUD.Entities;
using ServiceStack.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reusable.CRUD.Contract
{
    public abstract class BaseDocument : Trackable
    {
        //Optimistic Concurrency:
        [Timestamp]
        public byte[] RowVersion { get; set; }

        virtual public string DocumentStatus { get; set; }

        public string CheckedoutBy { get; set; }

        [Ignore]
        [NotMapped]
        public List<Revision> Revisions { get; set; }

        public string RevisionMessage { get; set; }
    }
}
