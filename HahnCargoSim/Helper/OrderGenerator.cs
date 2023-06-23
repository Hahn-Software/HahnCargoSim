using System.Text.Json;
using HahnCargoSim.Model;
using HahnCargoSim.Model.DTO;
using HahnCargoSim.Services;
using HahnCargoSim.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HahnCargoSim.Helper
{
  public class OrderGenerator
  {
    //The generated Orders will be base on the current configured Grid and can only be run with this grid.
    public static void Run(IGridService gridService, int maxTicks, string filename)
    {
      var ordersPerTick = new Dictionary<int, List<FileOrder>>();
     
      var orderId = 0;
      for (var i = 1; i <= maxTicks; i++)
      {
        var random = new Random();
        if (random.Next(2) != 0) continue;

        var orders = new List<FileOrder>();
        var gridSizeFactor = gridService.GetGridSizeFactor();
        var rand = new Random();
        var orderAmount = rand.Next(gridSizeFactor) + 1;
        for (var j = 0; j < orderAmount; j++)
        {
          var order = Generate(gridService, orderId);
          orders.Add(new FileOrder(order));
          orderId++;
        }
        ordersPerTick.Add(i, orders);
      }

      FileHelper.WriteToFile("Order", JsonSerializer.Serialize(ordersPerTick), filename);
    }

    public static Order Generate(IGridService gridService, int orderId)
    {
      var gridSizeFactor = gridService.GetGridSizeFactor();
      var rand = new Random();

      var orgNode = gridService.GetRandomNode();
      var tarNode = gridService.GetRandomNode(new List<int> { orgNode.Id });
      var load = rand.Next(1, 101) + rand.Next(1, 101) + rand.Next(1, 101) + rand.Next(1, 701);
      var value = (rand.Next(3, 10) * gridSizeFactor) + (int)(rand.Next(1, 10) / 100.0 * load);
      var deliveryDate = DateTime.UtcNow + new TimeSpan(0, 0, rand.Next(1, 5) * gridSizeFactor, rand.Next(60));
      return new Order
      {
        Id = orderId,
        OriginNode = orgNode,
        TargetNode = tarNode,
        Load = load,
        Value = value,
        DeliveryDate = deliveryDate,
        ExpirationDate = deliveryDate + new TimeSpan(0, 0, gridSizeFactor + rand.Next(5), 0)
      };
    }
  }
}
