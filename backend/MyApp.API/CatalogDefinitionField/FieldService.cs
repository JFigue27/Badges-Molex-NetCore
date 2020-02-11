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
    public class FieldService : BaseService<FieldLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllFields request)
        {
            return WithDb(db => Logic.GetAll());
        }
        public object Get(GetFieldById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }
        public object Get(GetFieldWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }
        public object Get(GetPagedFields request)
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
        public object Post(CreateFieldInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<Field>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }
        public object Post(InsertField request)
        {
            var entity = request.ConvertTo<Field>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Put(UpdateField request)
        {
            var entity = request.ConvertTo<Field>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteField request)
        {
            var entity = request.ConvertTo<Field>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdField request)
        {
            var entity = request.ConvertTo<Field>();
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
    [Route("/Field", "GET")]
    public class GetAllFields : GetAll<Field> { }
    [Route("/Field/{Id}", "GET")]
    public class GetFieldById : GetSingleById<Field> { }
    [Route("/Field/GetSingleWhere", "GET")]
    [Route("/Field/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetFieldWhere : GetSingleWhere<Field> { }
    [Route("/Field/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedFields : QueryDb<Field>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion
    #region Generic Write
    [Route("/Field/CreateInstance", "POST")]
    public class CreateFieldInstance : Field { }
    [Route("/Field", "POST")]
    public class InsertField : Field { }
    [Route("/Field", "PUT")]
    public class UpdateField : Field { }
    [Route("/Field", "DELETE")]
    public class DeleteField : Field { }
    [Route("/Field/{Id}", "DELETE")]
    public class DeleteByIdField : Field { }
    #endregion
}
