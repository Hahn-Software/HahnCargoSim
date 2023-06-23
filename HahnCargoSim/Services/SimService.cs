using System.Threading;
using HahnCargoSim.Model;
using HahnCargoSim.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HahnCargoSim.Services
{
  public class SimService : ISimService
  {
    private readonly SimConfig? config;
    private readonly IGridService gridService;
    private readonly IOrderService orderService;
    private readonly ICargoTransporterService transporterService;
    private readonly IUserService userService;
    private readonly ILoggerService loggerService;

    private bool IsSimRunning;

    private readonly List<MoveOrder> MoveOrderQueue = new List<MoveOrder>();
    private int maxMoveOrderId;


    public SimService (IOptions<SimConfig> config, 
      IGridService gridService, 
      IOrderService orderService, 
      ICargoTransporterService transporterService, 
      IUserService userService, 
      ILoggerService loggerService,
      IHostApplicationLifetime applicationLifetime)
    {
      this.loggerService = loggerService;
      this.config = config.Value;
      if (this.config == null)
      {
        loggerService.Log("Error - Config is null");
        throw new Exception("Config is null");
      }

      this.gridService = gridService;
      this.orderService = orderService;
      this.transporterService = transporterService;
      this.userService = userService;
      
      this.IsSimRunning = false;
      this.maxMoveOrderId = 0;
    }

    public void Start()
    {
      if(IsSimRunning) return;

      IsSimRunning = true;

      loggerService.Log("Sim started");

    }

    public void Stop()
    {
      IsSimRunning = false;
      loggerService.Log("Sim stopped");
      userService.PrintResult();
    }

    public bool QueueMoveOrder(MoveOrder moveOrder)
    {
      //Valid Check - Base Data
      var transporter = transporterService.Get(moveOrder.CargoTransporterId, moveOrder.Owner);

      var startNode = transporter?.Position;
      if (startNode == null) return false;

      var connectionId = gridService.ConnectionAvailable(startNode.Id, moveOrder.TargetNodeId);
      if (connectionId == null) return false;

      var travelCost = gridService.GetConnectionCost((int)connectionId);
      if(userService.GetCoinAmount(moveOrder.Owner) - travelCost < 0) return false;


      //Pay & Queue
      maxMoveOrderId++;
      moveOrder.Id = maxMoveOrderId;
      userService.UpdateCoinAmount(moveOrder.Owner, travelCost * -1);
      moveOrder.TravelTime = gridService.GetConnectionTime((int)connectionId);
      moveOrder.AlreadyTraveled = new TimeSpan(0, 0, 0, 0);
      moveOrder.Done = false;

      MoveOrderQueue.Add(moveOrder);

      loggerService.Log($"Move Started: {moveOrder.Owner} payed {travelCost} Coins to move transporter {moveOrder.CargoTransporterId} from {startNode.Id} to {moveOrder.TargetNodeId}.");

      //Update Transporter
      transporterService.PutTransporterIntoMove(moveOrder.CargoTransporterId, moveOrder.Owner);

      return true;
    }

    public Task RunTick(int tick)
    {
      if (!IsSimRunning) return Task.CompletedTask;

      UpdateMoveOrders();

      UnloadTransporters();

      LoadTransporters();

      RemoveExpiredOrders();

      OrderGeneration(tick);

      MoveOrderQueue.RemoveAll(moveOrder => moveOrder.Done);

      return Task.CompletedTask;
    }

    private void LoadTransporters()
    {
      foreach (var owner in userService.GetAllUserIds())
      {
        var ownedTransporterNotInTransit = transporterService.GetAll(owner).Where(t => !t.InTransit).ToList();
        
        var acceptedOrders = orderService.GetForOwner(owner);
        foreach (var transporter in ownedTransporterNotInTransit)
        {
          if (transporter.Position == null) continue;
          var ordersStartingOnTransporterPosition =
            acceptedOrders.FindAll(order => order.OriginNode.Id == transporter.Position!.Id);
          
          transporterService.LoadOrdersToCargoTransporter(transporter.Id, owner, ordersStartingOnTransporterPosition);
        }
      }
    }

    private void UnloadTransporters()
    {
      foreach (var moveOrder in MoveOrderQueue.FindAll(moveOrder => moveOrder.Done))
      {
        var finishedOrders = transporterService.UnloadOrdersFromCargoTransporter(moveOrder.CargoTransporterId, moveOrder.Owner);
        foreach (var finishedOrder in finishedOrders.Where(finishedOrder => orderService.Finish(finishedOrder.Id, moveOrder.Owner)))
        {
          var orderValue = finishedOrder.Value;
          var percentageValue = 0.0;
          if (DateTime.UtcNow > finishedOrder.DeliveryDate)
          {
            var overTimePercentage = 100 / (finishedOrder.ExpirationDate - finishedOrder.DeliveryDate).TotalSeconds * 
                                     (DateTime.UtcNow - finishedOrder.DeliveryDate).TotalSeconds;
            percentageValue = orderValue / 100 * (100 - overTimePercentage);
            orderValue = (int)percentageValue;
            loggerService.Log($"UnloadTransporters: Order {finishedOrder.Id} over time - new orderValue {orderValue}, overtime percentage is {overTimePercentage}");
          }
          userService.UpdateCoinAmount(moveOrder.Owner, orderValue);
          orderService.RemoveAccepted(finishedOrder.Id, moveOrder.Owner);
          loggerService.Log($"UnloadTransporters: {moveOrder.Owner} made {orderValue} Coins of Order {finishedOrder.Id} after unloading transporter {moveOrder.CargoTransporterId} at {moveOrder.TargetNodeId}.");
        }
      }
    }

    private void RemoveExpiredOrders()
    { 
      orderService.RemoveExpiredAvailable();

      foreach (var owner in userService.GetAllUserIds())
      {
        var expiredOrders = orderService.GetForOwner(owner).Where(order => order.ExpirationDate < DateTime.UtcNow).ToList();
        foreach (var order in expiredOrders)
        {
          userService.UpdateCoinAmount(owner, order.Value * -1);
          transporterService.RemoveLoadedOrder(order.Id, owner);
          orderService.RemoveAccepted(order.Id, owner);
          loggerService.Log($"Order expired: The order {order.Id} accepted by {owner} is expired at the cost of {order.Value * -1} Coins.");
        }
      }
    }

    private void UpdateMoveOrders()
    {
      if (config == null) return;

      foreach (var moveOrder in MoveOrderQueue)
      {
        moveOrder.AlreadyTraveled += config.SimTick;
        if (moveOrder.TravelTime > moveOrder.AlreadyTraveled) continue;
        transporterService.PutTransporterIntoPosition(moveOrder.CargoTransporterId, moveOrder.Owner, moveOrder.TargetNodeId);
        moveOrder.Done = true;

        loggerService.Log($"Move Done: {moveOrder.Owner} moved transporter {moveOrder.CargoTransporterId} to {moveOrder.TargetNodeId}.");

      }
    }

    private void OrderGeneration(int tick)
    {
      if (config!.UseRandomOrders)
      {
        var random = new Random();
        if (random.Next(2) == 0)
        {
          orderService.Create();
        }
      }
      else
      {
        orderService.Prefabricated(tick);
      }
    }
  }
}
