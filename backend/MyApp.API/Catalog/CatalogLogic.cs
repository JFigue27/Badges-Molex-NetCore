using MyApp.Logic.Entities;
using Reusable.Attachments;
using Reusable.CRUD.Contract;
using Reusable.CRUD.Entities;
using Reusable.CRUD.Implementations.SS;
using Reusable.CRUD.JsonEntities;
using Reusable.EmailServices;
using Reusable.Rest;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
namespace MyApp.Logic
{
    public class CatalogLogic : WriteLogic<Catalog>, ILogicWriteAsync<Catalog>
    {
        public CatalogFieldValueLogic FieldValueLogic { get; set; }
        public override void Init(IDbConnection db, IAuthSession auth, IRequest request, IAppSettings appSettings)
        {
            base.Init(db, auth, request, appSettings);
            FieldValueLogic.Init(db, auth, request, appSettings);
        }
        protected override SqlExpression<Catalog> OnGetList(SqlExpression<Catalog> query)
        {
            var catalogDefinitionName = Request.QueryString["CatalogDefinition"];
            if (IsValidJSValue(catalogDefinitionName))
            {
                var catalogDefinition = Db.Single<CatalogDefinition>(e => e.Name == catalogDefinitionName);
                if (catalogDefinition != null)
                    query
                        .Join<CatalogDefinition>()
                        .Where<CatalogDefinition>(e => e.Name == catalogDefinitionName);
                else
                    throw new Exception($"Catalog does not exist: [{catalogDefinitionName}]");
            }

            return query;
        }
        protected override bool BeforeSearch(List<Catalog> entities)
        {
            AdapterOut(entities.ToArray());

            var whereField = Request.QueryString["WhereField"];
            var whereValue = Request.QueryString["WhereValue"];
            if (IsValidJSValue(whereField) && IsValidJSValue(whereValue))
            {
                entities.RemoveAll(item => item.Values == null || !item.Values.Any(e => e.Field.FieldName == whereField && e.Value == whereValue));
            }

            return true;
        }
        protected override void OnAfterSaving(Catalog entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            if (entity.Values != null)
            {
                entity.Values.ForEach(e => e.CatalogId = entity.Id);
                //Upserts:
                Db.SaveAll(entity.Values.Where(e => e.Entry_State == BaseEntity.EntryState.Upserted));
            }
            Cache.FlushAll();
        }
        protected override List<Catalog> AdapterOut(params Catalog[] entities)
        {
            foreach (var item in entities)
            {
                item.Values?.ForEach(v =>
                {
                    if (v.FieldId > 0)
                    {
                        v.Field = Db.SingleById<Field>(v.FieldId);

                        if (v.Field.FieldType == "Relationship: Has One")
                        {
                            v.Value = Db.SingleById<Catalog>(v.IdsList[0]).Value;
                        }
                        else if (v.Field.FieldType == "Relationship: Has Many")
                        {
                            v.Value = Db.SelectByIds<Catalog>(v.IdsList).Select(e => e.Value).Join(", ");
                        }
                    }
                });
            }
            return base.AdapterOut(entities);
        }
    }
}
