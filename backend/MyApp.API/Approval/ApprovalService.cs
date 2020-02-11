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
    public class ApprovalService : BaseService<ApprovalLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllApprovals request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetApprovalById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetApprovalWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedApprovals request)
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
        public object Post(CreateApprovalInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<Approval>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertApproval request)
        {
            var entity = request.ConvertTo<Approval>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateApproval request)
        {
            var entity = request.ConvertTo<Approval>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteApproval request)
        {
            var entity = request.ConvertTo<Approval>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdApproval request)
        {
            var entity = request.ConvertTo<Approval>();
            return InTransaction(db =>
            {
                Logic.RemoveById(entity.Id);
                return new CommonResponse();
            });
        }
        #endregion

        #region Endpoints - Generic Document
        virtual public object Post(MakeApprovalRevision request)
        {
            var entity = request.ConvertTo<Approval>();
            return InTransaction(db =>
            {
                Logic.MakeRevision(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CheckoutApproval request)
        {
            var entity = request.ConvertTo<Approval>();
            return InTransaction(db =>
            {
                Logic.Checkout(entity.Id);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CancelCheckoutApproval request)
        {
            var entity = request.ConvertTo<Approval>();
            return InTransaction(db =>
            {
                Logic.CancelCheckout(entity.Id);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CheckinApproval request)
        {
            var entity = request.ConvertTo<Approval>();
            return InTransaction(db =>
            {
                Logic.Checkin(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CreateAndCheckoutApproval request)
        {
            var entity = request.ConvertTo<Approval>();
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
    [Route("/Approval", "GET")]
    public class GetAllApprovals : GetAll<Approval> { }

    [Route("/Approval/{Id}", "GET")]
    public class GetApprovalById : GetSingleById<Approval> { }

    [Route("/Approval/GetSingleWhere", "GET")]
    [Route("/Approval/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetApprovalWhere : GetSingleWhere<Approval> { }

    [Route("/Approval/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedApprovals : QueryDb<Approval>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion

    #region Generic Write
    [Route("/Approval/CreateInstance", "POST")]
    public class CreateApprovalInstance : Approval { }

    [Route("/Approval", "POST")]
    public class InsertApproval : Approval { }

    [Route("/Approval", "PUT")]
    public class UpdateApproval : Approval { }

    [Route("/Approval", "DELETE")]
    public class DeleteApproval : Approval { }

    [Route("/Approval/{Id}", "DELETE")]
    public class DeleteByIdApproval : Approval { }
    #endregion

    #region Generic Documents
    [Route("/Approval/MakeRevision", "POST")]
    public class MakeApprovalRevision : Approval { }

    [Route("/Approval/Checkout/{Id}", "POST")]
    public class CheckoutApproval : Approval { }

    [Route("/Approval/CancelCheckout/{Id}", "POST")]
    public class CancelCheckoutApproval : Approval { }

    [Route("/Approval/Checkin", "POST")]
    public class CheckinApproval : Approval { }

    [Route("/Approval/CreateAndCheckout", "POST")]
    public class CreateAndCheckoutApproval : Approval { }
    #endregion
}
