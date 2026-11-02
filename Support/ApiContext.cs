using RestSharp;

namespace DummyJson.ApiTests.Support;
public class ApiContext
{
    public RestClient Client { get; } = new(new RestClientOptions("https://dummyjson.com"));
    public RestResponse? Response { get; set; }
    public static string? AccessToken { get; set; }
}