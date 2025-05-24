using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuebrixClient;
using QuebrixClient.RequestDtos;
using QuebrixClient.Response.Dtos;

namespace Example.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuebrixExampleController(IQuebrixCacheProvider qbxCache,FakeAppDbContext _context) : ControllerBase
    {

        [HttpPost("auth")]
        public async Task<CacheResponse<string>> Authenticate(string userName,string password)
            => await qbxCache.AuthenticateAsync(userName,password);


        [HttpPost("set")]
        public async Task<CacheResponse<bool>> Set()
            => await qbxCache.SetAsync("prod", "testClientCS",JsonConvert.SerializeObject(new AuthenticateRequest { password = "admin",UserName = "admin",Role ="admin"}));

        [HttpPost("get")]
        public async Task<CacheResponse<AuthenticateRequest>> get(string cluster, string key, long ttl)
           => await qbxCache.GetAsync<AuthenticateRequest>("prod", "testClientCS");



        [HttpPost]
        public async Task<IActionResult> AddUser(string name)
        {
            int? managementAccountId = 1;
            if (!managementAccountId.HasValue) return BadRequest("Missing header X-ManagementAccountId");

            _context.Users.Add(new User
            {
                Name = name,
                ManagementAccountId = managementAccountId.Value
            });

            await _context.SaveChangesAsync();

            return Ok("User added to shard");
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

    }
}
