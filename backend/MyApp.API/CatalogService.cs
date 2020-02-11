using MyApp.Logic.Entities;
using MyApp.Logic;
using Reusable.Rest;
using ServiceStack;
using ServiceStack.OrmLite;
using System;
using System.Threading.Tasks;
using ServiceStack.Text;
using Reusable.Rest.Implementations.SS;
using System.Collections.Generic;
using System.Linq;

///start:slot:imports<<<///end:slot:imports<<<

namespace MyApp.API
{
    // [Authenticate]
    public class CatalogService : BaseService<CatalogLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllCatalogs request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetCatalogById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetCatalogWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        //public object Get(GetPagedCatalogs request)
        //{
        //    return WithDb(db => Logic.GetPaged(
        //        request.Limit,
        //        request.Page,
        //        request.FilterGeneral));
        //}
        #endregion

        #region Endpoints - Generic Write
        public object Post(CreateCatalogInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<Catalog>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertCatalog request)
        {
            var entity = request.ConvertTo<Catalog>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateCatalog request)
        {
            var entity = request.ConvertTo<Catalog>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteCatalog request)
        {
            var entity = request.ConvertTo<Catalog>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdCatalog request)
        {
            var entity = request.ConvertTo<Catalog>();
            return InTransaction(db =>
            {
                Logic.RemoveById(entity.Id);
                return new CommonResponse();
            });
        }
        #endregion

        #region Endpoints - Specific

        public object Get(GetPagedCustomCatalogs request)
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

        [Route("/Catalog/OnlyValues/{CatalogType}", "GET")]
        public class GetOnlyValues : GetPagedCustomCatalogs
        {
            public string CatalogType { get; set; }
        }

        public object Get(GetOnlyValues request)
        {
            var query = AutoQuery.CreateQuery(request, Request);

            return WithDb(db =>
            {
                CommonResponse commonResponse = Logic.GetPaged(
                    request.Limit,
                    request.Page,
                    request.FilterGeneral,
                    query,
                    requiresKeysInJsons: request.RequiresKeysInJsons
                    );

                var result = commonResponse.Result as List<Catalog>;
                return result.Select(e => e.Value);
            });
        }

        ///start:slot:endpoints<<<///end:slot:endpoints<<<
        #endregion
    }

    #region Specific

    ///start:slot:endpointsRoutes<<<
    [Route("/Catalog", "GET")]
    [Route("/Catalog/{Limit}/{Page}", "GET")]
    [Route("/Catalog/{ignore}", "GET")]
    [Route("/Catalog/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedCustomCatalogs : QueryDb<Catalog>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public object Value { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
        public string[] Departments { get; set; }
    }

    [Route("/Catalog/Query")]
    public class QueryCatalogs : QueryDb<Catalog> { }


    ///end:slot:endpointsRoutes<<<
    #endregion

    #region Generic Read Only
    [Route("/Catalog", "GET")]
    public class GetAllCatalogs : GetAll<Catalog> { }

    [Route("/Catalog/{Id}", "GET")]
    public class GetCatalogById : GetSingleById<Catalog> { }

    [Route("/Catalog/GetSingleWhere", "GET")]
    [Route("/Catalog/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetCatalogWhere : GetSingleWhere<Catalog> { }

    //[Route("/Catalog/GetPaged/{Limit}/{Page}", "GET")]
    //public class GetPagedCatalogs : GetPaged<Catalog> { }
    #endregion

    #region Generic Write
    [Route("/Catalog/CreateInstance", "POST")]
    public class CreateCatalogInstance : Catalog { }

    [Route("/Catalog", "POST")]
    public class InsertCatalog : Catalog { }

    [Route("/Catalog", "PUT")]
    public class UpdateCatalog : Catalog { }

    [Route("/Catalog", "DELETE")]
    public class DeleteCatalog : Catalog { }

    [Route("/Catalog/{Id}", "DELETE")]
    public class DeleteByIdCatalog : Catalog { }
    #endregion
}
