using Newtonsoft.Json;

namespace QuebrixClient.Response.Dtos
{
    public class AuthenticationRequestResponse
    {
        [JsonProperty("username")]
        public string UserName { get; set; } = string.Empty;
        [JsonProperty("password")]
        public string password { get; set; } = string.Empty;
        [JsonProperty("role")]
        public string Role { get; set; } = string.Empty;
    }
}
