using Reusable.CRUD.Contract;

namespace Reusable.Rest
{
    #region Read Only ROUTES / DTOs
    public abstract class GetAll<T> where T : IEntity { }

    public abstract class GetSingleById<T> where T : IEntity
    {
        public long Id { get; set; }
    }

    public abstract class GetSingleWhere<T> where T : IEntity
    {
        public string Property { get; set; }
        public object Value { get; set; }
    }

    public abstract class GetPaged<T> where T : IEntity
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public object Value { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
    }
    #endregion
}
