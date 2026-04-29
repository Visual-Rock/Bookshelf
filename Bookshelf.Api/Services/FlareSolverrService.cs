using Bookshelf.Api.Config;
using FlareSolverrSharp;

namespace Bookshelf.Api.Services;

public interface IFlareSolverrService
{
    Task<string> GetStringAsync(string url);
    Task<HttpResponseMessage> GetAsync(string url);
}

public class FlareSolverrService : IFlareSolverrService
{
    private HttpClient _client;
    
    public FlareSolverrService(IConfiguration config)
    {
        var flareSolverrConfig = config.GetSection("FlareSolverr").Get<FlareSolverrConfig>()!;
        
        var handler = new ClearanceHandler(flareSolverrConfig.ServerUrl)
        {
            MaxTimeout = flareSolverrConfig.MaxTimeout, 
            ProxyUrl = flareSolverrConfig?.ProxyUrl ?? string.Empty,
            ProxyUsername = flareSolverrConfig?.ProxyUsername ?? string.Empty,
            ProxyPassword = flareSolverrConfig?.ProxyPassword ?? string.Empty
        };

        _client = new HttpClient(handler);
    }
    
    public Task<string> GetStringAsync(string url) => _client.GetStringAsync(url);

    public Task<HttpResponseMessage> GetAsync(string url) => _client.GetAsync(url);
}