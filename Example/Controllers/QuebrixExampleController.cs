using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuebrixClient;
using QuebrixClient.RequestDtos;
using QuebrixClient.Response.Dtos;

namespace Example.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QuebrixExampleController(IQuebrixCacheProvider qbxCache) : ControllerBase
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

    }
}
