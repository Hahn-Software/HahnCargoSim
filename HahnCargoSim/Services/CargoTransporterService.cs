using HahnCargoSim.Model;
using HahnCargoSim.Services.Interfaces;

namespace HahnCargoSim.Services
{
  public class CargoTransporterService : ICargoTransporterService
  {

    private readonly Dictionary<string, List<CargoTransporter>> cargoTransporters;
    private readonly IGridService gridService;
    private readonly IUserService userService;
    private readonly ILoggerService loggerService;

    private int maxTransporterId;

    public CargoTransporterService(IGridService gridService, IUserService userService, ILoggerService loggerService)
    {
      cargoTransporters = new Dictionary<string, List<CargoTransporter>>();
      this.gridService = gridService;
      this.userService = userService;
      this.loggerService = loggerService;
      maxTransporterId = 0;
    }

    public CargoTransporter? Get(int transporterId, string owner)
    {
      return !cargoTransporters.ContainsKey(owner) ? null : 
          cargoTransporters[owner].FirstOrDefault(transporter => transporter.Id == transporterId);
    }

    public List<CargoTransporter> GetAll(string owner)
    {
      return !cargoTransporters.ContainsKey(owner) ? new List<CargoTransporter>() : cargoTransporters[owner];
    }

    public int Buy(string owner, int positionNodeId)
    {
      if (!cargoTransporters.ContainsKey(owner))
      {
        loggerService.Log($"{owner} got the starter cargo transporter");
        return Create(owner); 
      }

      var coins = userService.GetCoinAmount(owner);
      if (coins < 1000)
      {
        loggerService.Log($"{owner} has not enough coins to buy a cargo transporter");
        return -1;
      }

      userService.UpdateCoinAmount(owner, -1000);
      loggerService.Log($"{owner} bought a cargo transporter {maxTransporterId}");
      return Add(owner, positionNodeId);
    }

    private int Create(string owner)
    {
      if (cargoTransporters.ContainsKey(owner)) return -1;
      cargoTransporters.Add(owner, new List<CargoTransporter>());
      return Add(owner, gridService.GetGrid().Nodes[0].Id);
    }

    private int Add(string owner, int positionNodeId)
    {
      if (!cargoTransporters.ContainsKey(owner))
      {
        return -1;
      }

      maxTransporterId++;

      var cargoTransporter = new CargoTransporter
      {
        Id = maxTransporterId,
        Owner = owner,
        Position = gridService.GetNode(positionNodeId) ?? gridService.GetRandomNode(),
        InTransit = false,
        Capacity = 1000,
        Load = 0,
        LoadedList = new List<Order>()
      };
      cargoTransporters[owner].Add(cargoTransporter);
      return cargoTransporter.Id;
    }

    public void PutTransporterIntoMove(int transporterId, string owner)
    {
      var transporter = Get(transporterId, owner);
      if (transporter == null) return;

      transporter.Position = null;
      transporter.InTransit = true;
    }

    public void PutTransporterIntoPosition(int transporterId, string owner, int nodeId)
    {
      var transporter = Get(transporterId, owner);
      if (transporter == null) return;
      
      var node = gridService.GetNode(nodeId);
      if(node == null) return;

      transporter.Position = node;
      transporter.InTransit = false;
    }

    public List<Order> UnloadOrdersFromCargoTransporter(int transporterId, string owner)
    {
      var finishedOrders = new List<Order>();

      var transporter = Get(transporterId, owner);
      if (transporter?.Position == null) return finishedOrders;

      finishedOrders = transporter.LoadedList.FindAll(order => order.TargetNode.Id == transporter.Position.Id);

      foreach (var finishedOrder in finishedOrders)
      {
        transporter.Load -= finishedOrder.Load;
        transporter.LoadedList.Remove(finishedOrder);
        loggerService.Log($"Unload cargo transporter {transporter.Id} from {owner} on node {transporter.Position.Id} unloaded cargo for Order {finishedOrder.Id}.");
      }

      return finishedOrders;
    }

    public void LoadOrdersToCargoTransporter(int transporterId, string owner, List<Order> ordersToLoad)
    {
      var transporter = Get(transporterId, owner);
      if (transporter?.Position == null) return;

      foreach (var order in ordersToLoad.Where(o => o.OriginNode.Id == transporter.Position.Id))
      {
        var alreadyLoaded = false;
        foreach (var loadedOrder in transporter.LoadedList)
        {
          if (loadedOrder.Id != order.Id) continue;
          alreadyLoaded = true;
        }
        if (alreadyLoaded)
        {
          continue;
        }
        
        if (transporter.Load + order.Load > transporter.Capacity)
        {
          loggerService.Log($"Load cargo transporter {transporter.Id} from {owner} on node {transporter.Position.Id} has not enough capacity to load order {order.Id}. Available: {transporter.Capacity - transporter.Load} Needed: {order.Load}.");
          continue;
        }

        transporter.Load += order.Load;
        transporter.LoadedList.Add(order);
        loggerService.Log($"Load cargo transporter {transporter.Id} from {owner} on node {transporter.Position.Id} - total order loaded on transporter {transporter.LoadedList.Count}.");
      }
    }

    public void RemoveLoadedOrder(int orderId, string owner)
    {
      foreach (var cargoTransporter in cargoTransporters[owner])
      {
        var order = cargoTransporter.LoadedList.FirstOrDefault(o => o.Id == orderId);
        if (order == null) continue;

        cargoTransporter.Load -= order.Load;
        cargoTransporter.LoadedList.Remove(order);
        
      }
    }


  }
}
