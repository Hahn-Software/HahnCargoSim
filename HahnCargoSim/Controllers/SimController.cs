using HahnCargoSim.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HahnCargoSim.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]/[action]")]
  public class SimController : ControllerBase
  {
    private readonly ISimService simService;

    public SimController(ISimService simService)
    {
      this.simService = simService;
    }

    [HttpPost]
    public void Start()
    {
      simService.Start();
    }

    [HttpPost]
    public void Stop()
    {
      simService.Stop();
    }


  }
}
