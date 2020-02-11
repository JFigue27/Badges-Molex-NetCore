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
using ServiceStack.OrmLite;
using ServiceStack.Text;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

///start:slot:imports<<<///end:slot:imports<<<

namespace MyApp.Logic
{
    public class CatalogLogic : WriteLogic<Catalog>, ILogicWriteAsync<Catalog>
    {
        
        ///start:slot:init<<<///end:slot:init<<<

        ///start:slot:ctor<<<///end:slot:ctor<<<

        protected override Catalog OnCreateInstance(Catalog entity)
        {
            
            ///start:slot:createInstance<<<///end:slot:createInstance<<<

            return entity;
        }

        protected override SqlExpression<Catalog> OnGetList(SqlExpression<Catalog> query)
        {
            var name = Request.QueryString["name"];
            if (IsValidJSValue(name))
                query.Where(e => e.CatalogType == name);

            ///start:slot:listQuery<<<///end:slot:listQuery<<<

            return query;
        }

        protected override SqlExpression<Catalog> OnGetSingle(SqlExpression<Catalog> query)
        {
            
            ///start:slot:singleQuery<<<///end:slot:singleQuery<<<

            return query;
        }

        protected override void OnBeforeSaving(Catalog entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            if (string.IsNullOrWhiteSpace(entity.CatalogType))
                throw new KnownError("Error. Catalog Type is a required field.");
            if (mode == OPERATION_MODE.UPDATE)
            {
                var original = GetById(entity.Id);
                if (original == null) throw new KnownError("Error. Entity no longer exists.");

                if (original.Value != entity.Value)
                {
                    Db.Update<Catalog>(new { Parent = entity.Value }, e => e.Parent == original.Value);
                }
            }

            ///start:slot:beforeSave<<<///end:slot:beforeSave<<<
        }

        protected override void OnAfterSaving(Catalog entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            Cache.FlushAll();
            ///start:slot:afterSave<<<///end:slot:afterSave<<<
        }

        protected override void OnBeforeRemoving(Catalog entity)
        {
            
            ///start:slot:beforeRemove<<<///end:slot:beforeRemove<<<
        }

        protected override List<Catalog> AdapterOut(params Catalog[] entities)
        {
            ///start:slot:adapterOut<<<///end:slot:adapterOut<<<

            foreach (var item in entities)
            {
                
            }

            return entities.ToList();
        }

        public List<string> GetOnlyValues(string catalogType, SqlExpression<Catalog> query = null)
        {
            var allEntries = GetAll().Where(e => e.CatalogType == catalogType);
            return allEntries.Select(e => e.Value).ToList();
        }

        ///start:slot:logic<<<///end:slot:logic<<<
    }
}
