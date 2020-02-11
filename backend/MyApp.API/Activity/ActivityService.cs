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
    public class ActivityService : BaseService<ActivityLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllActivitys request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetActivityById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetActivityWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedActivitys request)
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
        public object Post(CreateActivityInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<Activity>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertActivity request)
        {
            var entity = request.ConvertTo<Activity>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateActivity request)
        {
            var entity = request.ConvertTo<Activity>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteActivity request)
        {
            var entity = request.ConvertTo<Activity>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdActivity request)
        {
            var entity = request.ConvertTo<Activity>();
            return InTransaction(db =>
            {
                Logic.RemoveById(entity.Id);
                return new CommonResponse();
            });
        }
        #endregion

        #region Endpoints - Generic Document
        virtual public object Post(MakeActivityRevision request)
        {
            var entity = request.ConvertTo<Activity>();
            return InTransaction(db =>
            {
                Logic.MakeRevision(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CheckoutActivity request)
        {
            var entity = request.ConvertTo<Activity>();
            return InTransaction(db =>
            {
                Logic.Checkout(entity.Id);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CancelCheckoutActivity request)
        {
            var entity = request.ConvertTo<Activity>();
            return InTransaction(db =>
            {
                Logic.CancelCheckout(entity.Id);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CheckinActivity request)
        {
            var entity = request.ConvertTo<Activity>();
            return InTransaction(db =>
            {
                Logic.Checkin(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CreateAndCheckoutActivity request)
        {
            var entity = request.ConvertTo<Activity>();
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
    [Route("/Activity", "GET")]
    public class GetAllActivitys : GetAll<Activity> { }

    [Route("/Activity/{Id}", "GET")]
    public class GetActivityById : GetSingleById<Activity> { }

    [Route("/Activity/GetSingleWhere", "GET")]
    [Route("/Activity/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetActivityWhere : GetSingleWhere<Activity> { }

    [Route("/Activity/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedActivitys : QueryDb<Activity>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion

    #region Generic Write
    [Route("/Activity/CreateInstance", "POST")]
    public class CreateActivityInstance : Activity { }

    [Route("/Activity", "POST")]
    public class InsertActivity : Activity { }

    [Route("/Activity", "PUT")]
    public class UpdateActivity : Activity { }

    [Route("/Activity", "DELETE")]
    public class DeleteActivity : Activity { }

    [Route("/Activity/{Id}", "DELETE")]
    public class DeleteByIdActivity : Activity { }
    #endregion

    #region Generic Documents
    [Route("/Activity/MakeRevision", "POST")]
    public class MakeActivityRevision : Activity { }

    [Route("/Activity/Checkout/{Id}", "POST")]
    public class CheckoutActivity : Activity { }

    [Route("/Activity/CancelCheckout/{Id}", "POST")]
    public class CancelCheckoutActivity : Activity { }

    [Route("/Activity/Checkin", "POST")]
    public class CheckinActivity : Activity { }

    [Route("/Activity/CreateAndCheckout", "POST")]
    public class CreateAndCheckoutActivity : Activity { }
    #endregion
}
