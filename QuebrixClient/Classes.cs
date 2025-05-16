namespace QuebrixClient;

public class QuebrixConnectionOptions
{
    /// <summary>
    /// without http or https
    /// </summary>
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}

public static class StaticQuebrixConnectionOptions
{
    public static string Host { get; set; } = string.Empty;
    public static int Port { get; set; }
    public static string UserName { get; set; } = string.Empty;
    public static string Password { get; set; } = string.Empty;
    public static bool? HasEFSharding { get; set; }
    public static bool? HasCache { get; set; }
}


