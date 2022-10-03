using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using ILogger = Serilog.ILogger;

namespace PlatformService.SyncDataServices.Http
{
  public class HttpCommandDataClient : ICommandDataClient
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, ILogger logger, IConfiguration configuration)
    {
      _httpClient = httpClient;
      _logger = logger;
      _configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
      var httpContent = new StringContent(
        JsonSerializer.Serialize(platform),
        Encoding.UTF8,
        "application/json");

      var response = await _httpClient.PostAsync(_configuration["CommandService"], httpContent);

      if(response.IsSuccessStatusCode)
      {
        _logger.Information("--> Sync POST to CommandService was OK!");
      }
      else
      {
        _logger.Warning("--> Sync POST to CommandService was NOT OK!");
      }
    }
  }
}