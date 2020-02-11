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




namespace MyApp.Logic
{
    public class ActivityLogic : DocumentLogic<Activity>, IDocumentLogicAsync<Activity>
    {
        
        

        

        protected override Activity OnCreateInstance(Activity entity)
        {
            
            

            return entity;
        }

        protected override SqlExpression<Activity> OnGetList(SqlExpression<Activity> query)
        {
            
            

            return base.OnGetList(query);
        }

        protected override SqlExpression<Activity> OnGetSingle(SqlExpression<Activity> query)
        {
            
            

            return base.OnGetSingle(query);
        }

        protected override void OnBeforeSaving(Activity entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            
            
        }

        protected override void OnAfterSaving(Activity entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            
            
        }

        protected override void OnBeforeRemoving(Activity entity)
        {
            
            
        }

        protected override List<Activity> AdapterOut(params Activity[] entities)
        {
            

            foreach (var item in entities)
            {
                
            }

            return entities.ToList();
        }

        protected override void OnFinalize(Activity entity)
        {
            
        }

        protected override void OnUnfinalize(Activity entity)
        {
            
        }

        
        
    }
}
