namespace HahnCargoSim.Model.DTO
{
  public class CargoTransporterDto
  {
    public int Id { get; set; }
    public int PositionNodeId { get; set; }
    public bool InTransit { get; set; }
    public int Capacity { get; set; }
    public int Load { get; set; }
    public List<OrderDto>? LoadedOrders { get; set; }
  }
}
