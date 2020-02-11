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
    public class FilterDataService : BaseService<FilterDataLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllFilterDatas request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetFilterDataById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetFilterDataWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedFilterDatas request)
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
        public object Post(CreateFilterDataInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<FilterData>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertFilterData request)
        {
            var entity = request.ConvertTo<FilterData>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateFilterData request)
        {
            var entity = request.ConvertTo<FilterData>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteFilterData request)
        {
            var entity = request.ConvertTo<FilterData>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdFilterData request)
        {
            var entity = request.ConvertTo<FilterData>();
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
    [Route("/FilterData", "GET")]
    public class GetAllFilterDatas : GetAll<FilterData> { }

    [Route("/FilterData/{Id}", "GET")]
    public class GetFilterDataById : GetSingleById<FilterData> { }

    [Route("/FilterData/GetSingleWhere", "GET")]
    [Route("/FilterData/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetFilterDataWhere : GetSingleWhere<FilterData> { }

    [Route("/FilterData/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedFilterDatas : QueryDb<FilterData> {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion

    #region Generic Write
    [Route("/FilterData/CreateInstance", "POST")]
    public class CreateFilterDataInstance : FilterData { }

    [Route("/FilterData", "POST")]
    public class InsertFilterData : FilterData { }

    [Route("/FilterData", "PUT")]
    public class UpdateFilterData : FilterData { }

    [Route("/FilterData", "DELETE")]
    public class DeleteFilterData : FilterData { }

    [Route("/FilterData/{Id}", "DELETE")]
    public class DeleteByIdFilterData : FilterData { }
    #endregion
}
