using System.Buffers.Text;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using QuebrixClient.RequestDtos;
using QuebrixClient.Response.Dtos;
using RestSharp;

namespace QuebrixClient;

public class QuebrixCacheProvider:IQuebrixCacheProvider
{
    private readonly RestClient _client;
    public QuebrixCacheProvider()
    {
        _client = new RestClient($"http://{StaticQuebrixConnectionOptions.Host}:{StaticQuebrixConnectionOptions.Port}");
    }


    public async Task<CacheResponse<string>> AuthenticateAsync(string userName, string password)
    {
        var url = "/api/login";
        var request = new RestRequest(url, Method.Post);
        var setRequest = new AuthenticateRequest
        {
            password = password,
            UserName = userName,
            Role = string.Empty
        };
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(JsonConvert.SerializeObject(setRequest));
        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            return JsonConvert.DeserializeObject<CacheResponse<string>>(response.Content);
        }
        else
            return CacheResponse<string>.Failed("Authentication Failed,see Quebrix logs for see details");
    }

    public async Task<CacheResponse<List<string>>> GetAllClustersAsync()
    {
        var url = $"/api/get_clusters";
        var credentials = MakeAuth(StaticQuebrixConnectionOptions.UserName, StaticQuebrixConnectionOptions.Password);
        var request = new RestRequest(url, Method.Get);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            try
            {
                var apiResponse = JsonConvert.DeserializeObject<CacheResponse<List<string>>>(response.Content);
                return apiResponse;
            }
            catch
            {
                var apiResponse = JsonConvert.DeserializeObject<CacheResponse<string>>(response.Content);
                if (!string.IsNullOrEmpty(apiResponse.Data))
                {
                    return CacheResponse<List<string>>.Ok(new List<string>());
                }
                else
                {
                    return CacheResponse<List<string>>.Failed("no cluster found");
                }
            }
        }
        else
        {
            throw new Exception($"Error getting clusters: {response.ErrorMessage}");
        }
    }

    public async Task<bool> ClearCluster(string cluster)
    {
        var url = $"/api/clear_cluster/{HttpUtility.UrlEncode(cluster)}";
        var credentials = MakeAuth(StaticQuebrixConnectionOptions.UserName, StaticQuebrixConnectionOptions.Password);
        var request = new RestRequest(url, Method.Delete);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
            return true;
        else
            return false;
    }


    public async Task<CacheResponse<List<string>>> GetKeysOfClusterAsync(string clusterName)
    {
        var url = $"/api/get_keys/{HttpUtility.UrlEncode(clusterName)}";
        var credentials = MakeAuth(StaticQuebrixConnectionOptions.UserName, StaticQuebrixConnectionOptions.Password);
        var request = new RestRequest(url, Method.Get);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var apiResponse = JsonConvert.DeserializeObject<CacheResponse<List<string>>>(response.Content);
            return apiResponse;
        }
        else
        {
            throw new Exception($"Error getting keys of  cluster: {response.ErrorMessage}");
        }
    }

    public async Task<bool> SetCluster(string cluster)
    {
        var url = $"/api/set_cluster/{HttpUtility.UrlEncode(cluster)}";
        var credentials = MakeAuth(StaticQuebrixConnectionOptions.UserName, StaticQuebrixConnectionOptions.Password);
        var request = new RestRequest(url, Method.Post);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
            return true;
        else
            return false;
    }


    public async Task<CacheResponse<T>> GetAsync<T>(string cluster, string key)
    {
        try
        {
            var url = $"/api/get/{HttpUtility.UrlEncode(cluster)}/{HttpUtility.UrlEncode(key)}";
            var credentials = MakeAuth(StaticQuebrixConnectionOptions.UserName, StaticQuebrixConnectionOptions.Password);
            var request = new RestRequest(url, Method.Get);
            request.AddHeader("Authorization", credentials);

            var response = await _client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                return CacheResponse<T>.Failed(response.ErrorMessage ?? "Request failed");
            }

            // اول JSON رو به ApiResponse<ValueResult> دیکد کن
            var apiResponse = JsonConvert.DeserializeObject<CacheResponse<ValueResult>>(response.Content);

            if (apiResponse == null || !apiResponse.IsSuccess || apiResponse.Data == null)
            {
                return CacheResponse<T>.Failed(apiResponse?.Message ?? "Failed to get or deserialize data");
            }

            object finalValue;

            // اگر نوع داده مقصد int هست
            if (typeof(T) == typeof(int) && apiResponse.Data.ValueType == "Int")
            {
                // تبدیل بایت آرایه به int
                var intValue = BitConverter.ToInt32(apiResponse.Data.Value, 0);
                finalValue = intValue;
            }
            // اگر رشته هست
            else if (typeof(T) == typeof(string))
            {
                var stringValue = Encoding.UTF8.GetString(apiResponse.Data.Value);
                finalValue = stringValue;
            }
            // اگر خود T از نوع بایت آرایه است (byte[])
            else if (typeof(T) == typeof(byte[]))
            {
                finalValue = apiResponse.Data.Value;
            }
            else
            {
                // اگر نوع دیگه‌ای هست، تلاش می‌کنیم مستقیم دیکد کنیم (مثلا اگر JSON رشته‌سریال شده است)
                var jsonString = Encoding.UTF8.GetString(apiResponse.Data.Value);
                finalValue = JsonConvert.DeserializeObject<T>(jsonString);
            }

            return CacheResponse<T>.Ok((T)finalValue);
        }
        catch (Exception ex)
        {
            return CacheResponse<T>.Failed($"Exception occurred: {ex.Message}");
        }
    }





    public async Task<CacheResponse<bool>> SetAsync<T>(string cluster, string key, T value, long? expireTime = null)
    {
        var url = "/api/set";
        var request = new RestRequest(url, Method.Post);
        var credentials = MakeAuth(StaticQuebrixConnectionOptions.UserName, StaticQuebrixConnectionOptions.Password);
        var jsonValue = JsonConvert.SerializeObject(value);
        var setRequest = new SetRequest
        {
            cluster = cluster,
            key = key,
            value = jsonValue,
            ttl = expireTime
        };
        var jsonBody = JsonConvert.SerializeObject(setRequest);
        request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
        request.AddHeader("Authorization", $"{credentials}");

        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            var result = JsonConvert.DeserializeObject<CacheResponse<string>>(response.Content);
            if (result.IsSuccess)
            {
                return CacheResponse<bool>.Ok<bool>(true);
            }
            else
            {
                return CacheResponse<bool>.Failed();
            }
        }
        else
        {
            return CacheResponse<bool>.Failed();
        }
    }


    public async Task<CacheResponse<bool>> Delete(string cluster, string key)
    {
        var url = $"/api/delete/{HttpUtility.UrlEncode(cluster)}/{HttpUtility.UrlEncode(key)}";
        var request = new RestRequest(url, Method.Delete);
        var credentials = MakeAuth(StaticQuebrixConnectionOptions.UserName, StaticQuebrixConnectionOptions.Password);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            return CacheResponse<bool>.Ok(true);
        }
        else
        {
            return CacheResponse<bool>.Failed();
        }
    }


    public async Task<CacheResponse<bool>> CopyCluster(string srcCluster, string destCluster)
    {
        var url = "/api/copy_cluster";
        var request = new RestRequest(url, Method.Post);
        var credentials = MakeAuth(StaticQuebrixConnectionOptions.UserName, StaticQuebrixConnectionOptions.Password);
        var setRequest = new MoveOrCopyRequest
        {
            DestCluster = destCluster,
            SrcCluster = srcCluster
        };
        var jsonBody = JsonConvert.SerializeObject(setRequest);
        request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
        request.AddHeader("Authorization", $"{credentials}");
        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            var result = JsonConvert.DeserializeObject<CacheResponse<string>>(response.Content);
            if (result.IsSuccess)
            {
                return CacheResponse<bool>.Ok(true);
            }
            else
            {
                return CacheResponse<bool>.Failed();
            }
        }
        else
        {
            return CacheResponse<bool>.Failed();
        }
    }
    //helpers
    private string MakeAuth(string username, string password) => $"qbx.{Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"))}";
}
