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
    public class BadgeService : BaseService<BadgeLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllBadges request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetBadgeById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetBadgeWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedBadges request)
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
        public object Post(CreateBadgeInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<Badge>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertBadge request)
        {
            var entity = request.ConvertTo<Badge>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateBadge request)
        {
            var entity = request.ConvertTo<Badge>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteBadge request)
        {
            var entity = request.ConvertTo<Badge>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdBadge request)
        {
            var entity = request.ConvertTo<Badge>();
            return InTransaction(db =>
            {
                Logic.RemoveById(entity.Id);
                return new CommonResponse();
            });
        }
        #endregion

        #region Endpoints - Generic Document
        virtual public object Post(MakeBadgeRevision request)
        {
            var entity = request.ConvertTo<Badge>();
            return InTransaction(db =>
            {
                Logic.MakeRevision(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CheckoutBadge request)
        {
            var entity = request.ConvertTo<Badge>();
            return InTransaction(db =>
            {
                Logic.Checkout(entity.Id);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CancelCheckoutBadge request)
        {
            var entity = request.ConvertTo<Badge>();
            return InTransaction(db =>
            {
                Logic.CancelCheckout(entity.Id);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CheckinBadge request)
        {
            var entity = request.ConvertTo<Badge>();
            return InTransaction(db =>
            {
                Logic.Checkin(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CreateAndCheckoutBadge request)
        {
            var entity = request.ConvertTo<Badge>();
            return InTransaction(db =>
            {
                Logic.CreateAndCheckout(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        #endregion

        #region Endpoints - Specific
        
        #endregion
    }

    #region Specific
    
    #endregion

    #region Generic Read Only
    [Route("/Badge", "GET")]
    public class GetAllBadges : GetAll<Badge> { }

    [Route("/Badge/{Id}", "GET")]
    public class GetBadgeById : GetSingleById<Badge> { }

    [Route("/Badge/GetSingleWhere", "GET")]
    [Route("/Badge/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetBadgeWhere : GetSingleWhere<Badge> { }

    [Route("/Badge/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedBadges : QueryDb<Badge>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion

    #region Generic Write
    [Route("/Badge/CreateInstance", "POST")]
    public class CreateBadgeInstance : Badge { }

    [Route("/Badge", "POST")]
    public class InsertBadge : Badge { }

    [Route("/Badge", "PUT")]
    public class UpdateBadge : Badge { }

    [Route("/Badge", "DELETE")]
    public class DeleteBadge : Badge { }

    [Route("/Badge/{Id}", "DELETE")]
    public class DeleteByIdBadge : Badge { }
    #endregion

    #region Generic Documents
    [Route("/Badge/MakeRevision", "POST")]
    public class MakeBadgeRevision : Badge { }

    [Route("/Badge/Checkout/{Id}", "POST")]
    public class CheckoutBadge : Badge { }

    [Route("/Badge/CancelCheckout/{Id}", "POST")]
    public class CancelCheckoutBadge : Badge { }

    [Route("/Badge/Checkin", "POST")]
    public class CheckinBadge : Badge { }

    [Route("/Badge/CreateAndCheckout", "POST")]
    public class CreateAndCheckoutBadge : Badge { }
    #endregion
}
