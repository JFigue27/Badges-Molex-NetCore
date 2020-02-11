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
    public class CatalogFieldValueService : BaseService<CatalogFieldValueLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllCrossCatalogDefinitionFields request)
        {
            return WithDb(db => Logic.GetAll());
        }
        public object Get(GetCrossCatalogDefinitionFieldById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }
        public object Get(GetCrossCatalogDefinitionFieldWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }
        public object Get(GetPagedCrossCatalogDefinitionFields request)
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
        public object Post(CreateCrossCatalogDefinitionFieldInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<CatalogFieldValue>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }
        public object Post(InsertCrossCatalogDefinitionField request)
        {
            var entity = request.ConvertTo<CatalogFieldValue>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Put(UpdateCrossCatalogDefinitionField request)
        {
            var entity = request.ConvertTo<CatalogFieldValue>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteCrossCatalogDefinitionField request)
        {
            var entity = request.ConvertTo<CatalogFieldValue>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdCrossCatalogDefinitionField request)
        {
            var entity = request.ConvertTo<CatalogFieldValue>();
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
    [Route("/CrossCatalogDefinitionField", "GET")]
    public class GetAllCrossCatalogDefinitionFields : GetAll<CatalogFieldValue> { }
    [Route("/CrossCatalogDefinitionField/{Id}", "GET")]
    public class GetCrossCatalogDefinitionFieldById : GetSingleById<CatalogFieldValue> { }
    [Route("/CrossCatalogDefinitionField/GetSingleWhere", "GET")]
    [Route("/CrossCatalogDefinitionField/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetCrossCatalogDefinitionFieldWhere : GetSingleWhere<CatalogFieldValue> { }
    [Route("/CrossCatalogDefinitionField/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedCrossCatalogDefinitionFields : QueryDb<CatalogFieldValue>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion
    #region Generic Write
    [Route("/CrossCatalogDefinitionField/CreateInstance", "POST")]
    public class CreateCrossCatalogDefinitionFieldInstance : CatalogFieldValue { }
    [Route("/CrossCatalogDefinitionField", "POST")]
    public class InsertCrossCatalogDefinitionField : CatalogFieldValue { }
    [Route("/CrossCatalogDefinitionField", "PUT")]
    public class UpdateCrossCatalogDefinitionField : CatalogFieldValue { }
    [Route("/CrossCatalogDefinitionField", "DELETE")]
    public class DeleteCrossCatalogDefinitionField : CatalogFieldValue { }
    [Route("/CrossCatalogDefinitionField/{Id}", "DELETE")]
    public class DeleteByIdCrossCatalogDefinitionField : CatalogFieldValue { }
    #endregion
}
