namespace HahnCargoSim.Model
{
  public class MoveOrder
  {
    public int Id { get; set; }
    public string Owner { get; set; }
    public int CargoTransporterId { get; set; }
    public int TargetNodeId { get; set; }
    public TimeSpan TravelTime { get; set; }
    public TimeSpan AlreadyTraveled { get; set; }
    public bool Done { get; set; }
  }
}
