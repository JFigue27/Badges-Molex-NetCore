using Reusable.CRUD.Contract;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reusable.CRUD.Entities
{
    public class Revision : BaseDocument
    {
        [NotMapped]
        public string Discriminator { get; set; }

        public string ForeignType { get; set; }
        public long ForeignKey { get; set; }
        public string Value { get; set; }
    }
}
