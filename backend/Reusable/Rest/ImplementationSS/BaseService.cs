using Reusable.CRUD.Implementations.SS;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Reusable.Rest.Implementations.SS
{
    public class BaseService<TLogic> : Service where TLogic : BaseLogic
    {
        public IDbConnectionFactory DbConnectionFactory { get; set; }
        public TLogic Logic { get; set; }
        public IAutoQueryDb AutoQuery { get; set; }

        public IAppSettings AppSettings { get; set; }

        protected async Task<object> WithDbAsync(Func<IDbConnection, Task<object>> operation)
        {
            using (var db = await DbConnectionFactory.OpenAsync())
            {
                Logic.Init(db, GetSession(), Request, AppSettings);
                return await operation(db);
            }
        }

        protected object WithDb(Func<IDbConnection, object> operation)
        {
            using (var db = DbConnectionFactory.Open())
            {
                Logic.Init(db, GetSession(), Request, AppSettings);
                return operation(db);
            }
        }

        protected object InTransaction(Func<IDbConnection, object> operation)
        {
            return WithDb(db =>
            {
                using (var transaction = db.OpenTransaction())
                {
                    try
                    {
                        var result = operation(db);
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            });
        }

        protected void WithDb(Action<IDbConnection> operation)
        {
            using (var db = DbConnectionFactory.Open())
            {
                Logic.Init(db, GetSession(), Request, AppSettings);
                operation(db);
            }
        }

        protected void InTransaction(Action<IDbConnection> operation)
        {
            WithDb(db =>
            {
                using (var transaction = db.OpenTransaction())
                {
                    try
                    {
                        operation(db);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            });
        }
    }
}
