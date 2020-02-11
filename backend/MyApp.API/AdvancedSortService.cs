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
    public class AdvancedSortService : BaseService<AdvancedSortLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllAdvancedSorts request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetAdvancedSortById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetAdvancedSortWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedAdvancedSorts request)
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
        public object Post(CreateAdvancedSortInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<AdvancedSort>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertAdvancedSort request)
        {
            var entity = request.ConvertTo<AdvancedSort>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateAdvancedSort request)
        {
            var entity = request.ConvertTo<AdvancedSort>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteAdvancedSort request)
        {
            var entity = request.ConvertTo<AdvancedSort>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdAdvancedSort request)
        {
            var entity = request.ConvertTo<AdvancedSort>();
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
    [Route("/AdvancedSort", "GET")]
    public class GetAllAdvancedSorts : GetAll<AdvancedSort> { }

    [Route("/AdvancedSort/{Id}", "GET")]
    public class GetAdvancedSortById : GetSingleById<AdvancedSort> { }

    [Route("/AdvancedSort/GetSingleWhere", "GET")]
    [Route("/AdvancedSort/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetAdvancedSortWhere : GetSingleWhere<AdvancedSort> { }

    [Route("/AdvancedSort/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedAdvancedSorts : QueryDb<AdvancedSort> {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion

    #region Generic Write
    [Route("/AdvancedSort/CreateInstance", "POST")]
    public class CreateAdvancedSortInstance : AdvancedSort { }

    [Route("/AdvancedSort", "POST")]
    public class InsertAdvancedSort : AdvancedSort { }

    [Route("/AdvancedSort", "PUT")]
    public class UpdateAdvancedSort : AdvancedSort { }

    [Route("/AdvancedSort", "DELETE")]
    public class DeleteAdvancedSort : AdvancedSort { }

    [Route("/AdvancedSort/{Id}", "DELETE")]
    public class DeleteByIdAdvancedSort : AdvancedSort { }
    #endregion
}
