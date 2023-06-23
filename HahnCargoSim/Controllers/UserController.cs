using System.Security.Claims;
using HahnCargoSim.Helper;
using HahnCargoSim.Model;
using HahnCargoSim.Model.DTO;
using HahnCargoSim.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HahnCargoSim.Controllers
{
  [Authorize]
  [ApiController]
  [Route("[controller]/[action]")]

  public class UserController : ControllerBase
  {
    private readonly IUserService userService;
    private readonly SimConfig? config;

    public UserController(IUserService userService, IOptions<SimConfig> config)
    {
      this.userService = userService;
      this.config = config.Value;
      if (this.config == null)
      {
        throw new Exception("Config is null");
      }
    }

    [HttpGet]
    public int CoinAmount()
    {
      var owner = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
      return userService.GetCoinAmount(owner);
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login([FromBody] UserAuthenticateDto model)
    {
      IActionResult result;
      var user = userService.GetByUserName(model.Username);

      if (user == null || !CryptoHelper.IsAuthorized(model.Password, user.PasswordHash))
      {
        result = BadRequest(new { message = "Username or password is incorrect" });
      }
      else
      {
        var tokenString = CryptoHelper.GetJwtTokenString(config.TokenSecret, user.UserName);
        result = Ok(new
        {
          user.UserName,
          Token = tokenString
        });
      }

      return result;
    }
  }
}
