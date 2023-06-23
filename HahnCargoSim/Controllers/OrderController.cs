using System.Security.Claims;
using HahnCargoSim.Model.DTO;
using HahnCargoSim.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HahnCargoSim.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]/[action]")]
  public class OrderController : ControllerBase
  {
    private readonly IOrderService orderService;

    public OrderController(IOrderService orderService)
    {
      this.orderService = orderService;
    }

    [HttpGet]
    public List<OrderDto> GetAllAvailable()
    {
      return orderService.GetAllAvailable().ConvertAll(order => order.ToDto());
    }
    
    [HttpGet]
    public List<OrderDto> GetAllAccepted()
    {
      var owner = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
      return orderService.GetForOwner(owner).ConvertAll(order => order.ToDto());
    }

    [HttpPost]
    public IActionResult Accept(int orderId)
    {
      var owner = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
      return orderService.Accept(orderId, owner) ? Ok() : BadRequest();
    }

    [HttpPost]
    public IActionResult Create()
    {
      orderService.Create();
      return Ok();
    }

    [HttpPost]
    public IActionResult GenerateFile(int maxTicks, string filename)
    {
      orderService.GenerateFile(maxTicks, filename);
      return Ok();
    }


  }
}
