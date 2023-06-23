using System.Threading;
using HahnCargoSim.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HahnCargoSim.Services
{
  public class SimServer: BackgroundService
  {
    private readonly TimeSpan simTick;
    private readonly ISimService simService;
    private readonly SimConfig? config;

    public SimServer(IOptions<SimConfig> config, ISimService simService)
    {
      this.config = config.Value;
      if (this.config == null)
      {
        throw new Exception("Config is null");
      }
      this.simService = simService;
      this.simTick = this.config.SimTick;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var tick = 0;
      while (!stoppingToken.IsCancellationRequested)
      {
        

        var startTime = DateTime.UtcNow;
        tick++;
        await simService.RunTick(tick);

        await Task.Run(() => WaitForTick(DateTime.UtcNow - startTime), stoppingToken);
      }
    }

    private void WaitForTick(TimeSpan timePassed)
    {
      var timeToNextTick = simTick.Subtract(timePassed);
      var timeToNextTickInMilliseconds = (int)timeToNextTick.TotalMilliseconds;
      if (timeToNextTickInMilliseconds < 0)
      {
        timeToNextTickInMilliseconds = 0;
      }
      Thread.Sleep(timeToNextTickInMilliseconds);
    }

  }
}
