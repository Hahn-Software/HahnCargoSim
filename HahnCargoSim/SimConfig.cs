namespace HahnCargoSim
{
  public class SimConfig
  {
    public string TokenSecret { get; set; }
    public string GridToLoad { get; set; }
    public bool UseRandomOrders { get; set; }
    public string OrdersToLoad { get; set; }
    public TimeSpan SimTick { get; set; }
  }
}
