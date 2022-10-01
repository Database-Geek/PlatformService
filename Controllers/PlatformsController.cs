using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using Serilog;

namespace PlatformService.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    public PlatformsController(IPlatformRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
      Log.Information("--> Getting Platforms.");

      var platformItems = _repository.GetAllPlatforms();

      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    } 

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
      var platformItem = _repository.GetPlatformById(id);

      if (platformItem != null)
      {
        return Ok(_mapper.Map<PlatformReadDto>(platformItem));
      }
      else
      {
        return NotFound();
      }
    }
  }
}