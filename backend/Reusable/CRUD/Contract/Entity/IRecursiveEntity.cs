using ServiceStack.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace Reusable.CRUD.Contract
{
    public interface IRecursiveEntity<T> : IEntity where T : IEntity
    {
        [Ignore]
        [NotMapped]
        List<T> Children { get; set; }
    }
}
