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
    public class CatalogDefinitionLogic : WriteLogic<CatalogDefinition>, ILogicWriteAsync<CatalogDefinition>
    {
        public FieldLogic FieldLogic { get; set; }
        public override void Init(IDbConnection db, IAuthSession auth, IRequest request, IAppSettings appSettings)
        {
            base.Init(db, auth, request, appSettings);
            FieldLogic.Init(db, auth, request, appSettings);
        }

        protected override SqlExpression<CatalogDefinition> OnGetList(SqlExpression<CatalogDefinition> query)
        {
            //query.LeftJoin<Field>()
            //    .CustomJoin($"LEFT JOIN {query.Table<CatalogDefinition>()} B ON (B.Id = {query.Table<Field>()}.{query.Column<Field>(e => e.ForeignId)})");
            //query.LeftJoin<Field>()
            //    .LeftJoin<Field, CatalogDefinition>((field, b) => field.ForeignId == b.Id, Db.TableAlias("B"));

            return query;
        }

        protected override void OnBeforeSaving(CatalogDefinition entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            if (string.IsNullOrWhiteSpace(entity.Name)) throw new KnownError("Invalid Name.");

            //if (mode == OPERATION_MODE.UPDATE)
            //{
            //    var original = GetById(entity.Id);
            //    if (original == null) throw new KnownError("Error. Entity no longer exists.");

            //    foreach (var field in entity.Fields)
            //    {
            //        //Delete
            //        if (field.Entry_State == BaseEntity.EntryState.Deleted)
            //        {
            //            //TODO: To delete in Catalog as well?
            //        }

            //        //Update:
            //        if (field.Entry_State == BaseEntity.EntryState.Upserted && field.Id > 0)
            //        {

            //        }
            //    }
            //}

            ////Update or New, we always check relationships:
            //if (field.Relationship == "Has Parent")
            //{

            //    Db.Update<CatalogDefinition>(new { ParentType = entity.Name }, e => e.ParentType == original.Name);
            //}

        }
        protected override void OnAfterSaving(CatalogDefinition entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            if (entity.Fields != null)
            {
                entity.Fields.ForEach(e => e.CatalogDefinitionId = entity.Id);
                //Deletes:
                Db.DeleteAll(entity.Fields.Where(e => e.Entry_State == BaseEntity.EntryState.Deleted));
                //Inserts and Updates:
                Db.SaveAll(entity.Fields.Where(e => e.Entry_State == BaseEntity.EntryState.Upserted));
            }

            Cache.FlushAll();
        }
        protected override List<CatalogDefinition> AdapterOut(params CatalogDefinition[] entities)
        {
            foreach (var item in entities)
            {
                item.Fields?.ForEach(r =>
                {
                    //Cannot use GetById because AdapterOut causes circular reference:
                    if (r.ForeignId > 0) r.Foreign = Db.SingleById<CatalogDefinition>(r.ForeignId);
                });

                //item.Fields = FieldLogic
                //    .GetPaged(query: Db.From<Field>()
                //        .Where(e => e.CatalogDefinitionId == item.Id), cacheKey: "CatalogDefinitionId=" + item.Id)
                //    .Result as List<Field>;

                //item.CatalogDefinitionRelationships = CatalogDefinitionRelationshipLogic
                //    .GetPaged(query: Db.From<CatalogDefinitionRelationship>()
                //        .Where(e => e.CatalogDefinitionId == item.Id), cacheKey: "CatalogDefinitionId=" + item.Id)
                //    .Result as List<CatalogDefinitionRelationship>;

            }
            return entities.ToList();
        }
    }
}
