using Newtonsoft.Json;

namespace QuebrixClient.RequestDtos;

public class MoveOrCopyRequest
{

    [JsonProperty("src_cluster")]
    public string SrcCluster { get; set; }

    [JsonProperty("desc_cluster")]
    public string DestCluster { get; set; }

}
