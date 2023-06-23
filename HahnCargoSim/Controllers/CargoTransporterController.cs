using System.Security.Claims;
using HahnCargoSim.Model;
using HahnCargoSim.Model.DTO;
using HahnCargoSim.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HahnCargoSim.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]/[action]")]
  public class CargoTransporterController : ControllerBase
  {
    private readonly ICargoTransporterService transporterService;
    private readonly ISimService simService;

    public CargoTransporterController(ICargoTransporterService transporterService, ISimService simService)
    {
      this.transporterService = transporterService;
      this.simService = simService;
    }

    [HttpPost]
    public int Buy(int positionNodeId)
    {
      var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
      
      return transporterService.Buy(username, positionNodeId);
    }

    [HttpGet]
    public CargoTransporterDto? Get(int transporterId)
    {
      var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
      var transporter = transporterService.Get(transporterId, username);
      return transporter?.ToDto();
    }

    [HttpPut]
    public async Task<IActionResult> Move(int transporterId, int targetNodeId)
    {
      var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
      var transporter = transporterService.Get(transporterId, username);

      if (transporter == null)
      {
        return BadRequest("Invalid transporter");
      }

      var orderValid = await Task.Run(() => this.simService.QueueMoveOrder(new MoveOrder
        {
          CargoTransporterId = transporter.Id,
          Owner = username,
          TargetNodeId = targetNodeId,
        }
      ));

      if (orderValid)
      {
        return Ok();
      } 
      
      return BadRequest("Invalid move order");
      
    }
  }
}
