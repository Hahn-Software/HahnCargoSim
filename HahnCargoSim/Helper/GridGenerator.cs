using System.Text.Json;
using HahnCargoSim.Model;
using Microsoft.IdentityModel.Tokens;

namespace HahnCargoSim.Helper
{
  public class GridGenerator
  {

    public static void Run(int numberOfNodes, int numberOfEdges, int numberOfConnectionsPerNode, string filename)
    {
      if(numberOfNodes < 2) { numberOfNodes = 2; }
      if(numberOfEdges < 1 ) { numberOfEdges = 1; }
      if(numberOfConnectionsPerNode < 1 ) { numberOfConnectionsPerNode = 1; }
      

      var nodes = new List<Node>();
      for(var i = 0; i < numberOfNodes; i++) { 
        nodes.Add(new Node { 
          Id = i,
          Name = RandomNameGenerator()
        }); 
      }

      var edges = new List<Edge>();
      var rnd =  new  Random();
      for (var i = 0; i < numberOfEdges; i++) {
        edges.Add(new Edge { 
          Id = i,
          Cost = rnd.Next(1, 10),
          Time = new TimeSpan(0,0,0,rnd.Next(10, 60)),
        });
      }

      var connections = new List<Connection>();
      var id = 0;
      foreach (var node in nodes)
      {
        //Remove self form possible destinations
        var notUsedNodes = nodes.Where(n => n.Id != node.Id).ToList(); 
        
        //Remove all nodes from possible destinations that have this as destination.
        var usedNodeIds = connections.Where(n => n.SecondNodeId == node.Id).Select(n => n.FirstNodeId).ToList();
        if (usedNodeIds.Any())
        {
          notUsedNodes.RemoveAll(n => usedNodeIds.Contains(n.Id));
        }
        
        for(var i = 0; i < numberOfConnectionsPerNode; i++)
        {
          if (notUsedNodes == null || notUsedNodes.Count <= 0) 
            break;

          var secoundNode = notUsedNodes[rnd.Next(notUsedNodes.Count)];
          connections.Add(new Connection
          {
            Id = id,
            EdgeId = edges[rnd.Next(edges.Count)].Id,
            FirstNodeId = node.Id,
            SecondNodeId = secoundNode.Id
          });
          id++;
          notUsedNodes.Remove(secoundNode);
        }
      }

      var grid = new Grid { Nodes = nodes, Edges = edges, Connections = connections };

      FileHelper.WriteToFile("Grid", JsonSerializer.Serialize(grid), filename);
    }


    

    private static string RandomNameGenerator()
    {
      var rnd = new Random();
      
      char[] name = new char[6];

      var dashPos = rnd.Next(1, 5);
      name[dashPos] = '-';

      for(int i = 0; i < name.Length; i++)
      {
        if (i == dashPos) continue;
        var numberOrLetter = rnd.Next(3);

        if(numberOrLetter == 0)
        {
          name[i] = Convert.ToChar(rnd.Next(48, 58));
          continue;
        }
        name[i] = Convert.ToChar(rnd.Next(65, 91));
      }
      
      return new string(name);
    }

    

  }
}
