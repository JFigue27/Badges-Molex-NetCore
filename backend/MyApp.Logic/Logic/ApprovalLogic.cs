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
    public class ApprovalLogic : DocumentLogic<Approval>, IDocumentLogicAsync<Approval>
    {
        public ApplicationTaskLogic ApplicationTaskLogic { get; set; }
        public override void Init(IDbConnection db, IAuthSession auth, IRequest request, IAppSettings appSettings)
        {
            base.Init(db, auth, request, appSettings);
            ApplicationTaskLogic.Init(db, auth, request, appSettings);
        }

        

        

        protected override Approval OnCreateInstance(Approval entity)
        {
            entity.Status = "New";
            

            return entity;
        }

        protected override SqlExpression<Approval> OnGetList(SqlExpression<Approval> query)
        {
            
            

            return base.OnGetList(query);
        }

        protected override SqlExpression<Approval> OnGetSingle(SqlExpression<Approval> query)
        {
            
            

            return base.OnGetSingle(query);
        }

        protected override void OnBeforeSaving(Approval entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            if (mode == OPERATION_MODE.ADD)
            {
                entity.RequestedDate = DateTimeOffset.Now;
                entity.Status = entity.Status != "Requested" ? "Created" : entity.Status;
            }

            
        }

        protected override void OnAfterSaving(Approval entity, OPERATION_MODE mode = OPERATION_MODE.NONE)
        {
            
            
        }

        protected override void OnBeforeRemoving(Approval entity)
        {
            
            
        }

        protected override List<Approval> AdapterOut(params Approval[] entities)
        {
            

            foreach (var item in entities)
            {
                item.ApplicationTasks = ApplicationTaskLogic.GetAll()
                    .Where(e => !e.IsCanceled)
                    .Where(e => e.ForeignApp == "Approvals")
                    .Where(e => e.ForeignType == "Approval")
                    .Where(e => e.ForeignKey == item.Id)
                    .ToList();

            }

            return entities.ToList();
        }

        protected override void OnFinalize(Approval entity)
        {
            
        }

        protected override void OnUnfinalize(Approval entity)
        {
            
        }

        public void RequestApproval(Approval approval)
        {
            if (approval.ApproversList == null || approval.ApproversList.Count == 0)
                throw new KnownError("Invalid Approvers.");

            approval.Status = "Requested";

            #region Email
            EmailService.From = Auth.Email;
            EmailService.Subject = "Request for Approval.";

            EmailService.Template = "approval.request";
            EmailService.TemplateParameters.Add("hyperlink", $"https://approvals.capsonic.com/approval?id={approval.Id}");

            foreach (var approver in approval.ApproversList)
                EmailService.To.Add(approver.Email);

            if (approval.SubscribersList != null)
                foreach (var subscriber in approval.SubscribersList)
                    EmailService.Cc.Add(subscriber.Email);

            //EmailService.SendMail();
            #endregion

            if (approval.Id > 0)
                Update(approval);
            else
                Add(approval);

            var taskTemplate = ApplicationTaskLogic.CreateInstance(new ApplicationTask
            {
                Title = approval.Title,
                Category = "Approval",
                Description = approval.RequestDescription,
                DueDate = approval.DueDate,
                CreatedBy = Auth.UserName,

                ForeignApp = "Approvals",
                ForeignType = "Approval",
                ForeignKey = approval.Id,
                ForeignURL = approval.Hyperlink,
            });

            ApplicationTaskLogic.SaveAll(taskTemplate, approval.ApproversList);
        }

        public Approval GetOrCreate(Approval entity)
        {
            var approvals = new List<Approval>();

            //Direct by ID:
            if (entity.Id > 0)
                return GetById(entity.Id);

            //No minimal required filters return new instance:
            if (string.IsNullOrWhiteSpace(entity.ForeignType)
                || entity.ForeignKey == null)
                return CreateInstance(entity);

            //Required filters:
            approvals = GetAll()
                .Where(e => e.ForeignType == entity.ForeignType)
                .Where(e => e.ForeignKey == entity.ForeignKey)
                .ToList();

            //New instance if not found:
            if (approvals.Count == 0)
                return CreateInstance(entity);

            //Use ForeignApp filter:
            if (approvals.Count > 1)
                approvals = approvals
                    .Where(e => e.ForeignApp == entity.ForeignApp)
                    .ToList();

            //Use ForeignTarget filter:
            if (approvals.Count > 1)
                approvals = approvals
                    .Where(e => e.ForeignTarget == entity.ForeignTarget)
                    .ToList();

            //Return first found even if there are more.
            return approvals[0];
        }

        public void SetStatus(long approvalId, string status)
        {
            var approval = GetById(approvalId);

            if (approval == null)
                throw new KnownError("Approval does not exist.");

            if (!HasRoles("Admin"))
                if (approval.ApproversList == null
                    || !approval.ApproversList.Any(e => e.UserName.ToLower() == Auth.UserName.ToLower()))
                    throw new KnownError("User not listed as Approver.");

            ApplicationTaskLogic.Done(new ApplicationTask
            {
                ForeignApp = "Approvals",
                ForeignType = "Approval",
                ForeignKey = approvalId,
                AssignedTo = Auth.UserName,
                Status = status
            });


            #region Calculate Approval Status

            approval = GetById(approvalId);

            var statusUpdated = "";

            var totalCount = approval.ApplicationTasks.Count();
            var totalCompleted = approval.ApplicationTasks.Where(e => e.ClosedAt != null).Count();
            var totalPending = approval.ApplicationTasks.Where(e => e.ClosedAt == null).Count();

            var qtyApproved = approval.ApplicationTasks
                .Where(e => e.Status == "Approved")
                .Count();
            //if (status == "Approved")
            //    qtyApproved++;


            var qtyRejected = approval.ApplicationTasks
                .Where(e => e.Status == "Rejected")
                .Count();
            //if (status == "Rejected")
            //    qtyRejected++;


            double percentageApproved = (double)qtyApproved / totalCount * 100;
            double percentageRejected = (double)qtyRejected / totalCount * 100;
            double percentagePending = (double)totalPending / totalCount * 100;

            if (approval.RequiredQuantity > 0)
            {
                if (qtyApproved >= approval.RequiredQuantity)
                    statusUpdated = "Approved";
                else if ((qtyApproved + totalPending) < approval.RequiredQuantity)
                    statusUpdated = "Rejected";
                // else:  Cannot calculate yet.
            }
            else if (approval.RequiredPercentage > 0)
            {
                if (percentageApproved >= approval.RequiredPercentage)
                    statusUpdated = "Approved";
                else if ((percentageApproved + percentagePending) < approval.RequiredPercentage)
                    statusUpdated = "Rejected";
                //else: Cannot calculate yet.
            }
            else
            {
                if (totalCompleted == totalCount)
                    if (qtyApproved == totalCount)
                        statusUpdated = "Approved";
                    else
                        statusUpdated = "Rejected";
                //else: Cannot calculate yet.
            }

            if (statusUpdated.Length > 0)
            {
                approval.Status = statusUpdated;
                Update(approval);
            }
            #endregion
        }

        public void Cancel(long approvalId)
        {
            var approval = GetById(approvalId);

            if (approval == null) throw new KnownError("Approval does not exist.");

            ApplicationTaskLogic.CancelAllFromForeign(approval, "Approval", "Approvals");

            RemoveById(approvalId);
        }

        
    }
}
