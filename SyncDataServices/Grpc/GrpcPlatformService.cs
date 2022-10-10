using AutoMapper;
using Grpc.Core;
using PlatformService.Data;
using ILogger = Serilog.ILogger;

namespace PlatformService.SyncDataServices.Grpc
{
  public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
  {
    private readonly ILogger _logger;
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;

    public GrpcPlatformService(ILogger logger, IPlatformRepo repository, IMapper mapper)
    {
      _logger = logger;
      _repository = repository;
      _mapper = mapper;

      _logger.Information("--> Starting GrpcPlatformService.");
    }

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
    {
      var response = new PlatformResponse();
      var platforms = _repository.GetAllPlatforms();

      foreach(var platform in platforms)
      {
        response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
      }

      return Task.FromResult(response);
    }
  }
}