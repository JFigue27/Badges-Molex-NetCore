using Reusable.CRUD.Entities;
using ServiceStack.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reusable.CRUD.Contract
{
    public abstract class BaseDocument : Trackable
    {
        //virtual public bool IsLockedOut { get; set; }

        virtual public string DocumentStatus { get; set; }

        public string CheckedoutBy { get; set; }

        [Ignore]
        [NotMapped]
        public List<Revision> Revisions { get; set; }

        public string RevisionMessage { get; set; }
    }
}
