using System;
using System.Linq.Expressions;

namespace Reusable.CRUD.Contract
{
    public interface IRepository<T> : IReadOnlyRepository<T> where T : BaseEntity
    {
        void Add(params T[] items);
        void Update(T item);
        void Delete(long id);
        void Delete(T entity);

        P AddToParent<P>(long parentId, T entity) where P : class;
        void RemoveFromParent<P>(long parentId, T entity) where P : class;
        void RemoveAllWhere(params Expression<Func<T, bool>>[] wheres);
        T SetPropertyValue(long entityId, string sProperty, string newValue);
    }
}
