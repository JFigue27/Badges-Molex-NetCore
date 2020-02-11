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
    [Authenticate]
    [RequiredRole(ApplyTo.Post | ApplyTo.Put | ApplyTo.Delete, "admin")]
    public class CatalogDefinitionService : BaseService<CatalogDefinitionLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllCatalogDefinitions request)
        {
            return WithDb(db => Logic.GetAll());
        }
        public object Get(GetCatalogDefinitionById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }
        public object Get(GetCatalogDefinitionWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }
        public object Get(GetPagedCatalogDefinitions request)
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
        public object Post(CreateCatalogDefinitionInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<CatalogDefinition>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }
        public object Post(InsertCatalogDefinition request)
        {
            var entity = request.ConvertTo<CatalogDefinition>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Put(UpdateCatalogDefinition request)
        {
            var entity = request.ConvertTo<CatalogDefinition>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteCatalogDefinition request)
        {
            var entity = request.ConvertTo<CatalogDefinition>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdCatalogDefinition request)
        {
            var entity = request.ConvertTo<CatalogDefinition>();
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
    [Route("/CatalogDefinition", "GET")]
    public class GetAllCatalogDefinitions : GetAll<CatalogDefinition> { }
    [Route("/CatalogDefinition/{Id}", "GET")]
    public class GetCatalogDefinitionById : GetSingleById<CatalogDefinition> { }
    [Route("/CatalogDefinition/GetSingleWhere", "GET")]
    [Route("/CatalogDefinition/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetCatalogDefinitionWhere : GetSingleWhere<CatalogDefinition> { }
    [Route("/CatalogDefinition/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedCatalogDefinitions : QueryDb<CatalogDefinition>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion
    #region Generic Write
    [Route("/CatalogDefinition/CreateInstance", "POST")]
    public class CreateCatalogDefinitionInstance : CatalogDefinition { }
    [Route("/CatalogDefinition", "POST")]
    public class InsertCatalogDefinition : CatalogDefinition { }
    [Route("/CatalogDefinition", "PUT")]
    public class UpdateCatalogDefinition : CatalogDefinition { }
    [Route("/CatalogDefinition", "DELETE")]
    public class DeleteCatalogDefinition : CatalogDefinition { }
    [Route("/CatalogDefinition/{Id}", "DELETE")]
    public class DeleteByIdCatalogDefinition : CatalogDefinition { }
    #endregion
}
