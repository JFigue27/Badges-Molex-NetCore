using Reusable.EmailServices;
using Reusable.Rest;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.Web;
using System.Data;

namespace Reusable.CRUD.Contract
{
    public interface ILogic
    {
        IRequest Request { get; set; }
        IDbConnection Db { get; set; }
        void Init(IDbConnection db, IAuthSession auth, IRequest request, IAppSettings appSettings);
        IEmailService EmailService { get; set; }
    }
}
