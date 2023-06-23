using HahnCargoSim.Model;

namespace HahnCargoSim.Services.Interfaces
{
  public interface ICargoTransporterService
  {
    CargoTransporter? Get(int transporterId, string owner);
    List<CargoTransporter> GetAll(string owner);
    int Buy(string owner, int positionNodeId);
    void PutTransporterIntoMove(int transporterId, string owner);
    void PutTransporterIntoPosition(int transporterId, string owner, int nodeId);
    List<Order> UnloadOrdersFromCargoTransporter(int transporterId, string owner);
    void LoadOrdersToCargoTransporter(int transporterId, string owner, List<Order> ordersToLoad);
    void RemoveLoadedOrder(int orderId, string owner);

  }
}
