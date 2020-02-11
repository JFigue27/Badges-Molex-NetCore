using ServiceStack;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reusable.CRUD.Contract
{
    public abstract class BaseEntity : IEntity
    {
        [AutoIncrement]
        public long Id { get; set; }

        [Ignore]
        [NotMapped]
        public string EntityName { get { return GetType().Name; } }

        [Ignore]
        [NotMapped]
        public EntryState Entry_State { get; set; }

        public enum EntryState
        {
            Unchanged,
            Upserted,
            Deleted
        }

        public virtual T Clone<T>()
        {
            return this.ToJson().FromJson<T>();
        }
    }
}
