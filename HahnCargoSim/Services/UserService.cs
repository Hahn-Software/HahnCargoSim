using HahnCargoSim.Helper;
using HahnCargoSim.Model;
using HahnCargoSim.Services.Interfaces;

namespace HahnCargoSim.Services
{
  public class UserService : IUserService
  {
    private readonly ILoggerService loggerService;

    private readonly Dictionary<string, CargoSimUser> users;

    public UserService(ILoggerService loggerService)
    {
      this.loggerService = loggerService;

      users = new Dictionary<string, CargoSimUser>();
    }

    public CargoSimUser? GetByUserName(string userName)
    {
      if (users.ContainsKey(userName))
      {
        return users[userName];
      }
      
      var user = new CargoSimUser
      {
        UserName = userName,
        PasswordHash = CryptoHelper.GetPasswordHash("Hahn"),
        CoinAmount = 1000
      };

      users.Add(userName, user);

      return user;
    }

    public List<string> GetAllUserIds()
    {
      return users.Keys.ToList();
    }

    public int GetCoinAmount(string userName)
    {
      return users[userName].CoinAmount;
    }

    public void UpdateCoinAmount(string userName, int amount)
    {
      users[userName].CoinAmount += amount;
    }


    public void PrintResult()
    {
      foreach (var userName in users.Keys)
      {
        loggerService.Log($"Result for {userName}: {users[userName].CoinAmount} Coins");
      }
    }

  }
}
