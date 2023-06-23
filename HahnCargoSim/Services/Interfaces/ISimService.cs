using HahnCargoSim.Model;

namespace HahnCargoSim.Services.Interfaces
{
  public interface ISimService
  {
    void Start();
    void Stop();
    bool QueueMoveOrder(MoveOrder moveOrder);
    Task RunTick(int tick);

  }
}
