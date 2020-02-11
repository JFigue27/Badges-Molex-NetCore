using Reusable.CRUD.Contract;
using Reusable.CRUD.Entities;
using Reusable.CRUD.Implementations.SS.Logic;
using ServiceStack;
using ServiceStack.Text;
using System.Threading.Tasks;

namespace Reusable.Rest.Implementations.SS
{
    [Restrict(LocalhostOnly = true)]
    // [Authenticate]
    public class RevisionService : BaseService<RevisionLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllRevisions request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetRevisionById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetRevisionWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedRevisions request)
        {
            return WithDb(db => Logic.GetPaged(
                request.Limit,
                request.Page,
                request.FilterGeneral));
        }
        #endregion

        #region Endpoints - Generic Write
        public object Post(CreateRevisionInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<Revision>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertRevision request)
        {
            var entity = request.ConvertTo<Revision>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateRevision request)
        {
            var entity = request.ConvertTo<Revision>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteRevision request)
        {
            var entity = request.ConvertTo<Revision>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
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
    [Route("/Revision", "GET")]
    public class GetAllRevisions : GetAll<Revision> { }

    [Route("/Revision/{Id}", "GET")]
    public class GetRevisionById : GetSingleById<Revision> { }

    [Route("/Revision/GetSingleWhere", "GET")]
    [Route("/Revision/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetRevisionWhere : GetSingleWhere<Revision> { }

    [Route("/Revision/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedRevisions : GetPaged<Revision> { }
    #endregion

    #region Generic Write
    [Route("/Revision/CreateInstance", "POST")]
    public class CreateRevisionInstance : Revision { }

    [Route("/Revision", "POST")]
    public class InsertRevision : Revision { }

    [Route("/Revision", "PUT")]
    public class UpdateRevision : Revision { }

    [Route("/Revision", "DELETE")]
    [Route("/Revision/{Id}", "DELETE")]
    public class DeleteRevision : Revision { }
    #endregion
}
