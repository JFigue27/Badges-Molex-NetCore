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
    public class FilterDataLogic : WriteLogic<FilterData>, ILogicWriteAsync<FilterData>
    {
        
        

        

        protected override FilterData OnCreateInstance(FilterData entity)
        {
            
            

            return entity;
        }

        protected override SqlExpression<FilterData> OnGetList(SqlExpression<FilterData> query)
        {
            
            

            return base.OnGetList(query);
        }

        protected override SqlExpression<FilterData> OnGetSingle(SqlExpression<FilterData> query)
        {
            
            

            return base.OnGetSingle(query);
        }

        protected override void OnBeforeSaving(FilterData entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            
            
        }

        protected override void OnAfterSaving(FilterData entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            
            
        }

        protected override void OnBeforeRemoving(FilterData entity)
        {
            
            
        }

        protected override List<FilterData> AdapterOut(params FilterData[] entities)
        {
            

            foreach (var item in entities)
            {
                
            }

            return entities.ToList();
        }

        protected override void OnFinalize(FilterData entity)
        {
            
        }

        protected override void OnUnfinalize(FilterData entity)
        {
            
        }

        
        
    }
}
