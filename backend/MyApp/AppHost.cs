using Funq;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.Data;
using System.Collections.Generic;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Api.OpenApi;
using Reusable.CRUD.Contract;
using Reusable.Rest.Implementations.SS;
using Reusable.CRUD.Implementations.SS.Logic;
using MyApp.Logic;
using MyApp.API;
using ServiceStack.Text;
using ServiceStack.Configuration;
using Reusable.EmailServices;

namespace MyApp
{
    //VS.NET Template Info: https://servicestack.net/vs-templates/EmptyAspNet
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Base constructor requires a Name and Assembly where web service implementation is located
        /// </summary>
        public AppHost()
            : base("MyApp",
                  typeof(PingService).Assembly,
                  typeof(MyServices).Assembly)
        {
            Licensing.RegisterLicenseFromFileIfExists("~/ss_license.txt".MapHostAbsolutePath());
        }

        private IAuthSession SessionFactory()
        {
            return new AuthUserSession();
        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                //EnableFeatures = Feature.All.Remove(Feature.Metadata),
                //DebugMode = false
            });

            JsConfig.IncludeNullValues = false;
            JsConfig.ExcludeTypeInfo = true;
            JsConfig.DateHandler = DateHandler.ISO8601;

            #region Database
            var dbFactory = new OrmLiteConnectionFactory(
                AppSettings.Get("dbConnectionString", ""), SqlServer2008Dialect.Provider);

            container.Register<IDbConnectionFactory>(dbFactory);

            OrmLiteConfig.StringFilter = s => s.Trim();
            #endregion

            #region Plugins
            Plugins.Add(new CorsFeature(
                allowedHeaders: "Content-Type, Allow, Authorization"));

            Plugins.Add(new OpenApiFeature()
            {
                ApiDeclarationFilter = declaration =>
                {
                    declaration.Info.Title = "Badges Molex Net Core";
                    //declaration.Info.Contact = new ServiceStack.Api.OpenApi.Specification.OpenApiContact()
                    //{
                    //    Email = "apacheco@capsonic.com",
                    //    Name = "Alfredo Pacheco"
                    //};
                    declaration.Info.Description = "";
                },
                OperationFilter = (verb, op) =>
                {
                    switch (verb)
                    {
                        case "POST":
                            op.Parameters.RemoveAll(p => p.Name == "Id");
                            op.Parameters.RemoveAll(p => p.Name == "RowVersion");
                            break;
                        default:
                            break;
                    }
                    op.Parameters.RemoveAll(p => p.Name == "EntityName");
                    op.Parameters.RemoveAll(p => p.Name == "EF_State");
                }
            });

            Plugins.Add(new AutoQueryFeature
            {
                MaxLimit = 100
            });
            #endregion

            #region Auth
            var authProviders = new List<IAuthProvider>
            {
                new JwtAuthProvider(AppSettings) {
                    RequireSecureConnection = false,
                    AllowInQueryString = true
                },
                new CredentialsAuthProvider()
            };

            var authFeature = new AuthFeature(SessionFactory, authProviders.ToArray());
            Plugins.Add(authFeature);

            //var authRepo = new OrmLiteAuthRepository(dbFactory);
            //container.Register<IUserAuthRepository>(authRepo);

            //authRepo.InitSchema();

            //Plugins.Add(new RegistrationFeature());

            //var admin = authRepo.GetUserAuthByUserName("admin");
            //if (admin == null)
            //    authRepo.CreateUserAuth(new UserAuth
            //    {
            //        UserName = "admin",
            //        Roles = new List<string> { RoleNames.Admin }
            //    }, "admin");
            #endregion

            //TODO:
            //Done. Global Response Filter: CommonResponse.
            //Done. Cache.
            //Done. Transactions.
            //Logging.
            //Batched requests.
            //attachments
            //Profiler.
            //Versioning.
            //Compression.
            //Autoquery.
            //stripe.com

            #region Cache
            //container.Register<ICacheClient>(new MemoryCacheClient());
            #endregion

            #region App
            //container.Register(c => dbFactory.Open());
            //container.Register(c => c.Resolve<IDbConnectionFactory>().OpenDbConnection()).ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<RevisionLogic>().ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<CatalogLogic>().ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<CatalogTypeLogic>().ReusedWithin(ReuseScope.Request);
            container.Register<IEmailService>(i => new MailgunService()).ReusedWithin(ReuseScope.Request);

            //This App:
            ///start:generated:di<<<
            container.RegisterAutoWired<ActivityLogic>().ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<ApprovalLogic>().ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<EmailLogic>().ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<AdvancedSortLogic>().ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<ApplicationTaskLogic>().ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<FilterDataLogic>().ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<SortDataLogic>().ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<TokenLogic>().ReusedWithin(ReuseScope.Request);
            ///end:generated:di<<<
            #endregion
        }
    }
}
