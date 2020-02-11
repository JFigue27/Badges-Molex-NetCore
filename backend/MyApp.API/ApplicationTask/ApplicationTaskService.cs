using MyApp.Logic.Entities;
using MyApp.Logic;
using Reusable.Rest;
using ServiceStack;
using ServiceStack.OrmLite;
using System;
using System.Threading.Tasks;
using ServiceStack.Text;
using Reusable.Rest.Implementations.SS;


namespace MyApp.API
{
    // [Authenticate]
    public class ApplicationTaskService : BaseService<ApplicationTaskLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllApplicationTasks request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetApplicationTaskById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetApplicationTaskWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedApplicationTasks request)
        {
            var query = AutoQuery.CreateQuery(request, Request);

            return WithDb(db => Logic.GetPaged(
                request.Limit,
                request.Page,
                request.FilterGeneral,
                query,
                requiresKeysInJsons: request.RequiresKeysInJsons
                ));
        }
        #endregion

        #region Endpoints - Generic Write
        public object Post(CreateApplicationTaskInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<ApplicationTask>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertApplicationTask request)
        {
            var entity = request.ConvertTo<ApplicationTask>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateApplicationTask request)
        {
            var entity = request.ConvertTo<ApplicationTask>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteApplicationTask request)
        {
            var entity = request.ConvertTo<ApplicationTask>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdApplicationTask request)
        {
            var entity = request.ConvertTo<ApplicationTask>();
            return InTransaction(db =>
            {
                Logic.RemoveById(entity.Id);
                return new CommonResponse();
            });
        }
        #endregion

        #region Endpoints - Specific
        
        #endregion
    }

    #region Specific
    
    #endregion

    #region Generic Read Only
    [Route("/ApplicationTask", "GET")]
    public class GetAllApplicationTasks : GetAll<ApplicationTask> { }

    [Route("/ApplicationTask/{Id}", "GET")]
    public class GetApplicationTaskById : GetSingleById<ApplicationTask> { }

    [Route("/ApplicationTask/GetSingleWhere", "GET")]
    [Route("/ApplicationTask/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetApplicationTaskWhere : GetSingleWhere<ApplicationTask> { }

    [Route("/ApplicationTask/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedApplicationTasks : QueryDb<ApplicationTask>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion

    #region Generic Write
    [Route("/ApplicationTask/CreateInstance", "POST")]
    public class CreateApplicationTaskInstance : ApplicationTask { }

    [Route("/ApplicationTask", "POST")]
    public class InsertApplicationTask : ApplicationTask { }

    [Route("/ApplicationTask", "PUT")]
    public class UpdateApplicationTask : ApplicationTask { }

    [Route("/ApplicationTask", "DELETE")]
    public class DeleteApplicationTask : ApplicationTask { }

    [Route("/ApplicationTask/{Id}", "DELETE")]
    public class DeleteByIdApplicationTask : ApplicationTask { }
    #endregion
}
