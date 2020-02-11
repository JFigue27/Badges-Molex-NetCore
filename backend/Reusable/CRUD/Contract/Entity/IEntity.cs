using System;

namespace Reusable.CRUD.Contract
{
    public interface IEntity : ICloneable
    {
        long Id { get; set; }
        string EntityName { get; }
        byte[] RowVersion { get; set; }
    }
}
