using System.Text.Json;
using Bookshelf.Api.Config;

namespace Bookshelf.Api.Services;

public interface IFlareSolverrService
{
    Task<string> GetStringAsync(string url);
}

public class FlareSolverrService(IConfiguration config) : IFlareSolverrService
{
    private FlareSolverrConfig _flareSolverrConfig = config.GetSection("FlareSolverr").Get<FlareSolverrConfig>()!;
    private readonly HttpClient _client = new();

    private static string _getCommand = "request.get";
    
    public async Task<string> GetStringAsync(string url)
    {
        var response = await SendMessage(_getCommand, url);
        response.EnsureSuccessStatusCode();
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("solution").GetProperty("response").GetString()!;
    }

    private Task<HttpResponseMessage> SendMessage(string command, string url)
    {
        var content = JsonContent.Create(new { cmd = command, url, maxTimeout = _flareSolverrConfig.MaxTimeout });
        return  _client.PostAsync(GetUrl(), content);
    }

    private string GetUrl() => $"{_flareSolverrConfig.ServerUrl}{(_flareSolverrConfig.ServerUrl.EndsWith('/') ? "" : "/")}v1";
}