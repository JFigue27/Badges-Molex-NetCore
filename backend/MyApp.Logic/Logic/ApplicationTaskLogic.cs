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
    public class ApplicationTaskLogic : WriteLogic<ApplicationTask>, ILogicWriteAsync<ApplicationTask>
    {
        
        

        

        protected override ApplicationTask OnCreateInstance(ApplicationTask entity)
        {
            
            

            return entity;
        }

        protected override SqlExpression<ApplicationTask> OnGetList(SqlExpression<ApplicationTask> query)
        {
            var sAdvancedSortName = Request.QueryString["advanced-sort"];
            if (sAdvancedSortName != null)
            {
                var advancedSort = Db.Single(Db.From<AdvancedSort>()
                    .LeftJoin<FilterData>()
                    .LeftJoin<SortData>()
                    .Where(e => e.UserName == Auth.UserName && e.Name == sAdvancedSortName));

                if (advancedSort != null)
                {
                    #region Filtering
                    //Status
                    var filterStatus = advancedSort.Filtering.FirstOrDefault(e => e.Key == "Status");
                    if (filterStatus != null && !string.IsNullOrWhiteSpace(filterStatus.Value))
                    {
                        var list = filterStatus.Value.FromJson<List<BaseCatalog>>();
                        if (list.Count > 0)
                        {
                            query.Where(task => Db.Exists(Db.From<AdvancedSort, FilterData>((a, f) => a.Id == f.AdvancedSortId)
                                .Where(advSort => advSort.UserName == Auth.UserName && advSort.Name == sAdvancedSortName)
                                .And<FilterData>(f => f.Key == "Status" && f.Value.Contains(task.Status.ToString()))));
                        }
                    }

                    //Category
                    var filterCategory = advancedSort.Filtering.FirstOrDefault(e => e.Key == "Category");
                    if (filterCategory != null && !string.IsNullOrWhiteSpace(filterCategory.Value))
                    {
                        var list = filterCategory.Value.FromJson<List<BaseCatalog>>();
                        if (list.Count > 0)
                        {
                            query.Where(task => Db.Exists(Db.From<AdvancedSort, FilterData>((a, f) => a.Id == f.AdvancedSortId)
                                .Where(advSort => advSort.UserName == Auth.UserName && advSort.Name == sAdvancedSortName)
                                .And<FilterData>(f => f.Key == "Category" && f.Value.Contains(task.Status.ToString()))));
                        }
                    }

                    //Priority
                    var filterPriority = advancedSort.Filtering.FirstOrDefault(e => e.Key == "Priority");
                    if (filterPriority != null && !string.IsNullOrWhiteSpace(filterPriority.Value))
                    {
                        var list = filterPriority.Value.FromJson<List<BaseCatalog>>();
                        if (list.Count > 0)
                        {
                            query.Where(task => Db.Exists(Db.From<AdvancedSort, FilterData>((a, f) => a.Id == f.AdvancedSortId)
                                .Where(advSort => advSort.UserName == Auth.UserName && advSort.Name == sAdvancedSortName)
                                .And<FilterData>(f => f.Key == "Priority" && f.Value.Contains(task.Status.ToString()))));
                        }
                    }

                    //CreatedBy
                    var filterCreatedBy = advancedSort.Filtering.FirstOrDefault(e => e.Key == "CreatedBy");
                    if (filterCreatedBy != null && !string.IsNullOrWhiteSpace(filterCreatedBy.Value))
                    {
                        var list = filterCreatedBy.Value.FromJson<List<BaseCatalog>>();
                        if (list.Count > 0)
                        {
                            query.Where(task => Db.Exists(Db.From<AdvancedSort, FilterData>((a, f) => a.Id == f.AdvancedSortId)
                                .Where(advSort => advSort.UserName == Auth.UserName && advSort.Name == sAdvancedSortName)
                                .And<FilterData>(f => f.Key == "CreatedBy" && f.Value.Contains("\"UserName\":" + task.CreatedBy + ","))));
                        }
                    }

                    //AssignedTo
                    var filterAssignedTo = advancedSort.Filtering.FirstOrDefault(e => e.Key == "AssignedTo");
                    if (filterAssignedTo != null && !string.IsNullOrWhiteSpace(filterAssignedTo.Value))
                    {
                        var list = filterAssignedTo.Value.FromJson<List<BaseCatalog>>();
                        if (list.Count > 0)
                        {
                            query.Where(task => Db.Exists(Db.From<AdvancedSort, FilterData>((a, f) => a.Id == f.AdvancedSortId)
                                .Where(advSort => advSort.UserName == Auth.UserName && advSort.Name == sAdvancedSortName)
                                .And<FilterData>(f => f.Key == "AssignedTo" && f.Value.Contains("\"UserName\":" + task.AssignedTo + ","))));
                        }
                    }

                    //ClosedBy
                    var filterClosedBy = advancedSort.Filtering.FirstOrDefault(e => e.Key == "ClosedBy");
                    if (filterClosedBy != null && !string.IsNullOrWhiteSpace(filterClosedBy.Value))
                    {
                        var list = filterClosedBy.Value.FromJson<List<BaseCatalog>>();
                        if (list.Count > 0)
                        {
                            query.Where(task => Db.Exists(Db.From<AdvancedSort, FilterData>((a, f) => a.Id == f.AdvancedSortId)
                                .Where(advSort => advSort.UserName == Auth.UserName && advSort.Name == sAdvancedSortName)
                                .And<FilterData>(f => f.Key == "ClosedBy" && f.Value.Contains("\"UserName\":" + task.ClosedBy + ","))));
                        }
                    }
                    #endregion

                    #region Sorting
                    advancedSort.Sorting = advancedSort.Sorting.OrderBy(s => s.Sequence).ToList();
                    query.OrderBy(e => 0);

                    foreach (var sort in advancedSort.Sorting)
                    {
                        OrderBy(query, sort);
                    }
                    #endregion
                }
            }
            query.Where(e => !e.IsCanceled);

            

            return base.OnGetList(query);
        }

        protected override SqlExpression<ApplicationTask> OnGetSingle(SqlExpression<ApplicationTask> query)
        {
            
            

            return base.OnGetSingle(query);
        }

        protected override void OnBeforeSaving(ApplicationTask entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            
            
        }

        protected override void OnAfterSaving(ApplicationTask entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            
            
        }

        protected override void OnBeforeRemoving(ApplicationTask entity)
        {
            
            
        }

        protected override List<ApplicationTask> AdapterOut(params ApplicationTask[] entities)
        {
            

            foreach (var item in entities)
            {
                
            }

            return entities.ToList();
        }

        protected override void OnFinalize(ApplicationTask entity)
        {
            
        }

        protected override void OnUnfinalize(ApplicationTask entity)
        {
            
        }

        private SqlExpression<ApplicationTask> OrderBy(SqlExpression<ApplicationTask> query, SortData sort)
        {
            switch (sort.Value)
            {
                case "Status":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.Status.ToString());
                    }
                    else
                    {
                        query = query.ThenBy(e => e.Status.ToString());
                    }
                    break;
                case "Created By":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.CreatedBy);
                    }
                    else
                    {
                        query = query.ThenBy(e => e.CreatedBy);
                    }
                    break;
                case "Assigned To":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.AssignedTo);
                    }
                    else
                    {
                        query = query.ThenBy(e => e.AssignedTo);
                    }
                    break;
                case "Priority":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.Priority.ToString());
                    }
                    else
                    {
                        query = query.ThenBy(e => e.Priority.ToString());
                    }
                    break;
                case "Category":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.Category);
                    }
                    else
                    {
                        query = query.ThenBy(e => e.Category);
                    }
                    break;
                case "Title":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.Title);
                    }
                    else
                    {
                        query = query.ThenBy(e => e.Title);
                    }
                    break;
                case "Description":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.Description);
                    }
                    else
                    {
                        query = query.ThenBy(e => e.Description);
                    }
                    break;
                case "Closed By":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.ClosedBy);
                    }
                    else
                    {
                        query = query.ThenBy(e => e.ClosedBy);
                    }
                    break;
                case "Date Created At":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.CreatedAt);
                    }
                    else
                    {
                        query = query.ThenBy(e => e.CreatedAt);
                    }
                    break;
                case "Date Due Date":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.DueDate);
                    }
                    else
                    {
                        query = query.ThenBy(e => e.DueDate);
                    }
                    break;
                case "Date Closed":
                    if (sort.AscDesc == "DESC")
                    {
                        query = query.ThenByDescending(e => e.ClosedAt);
                    }
                    else
                    {
                        query = query.ThenBy(e => e.ClosedAt);
                    }
                    break;
                default:
                    break;
            }

            return query;
        }

        public void SaveAll(ApplicationTask template, List<Contact> asignees)
        {
            var existingTasks = GetAll()
                .Where(e => e.ForeignApp == template.ForeignApp
                        && e.ForeignType == template.ForeignType
                        && e.ForeignKey == template.ForeignKey
                        && !e.IsCanceled)
                .ToList();

            #region No Asignees
            //Also compare count for performance purposes so we don't GetAll Users.
            if (asignees == null && asignees.Count == 0)
            {
                foreach (var task in existingTasks)
                {
                    task.IsCanceled = true;
                    Update(task);
                }
                return;
            }
            #endregion

            #region Existing Tasks / Update
            foreach (var task in existingTasks)
            {
                var asignee = asignees.FirstOrDefault(e => e.UserName.ToLower() == task.AssignedTo.ToLower());
                if (asignee != null)
                {
                    //Update Tasks if already exists for user
                    var taskToUpdate = template.ConvertTo<ApplicationTask>();
                    taskToUpdate.Id = task.Id;
                    Update(task);
                }
                else
                {
                    //Cancel ApplicationTask when user is removed from Approvers
                    task.IsCanceled = true;
                    Db.Update<ApplicationTask>(new { IsCancelled = true }, e => e.Id == task.Id);
                }
                //Cancel ApplicationTask when user no longer exists
                //TODO
            }
            #endregion

            #region New Asignees
            //Create ApplicationTask only if it is not already created for user
            foreach (var asignee in asignees)
                if (!existingTasks.Any(t => t.AssignedTo.ToLower() == asignee.UserName.ToLower()))
                {
                    var taskToAdd = template.ConvertTo<ApplicationTask>(); //clone
                    taskToAdd.AssignedTo = asignee.UserName;
                    Add(taskToAdd);
                }
            #endregion
        }

        public void SaveSingle(ApplicationTask template)
        {
            if (string.IsNullOrWhiteSpace(template.AssignedTo))
                throw new KnownError("Error. Cannot save task. AssignedTo is a required field.");

            var task = GetAll()
                .Where(e => e.ForeignApp == template.ForeignApp
                    && e.ForeignType == template.ForeignType
                    && e.ForeignKey == template.ForeignKey
                    && !e.IsCanceled)
                .Where(e => e.AssignedTo.ToLower() == template.AssignedTo.ToLower())
                .FirstOrDefault();

            if (task == null)
                Add(template);
            else
            {
                template.Id = task.Id;
                Update(template);
            }
        }

        public void Done(ApplicationTask template)
        {
            if (string.IsNullOrWhiteSpace(template.AssignedTo))
                throw new KnownError("Error. Cannot save task. AssignedTo is a required field.");

            var task = GetAll()
                .Where(e => e.ForeignApp == template.ForeignApp
                    && e.ForeignType == template.ForeignType
                    && e.ForeignKey == template.ForeignKey
                    && !e.IsCanceled)
                .Where(e => e.AssignedTo.ToLower() == template.AssignedTo.ToLower())
                .FirstOrDefault();

            if (task == null)
            {
                throw new KnownError("ApplicationTask not found.");
                // template.ClosedAt = DateTimeOffset.Now;
                // template.ClosedBy = Auth.UserName;
                //Missing other foreign fields.

                // Add(template);
            }
            else
            {
                task.Status = string.IsNullOrWhiteSpace(template.Status) ? "COMPLETED" : template.Status;
                task.ClosedAt = DateTimeOffset.Now;
                task.ClosedBy = Auth.UserName;

                Update(task);
            }
        }

        //Cancel ApplicationTask for all users:
        public void CancelAllFromForeign(BaseEntity foreign, string foreignType, string foreignApp)
        {
            var tasks = Db.Select<ApplicationTask>(t => !t.IsCanceled
                            && t.ForeignKey == foreign.Id
                            && t.ForeignType == foreignType
                            && t.ForeignApp == foreignApp);

            foreach (var task in tasks)
            {
                task.IsCanceled = true;
                Update(task);
            }
        }

        
    }
}
