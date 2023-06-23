using HahnCargoSim.Model;

namespace HahnCargoSim.Services.Interfaces
{
  public interface IUserService
  {
    CargoSimUser? GetByUserName(string userName);
    List<string> GetAllUserIds();
    int GetCoinAmount(string userName);
    void UpdateCoinAmount(string userName, int amount);
    void PrintResult();

  }
}
