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
    public class SortDataService : BaseService<SortDataLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllSortDatas request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetSortDataById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetSortDataWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedSortDatas request)
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
        public object Post(CreateSortDataInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<SortData>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertSortData request)
        {
            var entity = request.ConvertTo<SortData>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateSortData request)
        {
            var entity = request.ConvertTo<SortData>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteSortData request)
        {
            var entity = request.ConvertTo<SortData>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdSortData request)
        {
            var entity = request.ConvertTo<SortData>();
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
    [Route("/SortData", "GET")]
    public class GetAllSortDatas : GetAll<SortData> { }

    [Route("/SortData/{Id}", "GET")]
    public class GetSortDataById : GetSingleById<SortData> { }

    [Route("/SortData/GetSingleWhere", "GET")]
    [Route("/SortData/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetSortDataWhere : GetSingleWhere<SortData> { }

    [Route("/SortData/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedSortDatas : QueryDb<SortData>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion

    #region Generic Write
    [Route("/SortData/CreateInstance", "POST")]
    public class CreateSortDataInstance : SortData { }

    [Route("/SortData", "POST")]
    public class InsertSortData : SortData { }

    [Route("/SortData", "PUT")]
    public class UpdateSortData : SortData { }

    [Route("/SortData", "DELETE")]
    public class DeleteSortData : SortData { }

    [Route("/SortData/{Id}", "DELETE")]
    public class DeleteByIdSortData : SortData { }
    #endregion
}
