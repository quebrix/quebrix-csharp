using Newtonsoft.Json;

namespace QuebrixClient.Response.Dtos;

public class ValueResult
{
    [JsonProperty("value")]
    public byte[] Value { get; set; }
    [JsonProperty("value_type")]
    public string ValueType { get; set; }
}

public class CacheResponse<T>
{
    [JsonProperty("is_success")]
    public bool IsSuccess { get; set; }

    [JsonProperty("data")]
    public T? Data { get; set; }
    public string? Message { get; set; }


    public static CacheResponse<T> Ok<T>(T data) => new()
    {
        Data = data,
        IsSuccess = true
    };

    public static CacheResponse<T> Failed() => new()
    {
        IsSuccess = false
    };

    public static CacheResponse<T> Failed(string message) => new()
    {
        Message = message,
        IsSuccess = false
    };
}
