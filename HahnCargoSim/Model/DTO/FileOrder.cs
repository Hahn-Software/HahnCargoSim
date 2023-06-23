namespace HahnCargoSim.Model.DTO
{
  public class FileOrder
  {
    public int Id { get; set; }
    public Node OriginNode { get; set; }
    public Node TargetNode { get; set; }
    public int Load { get; set; }
    public int Value { get; set; }
    public TimeSpan DeliverySpan { get; set; }
    public TimeSpan ExpirationSpan { get; set; }

    public Order ToOrder()
    {
      return new Order
      {
        Id = this.Id,
        OriginNode = this.OriginNode,
        TargetNode = this.TargetNode,
        Load = this.Load,
        Value = this.Value,
        DeliveryDate = DateTime.UtcNow + this.DeliverySpan,
        ExpirationDate = DateTime.UtcNow + this.ExpirationSpan
      };
    }

    public FileOrder()
    {

    }

    public FileOrder(Order order)
    {
      this.Id = order.Id;
      this.OriginNode = order.OriginNode;
      this.TargetNode = order.TargetNode;
      this.Load = order.Load;
      this.Value = order.Value;
      this.DeliverySpan = order.DeliveryDate - DateTime.UtcNow;
      this.ExpirationSpan = order.ExpirationDate - DateTime.UtcNow;
    }


  }
}
