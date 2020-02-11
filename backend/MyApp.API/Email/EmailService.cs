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
    public class EmailService : BaseService<EmailLogic>
    {
        #region Endpoints - Generic Read Only
        public object Get(GetAllEmails request)
        {
            return WithDb(db => Logic.GetAll());
        }

        public object Get(GetEmailById request)
        {
            return WithDb(db => Logic.GetById(request.Id));
        }

        public object Get(GetEmailWhere request)
        {
            return WithDb(db => Logic.GetSingleWhere(request.Property, request.Value));
        }

        public object Get(GetPagedEmails request)
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
        public object Post(CreateEmailInstance request)
        {
            return WithDb(db =>
            {
                var entity = request.ConvertTo<Email>();
                return new HttpResult(new CommonResponse(Logic.CreateInstance(entity)))
                {
                    ResultScope = () => JsConfig.With(new Config { IncludeNullValues = true })
                };
            });
        }

        public object Post(InsertEmail request)
        {
            var entity = request.ConvertTo<Email>();
            return InTransaction(db =>
            {
                Logic.Add(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        public object Put(UpdateEmail request)
        {
            var entity = request.ConvertTo<Email>();
            return InTransaction(db =>
            {
                Logic.Update(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        public object Delete(DeleteEmail request)
        {
            var entity = request.ConvertTo<Email>();
            return InTransaction(db =>
            {
                Logic.Remove(entity);
                return new CommonResponse();
            });
        }
        public object Delete(DeleteByIdEmail request)
        {
            var entity = request.ConvertTo<Email>();
            return InTransaction(db =>
            {
                Logic.RemoveById(entity.Id);
                return new CommonResponse();
            });
        }
        #endregion

        #region Endpoints - Generic Document
        virtual public object Post(MakeEmailRevision request)
        {
            var entity = request.ConvertTo<Email>();
            return InTransaction(db =>
            {
                Logic.MakeRevision(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CheckoutEmail request)
        {
            var entity = request.ConvertTo<Email>();
            return InTransaction(db =>
            {
                Logic.Checkout(entity.Id);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CancelCheckoutEmail request)
        {
            var entity = request.ConvertTo<Email>();
            return InTransaction(db =>
            {
                Logic.CancelCheckout(entity.Id);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CheckinEmail request)
        {
            var entity = request.ConvertTo<Email>();
            return InTransaction(db =>
            {
                Logic.Checkin(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }

        virtual public object Post(CreateAndCheckoutEmail request)
        {
            var entity = request.ConvertTo<Email>();
            return InTransaction(db =>
            {
                Logic.CreateAndCheckout(entity);
                return new CommonResponse(Logic.GetById(entity.Id));
            });
        }
        #endregion

        #region Endpoints - Specific
        public object Any(TestEmail request)
        {
            WithDb(db => Logic.TestEmail(request.To));
            return "Sent Email Completed";
        }

        #endregion
    }

    #region Specific
    [Route("/Email/Test/{To}", "GET")]
    public class TestEmail
    {
        public string To { get; set; }
    }

    #endregion

    #region Generic Read Only
    [Route("/Email", "GET")]
    public class GetAllEmails : GetAll<Email> { }

    [Route("/Email/{Id}", "GET")]
    public class GetEmailById : GetSingleById<Email> { }

    [Route("/Email/GetSingleWhere", "GET")]
    [Route("/Email/GetSingleWhere/{Property}/{Value}", "GET")]
    public class GetEmailWhere : GetSingleWhere<Email> { }

    [Route("/Email/GetPaged/{Limit}/{Page}", "GET")]
    public class GetPagedEmails : QueryDb<Email>
    {
        public string FilterGeneral { get; set; }
        //public long? FilterUser { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }

        public bool RequiresKeysInJsons { get; set; }
    }
    #endregion

    #region Generic Write
    [Route("/Email/CreateInstance", "POST")]
    public class CreateEmailInstance : Email { }

    [Route("/Email", "POST")]
    public class InsertEmail : Email { }

    [Route("/Email", "PUT")]
    public class UpdateEmail : Email { }

    [Route("/Email", "DELETE")]
    public class DeleteEmail : Email { }

    [Route("/Email/{Id}", "DELETE")]
    public class DeleteByIdEmail : Email { }
    #endregion

    #region Generic Documents
    [Route("/Email/MakeRevision", "POST")]
    public class MakeEmailRevision : Email { }

    [Route("/Email/Checkout/{Id}", "POST")]
    public class CheckoutEmail : Email { }

    [Route("/Email/CancelCheckout/{Id}", "POST")]
    public class CancelCheckoutEmail : Email { }

    [Route("/Email/Checkin", "POST")]
    public class CheckinEmail : Email { }

    [Route("/Email/CreateAndCheckout", "POST")]
    public class CreateAndCheckoutEmail : Email { }
    #endregion
}
