using Newtonsoft.Json;

namespace QuebrixClient.RequestDtos;

public class AuthenticateRequest
{
    [JsonProperty("username")]
    public string UserName { get; set; }
    [JsonProperty("password")]
    public string password { get; set; }
    [JsonProperty("role")]
    public string Role { get; set; }
}
