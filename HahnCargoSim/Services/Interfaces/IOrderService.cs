using HahnCargoSim.Model;

namespace HahnCargoSim.Services.Interfaces
{
  public interface IOrderService
  {
    List<Order> GetAllAvailable();
    List<Order> GetForOwner(string owner);
    Order? GetAvailable(int orderId);
    Order? GetAccepted(int orderId, string owner);
    bool Accept(int orderId, string owner);
    bool Finish(int orderId, string owner);
    void Create();
    void RemoveExpiredAvailable();
    void RemoveAccepted(int orderId, string owner);
    void Prefabricated(int tick);
    void GenerateFile(int maxTicks, string filename);
  }
}
