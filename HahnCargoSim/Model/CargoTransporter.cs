using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HahnCargoSim.Model.DTO;

namespace HahnCargoSim.Model
{
  public class CargoTransporter
  {
    public int Id { get; set; }
    public string Owner { get; set; }
    public Node? Position { get; set; }
    public bool InTransit { get; set; }
    public int Capacity { get; set; }
    public int Load { get; set; }
    public List<Order> LoadedList { get; set; }


    public CargoTransporterDto ToDto()
    {
      return new CargoTransporterDto
      {
        Id = this.Id,
        PositionNodeId = this.Position?.Id ?? -1,
        InTransit = this.InTransit,
        Capacity = this.Capacity,
        Load = this.Load,
        LoadedOrders = this.LoadedList.Select(order => order.ToDto()).ToList()
      };
    }
  }
}
