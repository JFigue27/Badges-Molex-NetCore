using ServiceStack;

namespace Reusable.Rest.Implementations.SS
{
    //[Restrict(LocalhostOnly = true)]
    public class CacheService : Service,
        IGet<FlushAll>
    {
        public object Get(FlushAll request)
        {
            Cache.FlushAll();
            return new CommonResponse().Success();
        }
    }

    [Route("/Cache/FlushAll", "GET")]
    public class FlushAll
    {
    }
}
