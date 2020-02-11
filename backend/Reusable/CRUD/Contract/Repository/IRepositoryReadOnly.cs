using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Reusable.CRUD.Contract
{
    public interface IReadOnlyRepository<T> where T : class
    {
        long? ByUserId { get; set; }
        string EntityName { get; set; }

        IEnumerable<T> GetAll(Expression<Func<T, object>> orderBy = null, params Expression<Func<T, object>>[] navigationProperties);
        IEnumerable<T> GetList(Expression<Func<T, object>> orderBy, Expression<Func<T, object>>[] navigationProperties, params Expression<Func<T, bool>>[] wheres);
        T GetByID(long id, params Expression<Func<T, object>>[] navigationProperties);
        T GetSingle(Expression<Func<T, object>>[] navigationProperties, params Expression<Func<T, bool>>[] wheres);

        IEnumerable<T> GetListByParent<P>(long parentID, params Expression<Func<T, object>>[] navigationProperties) where P : class;
        IList<T> GetListByParent<P>(long parentID, string customProperty, params Expression<Func<T, object>>[] navigationProperties) where P : class;
        T GetSingleByParent<P>(long parentID, params Expression<Func<T, object>>[] navigationProperties) where P : class;
    }
}
