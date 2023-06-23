using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HahnCargoSim.Model.DTO;

namespace HahnCargoSim.Model
{
  public class Order
  {
    public int Id { get; set; }
    public Node OriginNode { get; set; }
    public Node TargetNode { get; set; }
    public int Load { get; set; }
    public int Value { get; set; }
    public DateTime DeliveryDate { get; set; }
    public DateTime ExpirationDate { get; set; }


    public OrderDto ToDto()
    {
      return new OrderDto
      {
        Id = this.Id,
        OriginNodeId = this.OriginNode.Id,
        TargetNodeId = this.TargetNode.Id,
        Load = this.Load,
        Value = this.Value,
        DeliveryDateUtc = this.DeliveryDate.ToString(CultureInfo.InvariantCulture),
        ExpirationDateUtc = this.ExpirationDate.ToString(CultureInfo.InvariantCulture)
      };
    }

    public Order Clone()
    {
      return new Order
      {
        Id = this.Id,
        OriginNode = this.OriginNode,
        TargetNode = this.TargetNode,
        Load = this.Load,
        Value = this.Value,
        DeliveryDate = this.DeliveryDate,
        ExpirationDate = this.ExpirationDate
      };
    }
  }
}
