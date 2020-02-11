using MyApp.Logic.Entities;
using MyApp.Logic;
using Reusable.Rest;
using ServiceStack;
using ServiceStack.OrmLite;
using System;
using System.Threading.Tasks;
using ServiceStack.Text;
using Reusable.Rest.Implementations.SS;

///start:slot:imports<<<///end:slot:imports<<<

namespace MyApp.API
{
    [Authenticate]
    [RequiredRole(ApplyTo.Post | ApplyTo.Put | ApplyTo.Delete, "admin")]
    public class CatalogTypeService : BaseService<CatalogTypeLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllCatalogTypes request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetCatalogTypeById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetCatalogTypeWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedCatalogTypes request)
        {
            return WithDb(db => Logic.GetPaged(
                request.Limit,
                request.Page,
                request.FilterGeneral));
        }
        #endregion

        #region Endpoints - Generic Write
        public object Post(CreateCatalogTypeInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<CatalogType>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertCatalogType request)
        {
            var entity = request.ConvertTo<CatalogType>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateCatalogType request)
        {
            var entity = request.ConvertTo<CatalogType>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteCatalogType request)
        {
            var entity = request.ConvertTo<CatalogType>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdCatalogType request)
        {
            var entity = request.ConvertTo<CatalogType>();
            return InTransaction(db =>
            {
                Logic.RemoveById(entity.Id);
                return new CommonResponse();
            });
        }
        #endregion

        #region Endpoints - Specific
        
        ///start:slot:endpoints<<<///end:slot:endpoints<<<
        #endregion
    }

    #region Specific
    
    ///start:slot:endpointsRoutes<<<///end:slot:endpointsRoutes<<<
    #endregion

    #region Generic Read Only
    [Route("/CatalogType", "GET")]
    public class GetAllCatalogTypes : GetAll<CatalogType> { }

    [Route("/CatalogType/{Id}", "GET")]
    public class GetCatalogTypeById : GetSingleById<CatalogType> { }

    [Route("/CatalogType/GetSingleWhere", "GET")]
    [Route("/CatalogType/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetCatalogTypeWhere : GetSingleWhere<CatalogType> { }

    [Route("/CatalogType/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedCatalogTypes : GetPaged<CatalogType> { }
    #endregion

    #region Generic Write
    [Route("/CatalogType/CreateInstance", "POST")]
    public class CreateCatalogTypeInstance : CatalogType { }

    [Route("/CatalogType", "POST")]
    public class InsertCatalogType : CatalogType { }

    [Route("/CatalogType", "PUT")]
    public class UpdateCatalogType : CatalogType { }

    [Route("/CatalogType", "DELETE")]
    public class DeleteCatalogType : CatalogType { }

    [Route("/CatalogType/{Id}", "DELETE")]
    public class DeleteByIdCatalogType : CatalogType { }
    #endregion
}
