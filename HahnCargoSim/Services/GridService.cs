using System.Text.Json;
using HahnCargoSim.Helper;
using HahnCargoSim.Model;
using HahnCargoSim.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace HahnCargoSim.Services
{
  public class GridService : IGridService
  {

    private readonly SimConfig? config;

    private readonly Grid grid;
    private readonly string gridJson;

    public GridService(IOptions<SimConfig> config)
    {
      this.config = config.Value;
      grid = LoadGridFromJson();
      gridJson = LoadGridAsJson();
    }

    public Grid GetGrid()
    {
      return grid;
    }

    public string GetGridAsJson()
    {
      return gridJson;
    }

    public int? ConnectionAvailable(int sourceNodeId, int targetNodeId)
    {
      return (from connection in grid.Connections where (connection!.FirstNodeId == sourceNodeId && connection.SecondNodeId == targetNodeId) || (connection.FirstNodeId == targetNodeId && connection.SecondNodeId == sourceNodeId) select connection.Id).FirstOrDefault();
    }

    public int GetConnectionCost(int connectionId)
    {
      var con = GetConnection(connectionId);
      Edge? edge = null;
      if (con != null)
      {
        edge = GetEdge(con.EdgeId);
      }
      return edge?.Cost ?? -1;
    }

    public TimeSpan GetConnectionTime(int connectionId)
    {
      var con = GetConnection(connectionId);
      Edge? edge = null;
      if (con != null)
      {
        edge = GetEdge(con.EdgeId);
      }
      return edge?.Time ?? new TimeSpan(0,0,0,0);
    }

    public Node GetRandomNode(List<int>? excludedNodes = null)
    {
      var filteredList = grid.Nodes.ConvertAll(node => node.Clone()).Where(node => excludedNodes == null || !excludedNodes.Contains(node.Id)).ToList();

      var random = new Random();
      return filteredList[random.Next(filteredList.Count)];
    }

    public Node? GetNode(int nodeId)
    {
      return grid.Nodes.Find(node => node.Id == nodeId);
    }

    private Connection? GetConnection(int Id)
    {
      return grid.Connections.Find(c => c != null && c.Id == Id);
    }

    private Edge? GetEdge(int Id)
    {
      return grid.Edges.Find(e => e != null && e.Id == Id);
    }

    private Grid LoadGridFromJson()
    {
      var jsonString = File.ReadAllText(config.GridToLoad);
      return JsonSerializer.Deserialize<Grid>(jsonString)!;
    }

    private string LoadGridAsJson()
    {
      return File.ReadAllText(config.GridToLoad);
    }

    public void GenerateFile(int numberOfNodes, int numberOfEdges, int numberOfConnectionsPerNode, string filename)
    {
      GridGenerator.Run(numberOfNodes, numberOfEdges, numberOfConnectionsPerNode, filename);
    }

    public int GetGridSizeFactor()
    {
      var factor = 1;
      if(grid?.Nodes != null && grid.Nodes.Count > 10)
      {
        factor = grid.Nodes.Count / 10;
      }
      return factor;
    }
  }
}
