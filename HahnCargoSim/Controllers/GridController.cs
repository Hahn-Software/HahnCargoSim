using System.Text.Json.Serialization;
using HahnCargoSim.Model;
using HahnCargoSim.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HahnCargoSim.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]/[action]")]
  public class GridController : ControllerBase
  {
    private readonly IGridService gridService;

    public GridController(IGridService gridService)
    {
      this.gridService = gridService;
    }

    [HttpGet]
    public string Get()
    {
      return gridService.GetGridAsJson();
    }

    [HttpPost]
    public IActionResult GenerateFile(int numberOfNodes, int numberOfEdges, int numberOfConnectionsPerNode, string filename)
    {
      gridService.GenerateFile(numberOfNodes, numberOfEdges, numberOfConnectionsPerNode, filename);
      return Ok();
    }
  }
}
