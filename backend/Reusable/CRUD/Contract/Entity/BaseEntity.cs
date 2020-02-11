using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Reusable.CRUD.Contract
{
    public abstract class BaseEntity : IEntity
    {
        [AutoIncrement]
        public long Id { get; set; }
        //Optimistic Concurrency:
        [Timestamp]
        public byte[] RowVersion { get; set; }
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
        public override bool Equals(object obj)
        {
            //Check whether the compared object is null.
            if (ReferenceEquals(obj, null)) return false;
            //Check whether the compared object is same type.
            if (!GetType().Name.Split('_')[0].Equals(obj.GetType().Name.Split('_')[0])) return false;
            //Check whether the compared object references the same data.
            if (ReferenceEquals(this, obj)) return true;
            //Check whether the IEntity' ids are equal.
            return Id.Equals(((BaseEntity)obj).Id);
        }
        public override int GetHashCode()
        {
            //Get hash code for the id field if it is not null.
            int hashID = Id.GetHashCode();
            return hashID;
        }
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
