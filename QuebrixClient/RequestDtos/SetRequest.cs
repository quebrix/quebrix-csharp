using Newtonsoft.Json;

namespace QuebrixClient.RequestDtos;

public class SetRequest
{
    [JsonProperty("cluster")]
    public string cluster { get; set; } = string.Empty;

    [JsonProperty("key")]
    public string key { get; set; } = string.Empty;

    [JsonProperty("value")]
    public string value { get; set; } = string.Empty ;

    [JsonProperty("ttl")]
    public long? ttl { get; set; } 
}
