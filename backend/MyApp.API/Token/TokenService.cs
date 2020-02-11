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
    public class TokenService : BaseService<TokenLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllTokens request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetTokenById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetTokenWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedTokens request)
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
        public object Post(CreateTokenInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<Token>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertToken request)
        {
            var entity = request.ConvertTo<Token>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateToken request)
        {
            var entity = request.ConvertTo<Token>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteToken request)
        {
            var entity = request.ConvertTo<Token>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdToken request)
        {
            var entity = request.ConvertTo<Token>();
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
    [Route("/Token", "GET")]
    public class GetAllTokens : GetAll<Token> { }

    [Route("/Token/{Id}", "GET")]
    public class GetTokenById : GetSingleById<Token> { }

    [Route("/Token/GetSingleWhere", "GET")]
    [Route("/Token/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetTokenWhere : GetSingleWhere<Token> { }

    [Route("/Token/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedTokens : QueryDb<Token>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion

    #region Generic Write
    [Route("/Token/CreateInstance", "POST")]
    public class CreateTokenInstance : Token { }

    [Route("/Token", "POST")]
    public class InsertToken : Token { }

    [Route("/Token", "PUT")]
    public class UpdateToken : Token { }

    [Route("/Token", "DELETE")]
    public class DeleteToken : Token { }

    [Route("/Token/{Id}", "DELETE")]
    public class DeleteByIdToken : Token { }
    #endregion
}
