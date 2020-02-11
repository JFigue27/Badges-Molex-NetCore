using System.Threading.Tasks;
namespace Reusable.CRUD.Contract
{
    public interface IDocumentLogic<Entity> : ILogicWrite<Entity> where Entity : BaseDocument
    {
        void Finalize(Entity document);
        void Unfinalize(Entity document);
        void MakeRevision(Entity document);
        Entity UpdateAndMakeRevision(Entity entity);
        void Checkout(long id);
        void Checkin(Entity document);
        void CancelCheckout(long id);
        Entity CreateAndCheckout(Entity document);
        //CommonResponse Activate(long id);
        //CommonResponse Unlock(Entity document);
        //CommonResponse Unlock(long id);
        //CommonResponse Lock(Entity document);
        //CommonResponse Lock(long id);
        //void _makeRevision(Entity document);
    }
    public interface IDocumentLogicAsync<Entity> : ILogicWriteAsync<Entity> where Entity : BaseDocument
    {
        Task FinalizeAsync(Entity entity);
        Task UnfinalizeAsync(Entity document);
        Task MakeRevisionAsync(Entity document);
        Task<Entity> UpdateAndMakeRevisionAsync(Entity entity);
        Task CheckoutAsync(long id);
        Task CheckinAsync(Entity document);
        Task CancelCheckoutAsync(long id);
        Task<Entity> CreateAndCheckoutAsync(Entity document);
    }
}
