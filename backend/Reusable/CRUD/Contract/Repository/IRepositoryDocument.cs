namespace Reusable.CRUD.Contract
{
    public interface IDocumentRepository<T> : IRepository<T> where T : BaseDocument
    {
        void Activate(long id);
        void Deactivate(long id);
        void Deactivate(T entity);
        void Lock(long id);
        void Lock(T entity);
        void Unlock(long id);
        void Unlock(T entity);

        void Finalize(T entity);
        void Unfinalize(T entity);
    }
}
