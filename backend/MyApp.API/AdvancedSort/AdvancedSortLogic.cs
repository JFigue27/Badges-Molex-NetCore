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
    public class AdvancedSortLogic : WriteLogic<AdvancedSort>, ILogicWriteAsync<AdvancedSort>
    {
        
        
        

        
        protected override SqlExpression<AdvancedSort> OnGetList(SqlExpression<AdvancedSort> query)
        {
            query.LeftJoin<SortData>()
                .LeftJoin<FilterData>();


            return base.OnGetList(query);
        }
        

        
        protected override SqlExpression<AdvancedSort> OnGetSingle(SqlExpression<AdvancedSort> query)
        {
            query.Where(e => e.UserName == Auth.UserName);

            return base.OnGetSingle(query);
        }
        

        
        protected override void OnBeforeSaving(AdvancedSort entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            entity.UserName = Auth.UserName;

            foreach (var item in entity.Sorting)
            {
                item.AdvancedSortId = entity.Id;
            }
            Db.SaveAll(entity.Sorting);

            foreach (var item in entity.Filtering)
            {
                item.AdvancedSortId = entity.Id;
            }
            Db.SaveAll(entity.Filtering);

            if (mode == OPERATION_MODE.UPDATE)
            {
                var originalEntity = GetById(entity.Id);

                for (int i = originalEntity.Sorting.Count - 1; i >= 0; i--)
                {
                    var oSort = originalEntity.Sorting[i];
                    if (!entity.Sorting.Any(e => e.Id == oSort.Id))
                    {
                        Db.Delete(oSort);
                    }
                }

                for (int i = originalEntity.Filtering.Count - 1; i >= 0; i--)
                {
                    var oFilter = originalEntity.Filtering[i];
                    if (!entity.Filtering.Any(e => e.Id == oFilter.Id))
                    {
                        Db.Delete(oFilter);
                    }
                }
            }

        }
        

        

        

        
        protected override List<AdvancedSort> AdapterOut(params AdvancedSort[] entities)
        {
            foreach (var item in entities)
            {
                item.Sorting = item.Sorting.OrderBy(e => e.Sequence).ToList();
            }

            return entities.ToList();
        }
        

        
    }
}
