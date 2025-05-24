using Microsoft.EntityFrameworkCore;
using QuebrixClient;

namespace Example
{
    //AppContext
    public class FakeAppDbContext : QuebrixDbContext<FakeAppDbContext>
    {
        public FakeAppDbContext(DbContextOptions<FakeAppDbContext> options, IQuebrixCacheProvider cacheProvider, IQuebrixEFSharder sharder) : base(options, cacheProvider, sharder)
        {
        }

        public DbSet<User> Users { get; set; }
    }

    //Entity
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ManagementAccountId { get; set; }
    }


    //ShardKey Finder
    public class FakeSharder : IQuebrixEFSharder
    {

        public async Task<int> GetShardingKey()
        {
           return await Task.FromResult(1001);
        }
    }
}
