using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Reusable.CRUD.Contract;
using Reusable.CRUD.Entities;
using ServiceStack.Auth;
using ServiceStack.OrmLite;
using ServiceStack.Web;

namespace Reusable.CRUD.Implementations.SS.Logic
{
    //Cannot inherit from DocumentLogic because causes a circular reference
    public class RevisionLogic : WriteLogic<Revision>
    {
        public override void Init(IDbConnection db, IAuthSession auth, IRequest request)
        {
            base.Init(db, auth, request);
        }

        #region Overrides
        public override Revision Add(Revision entity)
        {
            entity.CreatedAt = DateTimeOffset.Now;
            entity.CreatedBy = Auth.UserName;

            return base.Add(entity);
        }

        public override async Task<Revision> AddAsync(Revision entity)
        {
            entity.CreatedAt = DateTimeOffset.Now;
            entity.CreatedBy = Auth.UserName;

            return await base.AddAsync(entity);
        }
        #endregion

        #region Specific Operations
        public List<Revision> GetRevisionsForEntity(long ForeignKey, string ForeignType)
        {
            var query = Db.From<Revision>()
                .Where(e => e.ForeignKey == ForeignKey && e.ForeignType == ForeignType)
                .OrderByDescending(e => e.CreatedAt);

            return Db.LoadSelect(query).ToList();
        }
        #endregion
    }
}
