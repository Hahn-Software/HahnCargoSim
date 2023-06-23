namespace HahnCargoSim.Model.DTO
{
  public class OrderDto
  {
    public int Id { get; set; }
    public int OriginNodeId { get; set; }
    public int TargetNodeId { get; set; }
    public int Load { get; set; }
    public int Value { get; set; }
    public string DeliveryDateUtc { get; set; }
    public string ExpirationDateUtc { get; set; }
  }
}
